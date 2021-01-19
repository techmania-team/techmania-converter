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
            // tracks 4-7 will be processed later;
            // tracks 8-15 are ignored.
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

            // TODO: 2nd pass - process chain and repeat notes
            // TODO: 3rd pass - process special events

            // Calculate bga offset.
            if (bgaStartPulse >= 0)
            {
                pattern.PrepareForTimeCalculation();
                pattern.patternMetadata.bgaOffset = pattern.PulseToTime(bgaStartPulse);
            }
        }

        private int TrackToLane(uint track)
        {
            if (track < 4) return (int)track;
            return (int)track - 16;
        }

        private Note EventDataToNote(EventData e, Func<int, int> TickToPulse)
        {
            int pulse = TickToPulse(e.Tick);
            int lane = TrackToLane(e.TrackId);
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
                            sound = e.Instrument.Name
                        };
                    }
                    else
                    {
                        DragNote dragNote = new DragNote()
                        {
                            type = NoteType.Drag,
                            pulse = pulse,
                            lane = lane,
                            sound = e.Instrument.Name
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
                        sound = e.Instrument.Name
                    };
                case 6:
                    return new Note()
                    {
                        type = NoteType.ChainNode,
                        pulse = pulse,
                        lane = lane,
                        sound = e.Instrument.Name
                    };
                case 10:
                    if (e.Duration == 6)
                    {
                        return new Note()
                        {
                            type = NoteType.RepeatHead,
                            pulse = pulse,
                            lane = lane,
                            sound = e.Instrument.Name
                        };
                    }
                    else
                    {
                        return new HoldNote()
                        {
                            type = NoteType.RepeatHeadHold,
                            pulse = pulse,
                            lane = lane,
                            sound = e.Instrument.Name,
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
                            sound = e.Instrument.Name
                        };
                    }
                    else
                    {
                        return new HoldNote()
                        {
                            type = NoteType.RepeatHold,
                            pulse = pulse,
                            lane = lane,
                            sound = e.Instrument.Name,
                            duration = TickToPulse(e.Duration)
                        };
                    }
                case 12:
                    return new HoldNote()
                    {
                        type = NoteType.Hold,
                        pulse = pulse,
                        lane = lane,
                        sound = e.Instrument.Name,
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
