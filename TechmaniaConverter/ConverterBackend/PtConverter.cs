using DJMaxEditor.DJMax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ConverterBackend
{
    public class PtConverter : ConverterBase
    {
        public HashSet<string> allInstruments { get; private set; }
        // Disc image in the source becomes the eyecatch in the destination.
        public string sourceDiscImagePath { get; private set; }
        // Eyecatch in the source becomes the background image in the destination.
        public string sourceEyecatchPath { get; private set; }
        public string sourcePreviewPath { get; private set; }
        public string sourceBgaPath { get; private set; }
        public bool bgaConversionRequired { get; private set; }

        private const string unrecognizedFilenameMessage = "The file name must be in format <song_name>_<mode>_<level>.pt, where <mode> is either 'star' or 'pop', and <level> is one of '1', '2', '3' or '4'.";

        // Error reporting.
        private bool typeZeroWarning;
        private bool playerTwoWarning;
        private List<Tuple<string, EventData>> eventsWithUnknownAttribute;  // Item 1 is filename
        private List<Tuple<string, EventData>> unknownSpecialEvents;  // Item 1 is filename

        // Scroll speed tracking.
        private HashSet<Pattern> patternsWithScrollSpeedSet;

        #region Metadata
        private const float kDefaultTickToPulse = Pattern.pulsesPerBeat / 192f;  // This disregards BPS and assumes 192 ticks per measure.
        private const float kHpDeltaCoeff = 100f;

        public void ExtractShortNameAndInitialize(string filename)
        {
            Match match = Regex.Match(filename, @"(.*)_(star|pop)_[1-4]\.pt");
            if (!match.Success)
            {
                throw new Exception($"Unrecognized file name: {filename}. {unrecognizedFilenameMessage}");
            }

            track = new Track(match.Groups[1].Value.ToLower(), "");
            allInstruments = new HashSet<string>();
            typeZeroWarning = false;
            playerTwoWarning = false;
            eventsWithUnknownAttribute = new List<Tuple<string, EventData>>();
            unknownSpecialEvents = new List<Tuple<string, EventData>>();
            patternsWithScrollSpeedSet = new HashSet<Pattern>();

            reportWriter = new StringWriter();
        }

        // Assumes that the csv file exists.
        private List<string> FindLineInCsv(string csvPath, string shortName)
        {
            string text = File.ReadAllText(csvPath).Replace('\t', ',');  // Some flavors of discstock.csv uses tabs as separators.
            NReco.Csv.CsvReader csvReader = new NReco.Csv.CsvReader(new StringReader(text));
            while (csvReader.Read())
            {
                if (csvReader[1].ToLower() == shortName)
                {
                    List<string> line = new List<string>();
                    for (int i = 0; i < csvReader.FieldsCount; i++) line.Add(csvReader[i]);
                    return line;
                }
            }
            return null;
        }

        private void SetPatternLevel(string patternName, string levelString)
        {
            Pattern p = track.patterns.Find(p => p.patternMetadata.patternName == patternName);
            if (p != null)
            {
                int level;
                if (int.TryParse(levelString, out level))
                {
                    p.patternMetadata.level = level;
                }
                else
                {
                    reportWriter.WriteLine($"Warning: {patternName}'s level is missing or invalid in CSV; using default value of 1.");
                }
            }
        }

        private void SetPatternScrollSpeed(string patternName, string spString)
        {
            Pattern p = track.patterns.Find(p => p.patternMetadata.patternName == patternName);
            if (p == null) return;
            if (patternsWithScrollSpeedSet.Contains(p)) return;

            int sp;
            if (int.TryParse(spString, out sp))
            {
                p.patternMetadata.bps = ScrollSpeedToBps(sp);
                reportWriter.WriteLine($"Setting BPS of {patternName} to {p.patternMetadata.bps}.");
                patternsWithScrollSpeedSet.Add(p);
            }
            else
            {
                reportWriter.WriteLine($"Warning: {patternName}'s scroll speed is missing or invalid in CSV; using default value.");
            }
        }

        private void SetPatternDefaultScrollSpeed(string patternName, bool defaultIs1)
        {
            Pattern p = track.patterns.Find(p => p.patternMetadata.patternName == patternName);
            if (p == null) return;
            if (patternsWithScrollSpeedSet.Contains(p)) return;

            int scrollSpeed = defaultIs1 ? 1 : 2;
            p.patternMetadata.bps = ScrollSpeedToBps(scrollSpeed);
            reportWriter.WriteLine($"Setting BPS of {patternName} to the default value of {p.patternMetadata.bps}.");
            patternsWithScrollSpeedSet.Add(p);
        }

        private void AdjustDragNoteAnchorsForScrollSpeed(Pattern p)
        {
            if (p.patternMetadata.bps == 4) return;
            foreach (Note n in p.notes)
            {
                if (n.type != NoteType.Drag) continue;
                DragNote dragNote = n as DragNote;
                foreach (DragNode node in dragNote.nodes)
                {
                    node.anchor.lane *= 0.5f;
                }
            }
        }

        private LegacyRulesetOverride ReadBaseRule(string baseRuleFile)
        {
            LegacyRulesetOverride rule = new LegacyRulesetOverride();

            using StreamReader stream = new StreamReader(baseRuleFile);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            using XmlReader reader = XmlReader.Create(stream, settings);

            Action<string> proceedToElement = (string elementName) =>
            {
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    if (reader.Name == elementName) break;
                }
            };
            
            proceedToElement("PopMode");
            proceedToElement("Delta");
            proceedToElement("Delta");
            Func<string, float> attributeToPulse = (string attribute) => float.Parse(reader.GetAttribute(attribute)) * kDefaultTickToPulse;
            rule.timeWindows = new List<float>()
            {
                attributeToPulse("NormalPerPect2"),
                attributeToPulse("NormalPerPect"),
                attributeToPulse("NormalGreat"),
                attributeToPulse("NormalFair"),
                attributeToPulse("NormalMiss")
            };

            proceedToElement("Gauge");
            Func<string, int> attributeToHpDelta = (string attribute) => (int)(float.Parse(reader.GetAttribute(attribute)) * kHpDeltaCoeff);
            proceedToElement("Gauge");
            rule.hpDeltaBasic = new List<int>()
            {
                attributeToHpDelta("NormalPerPect2"),
                attributeToHpDelta("NormalPerPect"),
                attributeToHpDelta("NormalGreat"),
                attributeToHpDelta("NormalFair"),
                attributeToHpDelta("NormalMiss"),
                attributeToHpDelta("NormalFail")
            };
            proceedToElement("Gauge");
            rule.hpDeltaDrag = new List<int>()
            {
                attributeToHpDelta("LongPerPect2"),
                attributeToHpDelta("LongPerPect"),
                attributeToHpDelta("LongGreat"),
                attributeToHpDelta("LongFair"),
                attributeToHpDelta("LongMiss"),
                attributeToHpDelta("LongFail")
            };
            proceedToElement("Gauge");
            rule.hpDeltaHold = new List<int>()
            {
                attributeToHpDelta("HoldingPerPect2"),
                attributeToHpDelta("HoldingPerPect"),
                attributeToHpDelta("HoldingGreat"),
                attributeToHpDelta("HoldingFair"),
                attributeToHpDelta("HoldingMiss"),
                attributeToHpDelta("HoldingFail")
            };
            proceedToElement("Gauge");
            rule.hpDeltaChain = new List<int>()
            {
                attributeToHpDelta("PressPerPect2"),
                attributeToHpDelta("PressPerPect"),
                attributeToHpDelta("PressGreat"),
                attributeToHpDelta("PressFair"),
                attributeToHpDelta("PressMiss"),
                attributeToHpDelta("PressFail")
            };
            proceedToElement("Gauge");
            rule.hpDeltaRepeat = new List<int>()
            {
                attributeToHpDelta("OnePointPerPect2"),
                attributeToHpDelta("OnePointPerPect"),
                attributeToHpDelta("OnePointGreat"),
                attributeToHpDelta("OnePointFair"),
                attributeToHpDelta("OnePointMiss"),
                attributeToHpDelta("OnePointFail")
            };

            Func<List<int>, List<int>> hpDeltaDuringFeverFromHpDelta = (List<int> hpDelta) => new List<int>()
            {
                hpDelta[0],
                hpDelta[1],
                hpDelta[2],
                hpDelta[3],
                hpDelta[4],
                hpDelta[5]
            };
            rule.hpDeltaBasicDuringFever = hpDeltaDuringFeverFromHpDelta(rule.hpDeltaBasic);
            rule.hpDeltaDragDuringFever = hpDeltaDuringFeverFromHpDelta(rule.hpDeltaDrag);
            rule.hpDeltaHoldDuringFever = hpDeltaDuringFeverFromHpDelta(rule.hpDeltaHold);
            rule.hpDeltaChainDuringFever = hpDeltaDuringFeverFromHpDelta(rule.hpDeltaChain);
            rule.hpDeltaRepeatDuringFever = hpDeltaDuringFeverFromHpDelta(rule.hpDeltaRepeat);

            return rule;
        }

        private LegacyRulesetOverride ReadSongRule(string ruleFile)
        {
            LegacyRulesetOverride rule = new LegacyRulesetOverride();
            return rule;
        }

        private LegacyRulesetOverride CombineRule(LegacyRulesetOverride baseRule, LegacyRulesetOverride songRule)
        {
            LegacyRulesetOverride rule = new LegacyRulesetOverride();
            return rule;
        }

        public void SearchForMetadata(string ptFolderString)
        {
            Folder ptFolder = new Folder(ptFolderString);
            string shortName = track.trackMetadata.title;

            Folder resourceFolder = ptFolder.GoUp().GoUp().Open("Resource");
            Folder discInfoFolder = resourceFolder.Open("DiscInfo");

            // Search for title, artist, genre and difficulty levels in discstock.csv.
            string discstockPath = discInfoFolder.OpenFile("discstock.csv");
            if (File.Exists(discstockPath))
            {
                reportWriter.WriteLine("Found discstock.csv.");
                List<string> line = FindLineInCsv(discstockPath, shortName);
                if (line != null)
                {
                    reportWriter.WriteLine("Found title, artist, genre and difficulty levels.");
                    track.trackMetadata.title = line[2];
                    track.trackMetadata.genre = line[3];
                    track.trackMetadata.artist = line[4];
                    SetPatternLevel("Star NM", line[^8]);
                    SetPatternLevel("Star HD", line[^7]);
                    SetPatternLevel("Star MX", line[^6]);
                    SetPatternLevel("Star EX", line[^5]);
                    SetPatternLevel("Pop NM", line[^4]);
                    SetPatternLevel("Pop HD", line[^3]);
                    SetPatternLevel("Pop MX", line[^2]);
                    SetPatternLevel("Pop EX", line[^1]);
                }
                else
                {
                    reportWriter.WriteLine($"But did not find title, artist, genre and difficulty levels for {shortName}.");
                }
            }

            // Search for scroll speed in (star|pop)_stage_(1|2|3|bonus).csv.
            string[] starStageFiles = { "star_stage_1.csv", "star_stage_2.csv", "star_stage_3.csv", "star_stage_bonus.csv" };
            string[] popStageFiles = { "pop_stage_1.csv", "pop_stage_2.csv", "pop_stage_3.csv", "pop_stage_bonus.csv" };
            foreach (string starStageFilename in starStageFiles)
            {
                string starStagePath = discInfoFolder.OpenFile(starStageFilename);
                if (!File.Exists(starStagePath)) continue;
                List<string> line = FindLineInCsv(starStagePath, shortName);
                if (line == null) continue;

                reportWriter.WriteLine($"Found scroll speed for Star patterns in {starStageFilename}.");
                SetPatternScrollSpeed("Star NM", line[2]);
                SetPatternScrollSpeed("Star HD", line[5]);
                SetPatternScrollSpeed("Star MX", line[8]);
                SetPatternScrollSpeed("Star EX", line[11]);
            }
            SetPatternDefaultScrollSpeed("Star NM", PtOptions.instance.scrollSpeedDefaultsToOneOnStar[0]);
            SetPatternDefaultScrollSpeed("Star HD", PtOptions.instance.scrollSpeedDefaultsToOneOnStar[1]);
            SetPatternDefaultScrollSpeed("Star MX", PtOptions.instance.scrollSpeedDefaultsToOneOnStar[2]);
            SetPatternDefaultScrollSpeed("Star EX", PtOptions.instance.scrollSpeedDefaultsToOneOnStar[3]);
            foreach (string popStageFilename in popStageFiles)
            {
                string popStagePath = discInfoFolder.OpenFile(popStageFilename);
                if (!File.Exists(popStagePath)) continue;
                List<string> line = FindLineInCsv(popStagePath, shortName);
                if (line == null) continue;

                reportWriter.WriteLine($"Found scroll speed for Pop patterns in {popStageFilename}.");
                SetPatternScrollSpeed("Pop NM", line[2]);
                SetPatternScrollSpeed("Pop HD", line[5]);
                SetPatternScrollSpeed("Pop MX", line[8]);
                SetPatternScrollSpeed("Pop EX", line[11]);
            }
            SetPatternDefaultScrollSpeed("Pop NM", PtOptions.instance.scrollSpeedDefaultsToOneOnPop[0]);
            SetPatternDefaultScrollSpeed("Pop HD", PtOptions.instance.scrollSpeedDefaultsToOneOnPop[1]);
            SetPatternDefaultScrollSpeed("Pop MX", PtOptions.instance.scrollSpeedDefaultsToOneOnPop[2]);
            SetPatternDefaultScrollSpeed("Pop EX", PtOptions.instance.scrollSpeedDefaultsToOneOnPop[3]);

            // Adjust drag notes, if necessary.
            track.patterns.ForEach(p => AdjustDragNoteAnchorsForScrollSpeed(p));

            // Search for disc image.
            List<string> candidates = new List<string>();
            sourceDiscImagePath = null;
            Folder discImgFolder = resourceFolder.Open("Discimg");
            for (int i = 0; i <= 4; i++)
            {
                candidates.Add(discImgFolder.OpenFile($"{shortName}_{i}.png"));
                candidates.Add(discImgFolder.OpenFile($"{shortName}_{i}.jpg"));
            }
            candidates.Add(ptFolder.OpenFile($"{shortName}_disc.png"));
            candidates.Add(ptFolder.OpenFile($"{shortName}_disc.jpg"));
            foreach (string candidate in candidates)
            {
                if (candidate == null) continue;
                if (File.Exists(candidate))
                {
                    sourceDiscImagePath = candidate;
                    string extension = Path.GetExtension(sourceDiscImagePath); // Includes the dot
                    string destinationFilename = $"{shortName}_disc{extension}";
                    reportWriter.WriteLine($"Found disc image at {sourceDiscImagePath}.");
                    track.trackMetadata.eyecatchImage = destinationFilename;
                    break;
                }
            }

            // Search for eyecatch.
            candidates.Clear();
            sourceEyecatchPath = null;
            Folder eyecatchFolder = resourceFolder.Open("Eyecatch").Open("Song");
            for (int i = 0; i <= 4; i++)
            {
                candidates.Add(eyecatchFolder.OpenFile($"{shortName}_{i}.png"));
                candidates.Add(eyecatchFolder.OpenFile($"{shortName}_{i}.jpg"));
            }
            candidates.Add(ptFolder.OpenFile($"{shortName}_eyecatch.png"));
            candidates.Add(ptFolder.OpenFile($"{shortName}_eyecatch.jpg"));
            foreach (string candidate in candidates)
            {
                if (candidate == null) continue;
                if (File.Exists(candidate))
                {
                    sourceEyecatchPath = candidate;
                    string extension = Path.GetExtension(sourceEyecatchPath); // Includes the dot
                    string destinationFilename = $"{shortName}_eyecatch{extension}";
                    reportWriter.WriteLine($"Found eyecatch image at {sourceEyecatchPath}.");
                    foreach (Pattern p in track.patterns) p.patternMetadata.backImage = destinationFilename;
                    break;
                }
            }

            // Search for preview.
            candidates.Clear();
            sourcePreviewPath = null;
            candidates.Add(resourceFolder.Open("Preview").OpenFile($"{shortName}.ogg"));
            candidates.Add(resourceFolder.Open("Previewogg").OpenFile($"{shortName}.ogg"));
            candidates.Add(ptFolder.OpenFile($"{shortName}_pre.ogg"));
            foreach (string candidate in candidates)
            {
                if (candidate == null) continue;
                if (File.Exists(candidate))
                {
                    sourcePreviewPath = candidate;
                    string extension = Path.GetExtension(sourcePreviewPath); // Includes the dot
                    string destinationFilename = $"{shortName}_pre{extension}";
                    reportWriter.WriteLine($"Found preview at {sourcePreviewPath}.");
                    track.trackMetadata.previewTrack = destinationFilename;
                    break;
                }
            }

            // Search for BGA.
            candidates.Clear();
            sourceBgaPath = null;
            Folder rootFolder = resourceFolder.GoUp();
            for (int i = 0; i <= 20; i++)
            {
                string folderName = i == 0 ? "Movie" : $"Movie{i}";
                candidates.Add(rootFolder.Open(folderName).OpenFile($"{shortName}.bik"));
            }
            candidates.Add(ptFolder.OpenFile($"{shortName}.mp4"));
            foreach (string candidate in candidates)
            {
                if (candidate == null) continue;
                if (File.Exists(candidate))
                {
                    sourceBgaPath = candidate;
                    string extension = Path.GetExtension(sourceBgaPath); // Includes the dot
                    string destinationFilename = $"{shortName}.mp4";
                    reportWriter.WriteLine($"Found BGA at {sourceBgaPath}.");
                    foreach (Pattern p in track.patterns) p.patternMetadata.bga = destinationFilename;

                    if (extension.ToLower() == ".bik")
                    {
                        reportWriter.WriteLine("Conversion from .bik to .mp4 may take a few minutes.");
                        bgaConversionRequired = true;
                    }
                    else
                    {
                        bgaConversionRequired = false;
                    }
                    break;
                }
            }

            // Search for script.
            Folder scriptFolder = resourceFolder.Open("Script");
            string baseRuleFile = scriptFolder.Open("Maingame").OpenFile("JudgmentTempInfo.xml");
            Folder songScriptFolder = scriptFolder.Open("Song").Open(shortName);
            if (File.Exists(baseRuleFile) && songScriptFolder.Exists())
            {
                LegacyRulesetOverride baseRule = null;

                Action<string, string> readScript = (string filename, string patternName) =>
                {
                    string fullPath = songScriptFolder.OpenFile(filename);
                    if (!File.Exists(fullPath)) return;
                    if (baseRule == null) baseRule = ReadBaseRule(baseRuleFile);

                    LegacyRulesetOverride songRule = ReadSongRule(fullPath);
                    track.patterns.Find((Pattern p) => { return p.patternMetadata.patternName == patternName; }).legacyRulesetOverride =
                        CombineRule(baseRule, songRule);
                };
                readScript($"{shortName}_star_1.ini", "Star NM");
                readScript($"{shortName}_star_2.ini", "Star HD");
                readScript($"{shortName}_star_3.ini", "Star MX");
                readScript($"{shortName}_star_4.ini", "Star EX");
                readScript($"{shortName}_pop_1.ini", "Pop NM");
                readScript($"{shortName}_pop_2.ini", "Pop HD");
                readScript($"{shortName}_pop_3.ini", "Pop MX");
                readScript($"{shortName}_pop_4.ini", "Pop EX");
            }

            reportWriter.WriteLine();
        }
        #endregion

        #region Conversion
        // filename should be all lower case.
        public void ConvertAndAddPattern(string filename, PlayerData parsedPt)
        {
            Match match = Regex.Match(filename, @"(.*)_(star|pop)_([1-4])\.pt");
            if (!match.Success)
            {
                throw new Exception($"Unrecognized file name: {filename}. {unrecognizedFilenameMessage}");
            }

            StringBuilder patternNameBuilder = new StringBuilder();
            // C# 8.0 Hyyyyyyyyype!
            patternNameBuilder.Append(match.Groups[2].Value switch
            {
                "star" => "Star",
                "pop" => "Pop",
                _ => ""
            });
            patternNameBuilder.Append(' ');
            patternNameBuilder.Append(match.Groups[3].Value switch
            {
                "1" => "NM",
                "2" => "HD",
                "3" => "MX",
                "4" => "EX",
                _ => ""
            });

            Pattern pattern = new Pattern();
            pattern.patternMetadata.patternName = patternNameBuilder.ToString();
            pattern.patternMetadata.controlScheme = ControlScheme.Touch;
            pattern.patternMetadata.playableLanes = 4;
            pattern.patternMetadata.bps = bps;
            pattern.patternMetadata.initBpm = parsedPt.Tempo;
            pattern.patternMetadata.waitForEndOfBga = false;
            track.patterns.Add(pattern);
            bgaStartPulse = -1;

            // Conversion between ticks and pulses.
            int ticksPerMeasure = parsedPt.TickPerMinute;
            Func<int, int> TickToPulse = (int tick) => { return tick * pulsesPerScan / ticksPerMeasure; };

            // Copy all instruments, in preparation for copying files.
            foreach (InstrumentData i in parsedPt.Instruments)
            {
                if (i.Name == "") continue;
                allInstruments.Add(i.Name);
            }

            // 1st pass: convert all tracks by face value, except:
            // - tracks 4-7 will be processed later;
            // - tracks 8-15 are ignored.
            List<EventData> specialEvents = new List<EventData>();
            Dictionary<uint, int> trackVolumePercent = new Dictionary<uint, int>();
            bool anyNoteInLane2 = false, anyNoteInLane3 = false;
            foreach (TrackData t in parsedPt.Tracks)
            {
                foreach (EventData e in t.Events)
                {
                    if (e.TrackId == 18 && e.Tick == 0 && e.EventType == EventType.Note && PtOptions.instance.loadScrollSpeedFromTrack18)
                    {
                        pattern.patternMetadata.bps = ScrollSpeedToBps(e.Attribute);
                        reportWriter.WriteLine($"Loaded scroll speed for {filename} from track 18; BPS set to {pattern.patternMetadata.bps}.");
                        patternsWithScrollSpeedSet.Add(pattern);

                        // Do not parse this event as a note.
                        continue;
                    }

                    // Conveniently, events in a track are sorted by tick.
                    switch (e.EventType)
                    {
                        case EventType.None:
                            typeZeroWarning = true;
                            break;
                        case EventType.Note:
                            if (e.TrackId >= 4 && e.TrackId < 8)
                            {
                                // To be processed later.
                                specialEvents.Add(e);
                                break;
                            }
                            else if (e.TrackId >= 8 && e.TrackId < 16)
                            {
                                playerTwoWarning = true;
                                break;
                            }
                            if (e.Attribute == 100)
                            {
                                bgaStartPulse = TickToPulse(e.Tick);
                                break;
                            }
                            Note note = EventDataToNote(filename, e, TickToPulse, trackVolumePercent);
                            if (note == null)
                            {
                                break;
                            }
                            pattern.notes.Add(note);
                            if (note.lane == 2) anyNoteInLane2 = true;
                            if (note.lane == 3) anyNoteInLane3 = true;
                            break;
                        case EventType.Volume:
                            trackVolumePercent[e.TrackId] = IntVolumeToPercent(e.Volume);
                            break;
                        case EventType.Tempo:
                            if (e.Tick == 0 && MathF.Abs(e.Tempo - (float)pattern.patternMetadata.initBpm) < 0.001f)
                            {
                                break;
                            }
                            pattern.bpmEvents.Add(new BpmEvent()
                            {
                                pulse = TickToPulse(e.Tick),
                                bpm = e.Tempo
                            });
                            break;
                        case EventType.Beat:
                            // Do nothing.
                            break;
                    }
                }
            }
            if (!anyNoteInLane2 && !anyNoteInLane3)
            {
                pattern.patternMetadata.playableLanes = 2;
            }
            else if (!anyNoteInLane3)
            {
                pattern.patternMetadata.playableLanes = 3;
            }

            // 2nd pass: process chain and repeat notes. Also collect all drag notes
            // for the 3rd pass to use.
            int chainHeadPulse = -1;
            List<Note> convertedBasicNotes = new List<Note>();
            int[] repeatHeadPulse = { -1, -1, -1, -1 };
            List<DragNote> allDragNotes = new List<DragNote>();
            foreach (Note n in pattern.notes)
            {
                if (n.lane >= 4) continue;
                switch (n.type)
                {
                    case NoteType.ChainHead:
                        chainHeadPulse = n.pulse;
                        break;
                    case NoteType.Basic:
                        if (chainHeadPulse >= 0 && n.pulse > chainHeadPulse)
                        {
                            n.type = NoteType.ChainNode;
                            convertedBasicNotes.Add(n);
                        }
                        break;
                    case NoteType.ChainNode:
                        chainHeadPulse = -1;
                        // Basic notes on the same pulse shouldn't be converted.
                        foreach (Note converted in convertedBasicNotes)
                        {
                            if (converted.pulse == n.pulse)
                            {
                                converted.type = NoteType.Basic;
                            }
                        }
                        convertedBasicNotes.Clear();
                        break;
                    case NoteType.RepeatHead:
                    case NoteType.RepeatHeadHold:
                        if (repeatHeadPulse[n.lane] < 0)
                        {
                            // 1st repeat head. Convert all repeat heads after this one.
                            repeatHeadPulse[n.lane] = n.pulse;
                        }
                        else
                        {
                            // Between 1st repeat head and repeat note. Convert this head.
                            n.type = n.type switch
                            {
                                NoteType.RepeatHead => NoteType.Repeat,
                                NoteType.RepeatHeadHold => NoteType.RepeatHold,
                                _ => NoteType.Basic  // Should never happen
                            };
                        }
                        break;
                    case NoteType.Repeat:
                    case NoteType.RepeatHold:
                        repeatHeadPulse[n.lane] = -1;
                        break;
                    case NoteType.Drag:
                        allDragNotes.Add(n as DragNote);
                        break;
                    default:
                        break;
                }
            }

            // 3rd pass: process special events. Within the same track, specialEvents are sorted by tick.
            //
            // If a drag note is modified by any special event, the originally last node created from note duration
            // should be deleted.
            HashSet<DragNote> modifiedDragNotes = new HashSet<DragNote>();
            // However, a special event at the just right pulse may overwrite this node, in which case we should NOT
            // delete this node.
            HashSet<DragNote> dragNotesWithLastNodeOverwritten = new HashSet<DragNote>();
            foreach (EventData e in specialEvents)
            {
                int pulse = TickToPulse(e.Tick);
                int lane = (int)e.TrackId - 4;

                // Does this specify an end-of-scan note?
                Note modifiedNote = pattern.GetNoteAt(pulse, lane);
                if (modifiedNote != null)
                {
                    modifiedNote.endOfScan = true;
                    continue;
                }

                // Find the drag note that this event modifies, if any.
                bool modifiesDragNote = false;
                foreach (DragNote d in allDragNotes)
                {
                    if (d.lane != lane) continue;
                    if (pulse < d.pulse) continue;
                    if (pulse > d.pulse + d.nodes[^1].anchor.pulse) continue;

                    modifiesDragNote = true;
                    modifiedDragNotes.Add(d);
                    float anchorPulse = pulse - d.pulse;
                    float anchorLane;
                    if (e.Attribute == 0 || e.Attribute == 60)
                    {
                        anchorLane = 0;
                    }
                    else
                    {
                        float distanceToPreviousNode = 0;
                        for (int i = d.nodes.Count - 1; i >= 0; i--)
                        {
                            if (d.nodes[i].anchor.pulse < anchorPulse)
                            {
                                distanceToPreviousNode = anchorPulse - d.nodes[i].anchor.pulse;
                                break;
                            }
                        }
                        anchorLane = (e.Attribute - 60f) * distanceToPreviousNode / 5400f;
                        anchorLane = anchorLane / 4f * pattern.patternMetadata.playableLanes;
                    }

                    if (d.nodes[^1].anchor.pulse == anchorPulse)
                    {
                        d.nodes[^1].anchor.lane += anchorLane;
                        dragNotesWithLastNodeOverwritten.Add(d);
                    }
                    else
                    {
                        d.nodes.Add(new DragNode()
                        {
                            anchor = new FloatPoint(anchorPulse, anchorLane),
                            controlLeft = new FloatPoint(0f, 0f),
                            controlRight = new FloatPoint(0f, 0f)
                        });
                        d.nodes.Sort((DragNode n1, DragNode n2) => MathF.Sign(n1.anchor.pulse - n2.anchor.pulse));
                    }
                    break;
                }

                if (!modifiesDragNote)
                {
                    // This special event doesn't do anything.
                    unknownSpecialEvents.Add(new Tuple<string, EventData>(filename, e));
                }
            }

            // Delete last nodes of modified drag notes.
            foreach (DragNote n in allDragNotes)
            {
                if (modifiedDragNotes.Contains(n) &&
                    !dragNotesWithLastNodeOverwritten.Contains(n))
                {
                    n.nodes.RemoveAt(n.nodes.Count - 1);
                }
            }

            // Calculate bga offset. Even though there's no bga.
            pattern.PrepareForTimeCalculation();
            if (bgaStartPulse >= 0)
            {
                pattern.patternMetadata.bgaOffset = pattern.PulseToTime(bgaStartPulse);
            }
        }

        #region Volume and pan
        // Transforms [0, 1] to [0, 1] based on the given curve type and parameter. Output is clamped.
        private static float ApplyCurve(float input, VolumnPanCurveType type, float param)
        {
            float output = input;
            switch (type)
            {
                case VolumnPanCurveType.Exponential:
                    output = MathF.Pow(input, param);
                    break;
                case VolumnPanCurveType.Logarithmic:
                    output = MathF.Log(input, param) + 1f;
                    break;
            }
            if (float.IsNaN(output))
            {
                throw new ArgumentException("Unexpected NaN. Please make sure that parameters are within their valid ranges.");
            }
            if (output < 0f) return 0f;
            if (output > 1f) return 1f;
            return output;
        }

        private static int IntVolumeToPercent(int v)
        {
            float normalized = v / 127f;
            float curved = ApplyCurve(normalized, PtOptions.instance.volumeCurve, PtOptions.instance.volumeParam);
            return (int)MathF.Floor(curved * 100f);
        }

        private static int IntPanToPercent(int p)
        {
            p -= 64;
            if (p == 0) return 0;
            float sign;
            float normalized;
            if (p < 0)
            {
                sign = -1f;
                normalized = -p / 64f;
            }
            else
            {
                sign = 1f;
                normalized = p / 63f;
            }

            float curved = ApplyCurve(normalized, PtOptions.instance.panCurve, PtOptions.instance.panParam) * sign;
            return (int)MathF.Floor(curved * 100f);
        }
        #endregion

        private static int TrackToLane(uint track)
        {
            if (track < 4) return (int)track;
            int lane = (int)track - 12;
            if (lane >= maxLanes)
            {
                throw new Exception("Too many tracks in .pt, unable to convert. Please choose a .pt with fewer tracks.");
            }
            return lane;
        }

        private static int ScrollSpeedToBps(int scrollSpeed)
        {
            return 8 / scrollSpeed;
        }

        // Returns null on unknown attribute.
        private Note EventDataToNote(string filename, EventData e, Func<int, int> TickToPulse,
            Dictionary<uint, int> trackVolumePercent)
        {
            int pulse = TickToPulse(e.Tick);
            int lane = TrackToLane(e.TrackId);
            string sound = e.Instrument != null ? e.Instrument.Name : "";
            int volumeBasePercent = trackVolumePercent.ContainsKey(e.TrackId) ?
                trackVolumePercent[e.TrackId] : 100;
            if (PtOptions.instance.ignoreVolumeNotes) volumeBasePercent = 100;
            int volumePercent = IntVolumeToPercent(e.Vel) * volumeBasePercent / 100;
            int panPercent = IntPanToPercent(e.Pan);
            switch (e.Attribute)
            {
                case 0:
                    if (e.Duration <= 6)
                    {
                        return new Note()
                        {
                            type = NoteType.Basic,
                            pulse = pulse,
                            lane = lane,
                            sound = sound,
                            volumePercent = volumePercent,
                            panPercent = panPercent,
                            endOfScan = false
                        };
                    }
                    else
                    {
                        DragNote dragNote = new DragNote()
                        {
                            type = NoteType.Drag,
                            pulse = pulse,
                            lane = lane,
                            sound = sound,
                            volumePercent = volumePercent,
                            panPercent = panPercent,
                            curveType = global::CurveType.BSpline
                        };
                        dragNote.nodes.Add(new DragNode());
                        dragNote.nodes.Add(new DragNode());
                        dragNote.nodes[1].anchor.pulse = TickToPulse(e.Duration);
                        return dragNote;
                    }
                case 5:
                    return new Note()
                    {
                        type = NoteType.ChainHead,
                        pulse = pulse,
                        lane = lane,
                        sound = sound,
                        volumePercent = volumePercent,
                        panPercent = panPercent,
                        endOfScan = false
                    };
                case 6:
                    return new Note()
                    {
                        type = NoteType.ChainNode,
                        pulse = pulse,
                        lane = lane,
                        sound = sound,
                        volumePercent = volumePercent,
                        panPercent = panPercent,
                        endOfScan = false
                    };
                case 10:
                    if (e.Duration <= 6)
                    {
                        return new Note()
                        {
                            type = NoteType.RepeatHead,
                            pulse = pulse,
                            lane = lane,
                            sound = sound,
                            volumePercent = volumePercent,
                            panPercent = panPercent,
                            endOfScan = false
                        };
                    }
                    else
                    {
                        return new HoldNote()
                        {
                            type = NoteType.RepeatHeadHold,
                            pulse = pulse,
                            lane = lane,
                            sound = sound,
                            duration = TickToPulse(e.Duration),
                            volumePercent = volumePercent,
                            panPercent = panPercent,
                            endOfScan = false
                        };
                    }
                case 11:
                    if (e.Duration <= 6)
                    {
                        return new Note()
                        {
                            type = NoteType.Repeat,
                            pulse = pulse,
                            lane = lane,
                            sound = sound,
                            volumePercent = volumePercent,
                            panPercent = panPercent,
                            endOfScan = false
                        };
                    }
                    else
                    {
                        return new HoldNote()
                        {
                            type = NoteType.RepeatHold,
                            pulse = pulse,
                            lane = lane,
                            sound = sound,
                            duration = TickToPulse(e.Duration),
                            volumePercent = volumePercent,
                            panPercent = panPercent,
                            endOfScan = false
                        };
                    }
                case 12:
                    return new HoldNote()
                    {
                        type = NoteType.Hold,
                        pulse = pulse,
                        lane = lane,
                        sound = sound,
                        duration = TickToPulse(e.Duration),
                        volumePercent = volumePercent,
                        panPercent = panPercent,
                        endOfScan = false
                    };
                default:
                    eventsWithUnknownAttribute.Add(new Tuple<string, EventData>(filename, e));
                    return null;
            }
        }
        #endregion

        public void GenerateReport()
        {
            if (reportWriter == null) reportWriter = new StringWriter();

            if (typeZeroWarning)
            {
                reportWriter.WriteLine("Events of type 0 (None) are not supported, and will be ignored.");
                reportWriter.WriteLine();
            }
            if (playerTwoWarning)
            {
                reportWriter.WriteLine("Events on tracks 8-15 (P2 visible and P2 special) are not supported, and will be ignored.");
                reportWriter.WriteLine();
            }
            if (eventsWithUnknownAttribute.Count > 0)
            {
                reportWriter.WriteLine("The following notes contain unknown attributes and will be ignored:");
                foreach (Tuple<string, EventData> tuple in eventsWithUnknownAttribute)
                {
                    reportWriter.WriteLine($"{tuple.Item1}, track {tuple.Item2.TrackId}, tick {tuple.Item2.Tick}, attribute {tuple.Item2.Attribute}");
                }
                reportWriter.WriteLine();
                reportWriter.WriteLine();
            }
            if (unknownSpecialEvents.Count > 0)
            {
                reportWriter.WriteLine("The following special notes modify neither an end-of-scan note or a drag note. These special notes have no meaning, and will be ignored:");
                foreach (Tuple<string, EventData> tuple in unknownSpecialEvents)
                {
                    reportWriter.WriteLine($"{tuple.Item1}, track {tuple.Item2.TrackId}, tick {tuple.Item2.Tick}");
                }
                reportWriter.WriteLine();
            }
        }
    }
}
