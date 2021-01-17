using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TechmaniaConverter
{
    class BmsConverter
    {
        public string report { get; private set; }
        public BmsConverter()
        {
            report = null;
        }

        private Track track;
        private Pattern pattern;
        private int bgaStartPulse;
        private string longNoteCloser;
        private const int bps = 4;
        private const int pulsesPerScan = Pattern.pulsesPerBeat * bps;
        private const int maxLanes = 12;

        public Dictionary<string, string> keysoundIndexToName { get; private set; }
        private Dictionary<string, double> bpmIndexToValue;
        public Dictionary<string, string> bmpIndexToName { get; private set; }

        // This does not include full paths.
        public HashSet<string> allFilenamesInBmsFolder;

        private Dictionary<int, int> numNotesAtPulse;

        public string ConvertBmsToTech(string bms)
        {
            StringReader reader = new StringReader(bms);

            track = new Track("", "");  // To be filled later
            pattern = new Pattern();
            pattern.patternMetadata.patternName = "Converted from BMS";
            pattern.patternMetadata.controlScheme = ControlScheme.Keys;
            pattern.patternMetadata.bps = bps;
            pattern.patternMetadata.bga = "";
            track.patterns.Add(pattern);

            char[] delim = { ' ', ':' };
            longNoteCloser = "";

            // Lookup tables.
            keysoundIndexToName = new Dictionary<string, string>();
            bpmIndexToValue = new Dictionary<string, double>();
            bmpIndexToName = new Dictionary<string, string>();
            numNotesAtPulse = new Dictionary<int, int>();

            // Error reporting.
            HashSet<string> ignoredCommands = new HashSet<string>();
            HashSet<string> ignoredChannels = new HashSet<string>();
            bool meterWarning = false;
            bool nonVideoBmpWarning = false;
            bool lnTypeTwo = false;
            bool multipleBga = false;

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;
                line = line.Trim();
                if (line == "") continue;
                if (line[0] != '#') continue;

                // Find command.
                int delimIndex = line.IndexOfAny(delim);
                string command, remainder;
                if (delimIndex == -1)
                {
                    command = line;
                    remainder = "";
                }
                else
                {
                    command = line.Substring(0, delimIndex);
                    remainder = line.Substring(delimIndex + 1);
                }

                // Try to match supported commands.
                bool knownHeader = true;
                switch (command)
                {
                    case "#GENRE":
                        track.trackMetadata.genre = remainder;
                        break;
                    case "#TITLE":
                        track.trackMetadata.title = remainder;
                        break;
                    case "#ARTIST":
                        track.trackMetadata.artist = remainder;
                        break;
                    case "#BPM":
                        pattern.patternMetadata.initBpm = double.Parse(remainder);
                        break;
                    case "#LNTYPE":
                        if (remainder == "1")
                        {
                            // Silently acknowledge.
                        }
                        else
                        {
                            lnTypeTwo = true;
                        }
                        break;
                    case "#LNOBJ":
                        longNoteCloser = remainder;
                        break;
                    default:
                        knownHeader = false;
                        break;
                }
                if (knownHeader) continue;

                // Sound file command: record index and filename.
                if (command.Length == 6 && (
                    Regex.IsMatch(command, @"#WAV[A-Z0-9][A-Z0-9]") ||
                    Regex.IsMatch(command, @"#OGG[A-Z0-9][A-Z0-9]")))
                {
                    string fileIndex = command.Substring(4, 2);
                    string filename = FindFile(remainder);
                    if (filename == null)
                    {
                        // When a file is not found (even after looking for alternative extensions),
                        // it's not a failure, but notes referencing this file will have their
                        // keysound erased, and these files will also be skipped when copying.
                        filename = "";
                    }
                    keysoundIndexToName.Add(fileIndex, filename);
                    continue;
                }

                // BMP command: record index and filename, but only for videos.
                if (command.Length == 6 && Regex.IsMatch(command, @"#BMP.."))
                {
                    string extension = Path.GetExtension(remainder);
                    if (extension == ".mp4" || extension == ".wmv")
                    {
                        string fileIndex = command.Substring(4, 2);
                        string filename = FindFile(remainder);
                        if (filename != null)
                        {
                            bmpIndexToName.Add(fileIndex, filename);
                        }
                    }
                    else
                    {
                        nonVideoBmpWarning = true;
                    }
                    continue;
                }

                // BPM command: record index and BPM.
                if (command.Length == 6 && Regex.IsMatch(command, @"#BPM.."))
                {
                    string index = command.Substring(4, 2);
                    double value = double.Parse(remainder);
                    bpmIndexToValue.Add(index, value);
                    continue;
                }

                // Measure and channel: handle supported channels.
                if (command.Length == 6 && Regex.IsMatch(command,
                    @"#[0-9][0-9][0-9][A-Z0-9][A-Z0-9]"))
                {
                    int measure = int.Parse(command.Substring(1, 3));
                    string channel = command.Substring(4, 2);

                    // Parse all indices in this measure+channel.
                    List<Tuple<int, string>> pulseToIndex = new List<Tuple<int, string>>();
                    int denom = remainder.Length / 2;
                    for (int num = 0; num < denom; num++)
                    {
                        string index = remainder.Substring(num * 2, 2);
                        if (index == "00") continue;
                        int pulse = measure * pulsesPerScan + num * pulsesPerScan / denom;
                        pulseToIndex.Add(new Tuple<int, string>(pulse, index));
                    }

                    if (channel == "01" || Regex.IsMatch(channel, @"[1-4]."))
                    {
                        ConvertNotes(pulseToIndex);
                    }
                    else if (channel == "02")
                    {
                        meterWarning = true;
                    }
                    else if (channel == "03")
                    {
                        ConvertInlineBpmEvents(pulseToIndex);
                    }
                    else if (channel == "04")
                    {
                        ConvertBmp(pulseToIndex, ref multipleBga);
                    }
                    else if (channel == "08")
                    {
                        ConvertIndexedBpmEvents(pulseToIndex);
                    }
                    else
                    {
                        ignoredChannels.Add(channel);
                    }

                    continue;
                }

                // Unknown command.
                ignoredCommands.Add(command);
            }

            // Calculate BGA start time, after all BPM events are recorded.
            if (pattern.patternMetadata.bga != "")
            {
                pattern.PrepareForTimeCalculation();
                pattern.patternMetadata.bgaOffset = pattern.PulseToTime(bgaStartPulse);
            }

            // Generate warning message.
            StringWriter writer = new StringWriter();
            if (ignoredCommands.Count > 0)
            {
                writer.WriteLine("The following commands are not supported, and will be ignored:");
                foreach (string c in ignoredCommands)
                {
                    writer.Write(c + ", ");
                }
                writer.WriteLine();
                writer.WriteLine();
            }
            if (ignoredChannels.Count > 0)
            {
                writer.WriteLine("The following channels are not supported, and will be ignored:");
                foreach (string c in ignoredChannels)
                {
                    writer.Write(c + ", ");
                }
                writer.WriteLine();
                writer.WriteLine();
            }
            if (meterWarning)
            {
                writer.WriteLine("Channel 02 is not supported; conversion will assume 4/4 meter.");
                writer.WriteLine();
                writer.WriteLine();
            }
            if (nonVideoBmpWarning)
            {
                writer.WriteLine("#BMP commands that do not refer to a video will be ignored.");
                writer.WriteLine();
                writer.WriteLine();
            }
            if (multipleBga)
            {
                writer.WriteLine("Found multiple notes in channel 04 that refer to videos. The 2nd and onward of these notes will be ignored.");
                writer.WriteLine();
                writer.WriteLine();
            }
            if (lnTypeTwo)
            {
                writer.WriteLine("#LNTYPE 2 is not supported. All notes in channels 5x and 6x will be ignored.");
                writer.WriteLine();
                writer.WriteLine();
            }
            report = writer.ToString();
            if (report == "")
            {
                report = "No problems found.";
            }

            return track.Serialize();
        }

        // Both input and output filenames are without path.
        // If not found, returns null.
        public string FindFile(string filenameWithoutPath)
        {
            List<string> acceptedFilenames = new List<string>();
            string originalExtension = Path.GetExtension(filenameWithoutPath);
            if (originalExtension == ".wav" ||
                originalExtension == ".ogg")
            {
                acceptedFilenames.Add(Path.ChangeExtension(filenameWithoutPath, ".wav"));
                acceptedFilenames.Add(Path.ChangeExtension(filenameWithoutPath, ".ogg"));
            }
            else
            {
                acceptedFilenames.Add(filenameWithoutPath);
            }

            foreach (string acceptedFilename in acceptedFilenames)
            {
                if (allFilenamesInBmsFolder.Contains(acceptedFilename.ToLower()))
                {
                    return acceptedFilename;
                }
            }
            return null;
        }

        private void ConvertNotes(List<Tuple<int, string>> pulseToKeysoundIndex)
        {
            // TODO: support long notes.
            foreach (Tuple<int, string> tuple in pulseToKeysoundIndex)
            {
                int pulse = tuple.Item1;
                string index = tuple.Item2;

                string filename = "";
                if (keysoundIndexToName.ContainsKey(index))
                {
                    filename = keysoundIndexToName[index];
                }

                // If needed, nudge pulse forward until there are <12 notes
                // at the current pulse.
                while (true)
                {
                    if (!numNotesAtPulse.ContainsKey(pulse))
                    {
                        numNotesAtPulse.Add(pulse, 0);
                    }
                    if (numNotesAtPulse[pulse] < maxLanes) break;
                    pulse++;
                }

                int lane = numNotesAtPulse[pulse];
                numNotesAtPulse[pulse]++;
                pattern.notes.Add(new Note()
                {
                    type = NoteType.Basic,
                    pulse = pulse,
                    lane = lane,
                    sound = filename
                });
            }
        }

        private void ConvertInlineBpmEvents(List<Tuple<int, string>> pulseToHexBpm)
        {
            foreach (Tuple<int, string> tuple in pulseToHexBpm)
            {
                int pulse = tuple.Item1;
                string hexBpm = tuple.Item2;
                int bpm = Convert.ToInt32(hexBpm, 16);
                pattern.bpmEvents.Add(new BpmEvent()
                {
                    pulse = pulse,
                    bpm = bpm
                });
            }
        }

        private void ConvertIndexedBpmEvents(List<Tuple<int, string>> pulseToBpmIndex)
        {
            foreach (Tuple<int, string> tuple in pulseToBpmIndex)
            {
                int pulse = tuple.Item1;
                string index = tuple.Item2;
                double bpm = bpmIndexToValue[index];
                pattern.bpmEvents.Add(new BpmEvent()
                {
                    pulse = pulse,
                    bpm = bpm
                });
            }
        }

        private void ConvertBmp(List<Tuple<int, string>> pulseToBmpIndex,
            ref bool multipleBga)
        {
            foreach (Tuple<int, string> tuple in pulseToBmpIndex)
            {
                int pulse = tuple.Item1;
                string index = tuple.Item2;
                if (!bmpIndexToName.ContainsKey(index)) continue;
                
                if (pattern.patternMetadata.bga != "")
                {
                    multipleBga = true;
                    return;  // No need to process the next notes.
                }
                else
                {
                    bgaStartPulse = pulse;
                    pattern.patternMetadata.bga = bmpIndexToName[index];
                }
            }
        }
    }
}
