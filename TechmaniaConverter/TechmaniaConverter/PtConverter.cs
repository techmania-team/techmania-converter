using DJMaxEditor.DJMax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TechmaniaConverter
{
    class PtConverter : ConverterBase
    {
        public HashSet<string> allInstruments { get; private set; }

        private const string unrecognizedFilenameMessage = "The file name must be in format <song_id>_<mode>_<level>.pt, where <mode> is either 'star' or 'pop', and <level> is one of '1', '2', '3' or '4'.";

        // Error reporting.
        private bool typeZeroWarning;
        private bool typeTwoWarning;
        private bool typeFourWarning;
        private bool playerTwoWarning;
        private bool nonDefaultVolumeWarning;
        private bool nonDefaultPanWarning;
        private HashSet<int> unknownAttributes;
        private bool endOfScanWarning;
        private List<Tuple<string, EventData>> unknownSpecialEvents;  // Item 1 is filename

        public void ExtractSongIdFrom(string filename)
        {
            Match match = Regex.Match(filename, @"(.*)_(star|pop)_[1-4]\.pt");
            if (!match.Success)
            {
                throw new Exception($"Unrecognized file name: {filename}. {unrecognizedFilenameMessage}");
            }

            track = new Track(match.Groups[1].Value, "");
            allInstruments = new HashSet<string>();
            typeZeroWarning = false;
            typeTwoWarning = false;
            typeFourWarning = false;
            playerTwoWarning = false;
            nonDefaultVolumeWarning = false;
            nonDefaultPanWarning = false;
            unknownAttributes = new HashSet<int>();
            endOfScanWarning = false;
            unknownSpecialEvents = new List<Tuple<string, EventData>>();
        }

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
            pattern.patternMetadata.bps = bps;
            pattern.patternMetadata.initBpm = parsedPt.Tempo;
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
            foreach (TrackData t in parsedPt.Tracks)
            {
                foreach (EventData e in t.Events)
                {
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
                            Note note = EventDataToNote(e, TickToPulse);
                            if (note != null)
                            {
                                pattern.notes.Add(note);
                            }
                            break;
                        case EventType.Volume:
                            typeTwoWarning = true;
                            break;
                        case EventType.Tempo:
                            pattern.bpmEvents.Add(new BpmEvent()
                            {
                                pulse = TickToPulse(e.Tick),
                                bpm = e.Tempo
                            });
                            break;
                        case EventType.Beat:
                            typeFourWarning = true;
                            break;
                    }
                }
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

            // 3rd pass: process special events.
            foreach (EventData e in specialEvents)
            {
                int pulse = TickToPulse(e.Tick);
                int lane = (int)e.TrackId - 4;

                // Does this specify an end-of-scan note?
                if (pattern.notes.Contains(new Note()
                {
                    pulse = pulse,
                    lane = lane
                }))
                {
                    // This will be supported at a later version.
                    endOfScanWarning = true;
                    continue;
                }

                // Find the drag note that this event modifies, if any.
                bool modifiesDragNote = false;
                foreach (DragNote d in allDragNotes)
                {
                    if (d.lane != lane) continue;
                    if (pulse < d.pulse) continue;
                    if (pulse > d.pulse + d.Duration()) continue;

                    modifiesDragNote = true;
                    int anchorPulse = pulse - d.pulse;
                    int anchorLane;
                    if (e.Attribute == 0 || e.Attribute == 60)
                    {
                        anchorLane = 0;
                    }
                    else if (e.Attribute < 60)
                    {
                        // Curve upwards.
                        anchorLane = -1;
                    }
                    else
                    {
                        // Curve downwards.
                        anchorLane = 1;
                    }

                    if (d.nodes[d.nodes.Count - 1].anchor.pulse == anchorPulse)
                    {
                        d.nodes[d.nodes.Count - 1].anchor.lane += anchorLane;
                    }
                    else
                    {
                        d.nodes.Add(new DragNode()
                        {
                            anchor = new IntPoint(anchorPulse, anchorLane),
                            controlLeft = new FloatPoint(0f, 0f),
                            controlRight = new FloatPoint(0f, 0f)
                        });
                    }
                    d.nodes.Sort((DragNode n1, DragNode n2) => n1.anchor.pulse - n2.anchor.pulse);
                    break;
                }

                if (!modifiesDragNote)
                {
                    // This special event doesn't do anything.
                    unknownSpecialEvents.Add(new Tuple<string, EventData>(filename, e));
                }
            }

            // Give the drag notes some curve.
            foreach (DragNote d in allDragNotes)
            {
                if (d.nodes.Count <= 2) continue;
                for (int i = 1; i < d.nodes.Count - 1; i++)
                {
                    int prevAnchorPulse = d.nodes[i - 1].anchor.pulse;
                    int anchorPulse = d.nodes[i].anchor.pulse;
                    int nextAnchorPulse = d.nodes[i + 1].anchor.pulse;

                    d.nodes[i].controlLeft.pulse = (prevAnchorPulse - anchorPulse) * 0.5f;
                    d.nodes[i].controlRight.pulse = (nextAnchorPulse - anchorPulse) * 0.5f;
                }
            }

            // Calculate bga offset. Even though there's no bga.
            pattern.PrepareForTimeCalculation();
            if (bgaStartPulse >= 0)
            {
                pattern.patternMetadata.bgaOffset = pattern.PulseToTime(bgaStartPulse);
            }

            // Remove excessive BPM events from the beginning. Sometimes there are like 20 of them.
            // What the heck.
            while (pattern.bpmEvents.Count > 0 &&
                pattern.bpmEvents[0].bpm == pattern.patternMetadata.initBpm)
            {
                pattern.bpmEvents.RemoveAt(0);
            }
        }

        private int TrackToLane(uint track)
        {
            if (track < 4) return (int)track;
            return (int)track - 12;
        }

        private Note EventDataToNote(EventData e, Func<int, int> TickToPulse)
        {
            int pulse = TickToPulse(e.Tick);
            int lane = TrackToLane(e.TrackId);
            string sound = e.Instrument != null ? e.Instrument.Name : "";
            switch (e.Attribute)
            {
                case 0:
                    if (e.Duration == 6)
                    {
                        return new Note()
                        {
                            type = NoteType.Basic,
                            pulse = pulse,
                            lane = lane,
                            sound = sound
                        };
                    }
                    else
                    {
                        DragNote dragNote = new DragNote()
                        {
                            type = NoteType.Drag,
                            pulse = pulse,
                            lane = lane,
                            sound = sound
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
                        sound = sound
                    };
                case 6:
                    return new Note()
                    {
                        type = NoteType.ChainNode,
                        pulse = pulse,
                        lane = lane,
                        sound = sound
                    };
                case 10:
                    if (e.Duration == 6)
                    {
                        return new Note()
                        {
                            type = NoteType.RepeatHead,
                            pulse = pulse,
                            lane = lane,
                            sound = sound
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
                            duration = TickToPulse(e.Duration)
                        };
                    }
                case 11:
                    if (e.Duration == 6)
                    {
                        return new Note()
                        {
                            type = NoteType.Repeat,
                            pulse = pulse,
                            lane = lane,
                            sound = sound
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
                            duration = TickToPulse(e.Duration)
                        };
                    }
                case 12:
                    return new HoldNote()
                    {
                        type = NoteType.Hold,
                        pulse = pulse,
                        lane = lane,
                        sound = sound,
                        duration = TickToPulse(e.Duration)
                    };
                default:
                    unknownAttributes.Add(e.Attribute);
                    return null;
            }
        }

        public void GenerateReport()
        {
            StringWriter writer = new StringWriter();
            if (typeZeroWarning)
            {
                writer.WriteLine("Events of type 0 (None) are not supported, and will be ignored.");
                writer.WriteLine();
            }
            if (typeTwoWarning)
            {
                writer.WriteLine("Events of type 2 (Volume) are not supported, and will be ignored. All notes will play at default volume.");
                writer.WriteLine();
            }
            if (typeFourWarning)
            {
                writer.WriteLine("Events of type 4 (Beat) are not supported, and will be ignored. Converter will assume 4/4 meter.");
                writer.WriteLine();
            }
            if (playerTwoWarning)
            {
                writer.WriteLine("Events on tracks 8-15 (P2 visible and P2 special) are not supported, and will be ignored.");
                writer.WriteLine();
            }
            if (nonDefaultVolumeWarning)
            {
                writer.WriteLine("Volume on notes are not supported, and will be ignored. All notes will play at default volume.");
                writer.WriteLine();
            }
            if (nonDefaultPanWarning)
            {
                writer.WriteLine("Pan on notes are not supported, and will be ignored. All notes will play at default pan.");
                writer.WriteLine();
            }
            if (unknownAttributes.Count > 0)
            {
                writer.WriteLine("The following event attributes are not recognized, and events with these attributes will be ignored:");
                foreach (int c in unknownAttributes)
                {
                    writer.Write(c + ", ");
                }
                writer.WriteLine();
                writer.WriteLine();
            }
            if (endOfScanWarning)
            {
                writer.WriteLine("End of scan notes are not supported, and will be ignored.");
                writer.WriteLine();
            }
            if (unknownSpecialEvents.Count > 0)
            {
                writer.WriteLine("The following special notes modify neither an end-of-scan note or a drag note. These special notes have no meaning, and will be ignored:");
                foreach (Tuple<string, EventData> tuple in unknownSpecialEvents)
                {
                    writer.WriteLine($"{tuple.Item1}, tick {tuple.Item2.Tick}, track {tuple.Item2.TrackId}");
                }
                writer.WriteLine();
            }
            report = writer.ToString();
            if (report == "")
            {
                report = "No problems found.";
            }
        }

        public string Serialize()
        {
            return track.Serialize();
        }
    }
}
