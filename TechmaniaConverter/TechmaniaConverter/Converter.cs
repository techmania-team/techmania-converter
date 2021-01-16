using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TechmaniaConverter
{
    class Converter
    {
        public string report { get; private set; }
        public Converter()
        {
            report = null;
        }

        private Track track;
        private Pattern pattern;
        private string longNoteCloser;
        private const int bps = 4;
        private const int pulsesPerScan = Pattern.pulsesPerBeat * bps;
        private const int maxLanes = 12;
        private Dictionary<string, string> fileIndexToName;
        private Dictionary<int, int> numNotesAtPulse;

        public string ConvertBmsToTech(string bms)
        {
            StringReader reader = new StringReader(bms);

            track = new Track("", "");  // To be filled later
            pattern = new Pattern();
            pattern.patternMetadata.patternName = "Converted from BMS";
            pattern.patternMetadata.bps = bps;
            track.patterns.Add(pattern);
            char[] delim = { ' ', ':' };
            longNoteCloser = "";

            // Keysound table.
            fileIndexToName = new Dictionary<string, string>();
            numNotesAtPulse = new Dictionary<int, int>();

            // Error reporting.
            HashSet<string> ignoredCommands = new HashSet<string>();
            HashSet<string> ignoredChannels = new HashSet<string>();
            bool meterWarning = false;

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
                    string filename = remainder;
                    fileIndexToName.Add(fileIndex, filename);
                    continue;
                }

                // BMP command: record index and filename, but only for videos.
                if (command.Length == 6 && Regex.IsMatch(command, @"#BMP.."))
                {
                    // TODO: If video, don't ignore.
                    ignoredCommands.Add("#BMPxx");
                    continue;
                }

                // BPM command: record index and BPM.
                if (command.Length == 6 && Regex.IsMatch(command, @"#BPM.."))
                {
                    // TODO
                    ignoredCommands.Add(command);
                    continue;
                }

                // Measure and channel: handle supported channels.
                if (command.Length == 6 && Regex.IsMatch(command,
                    @"#[0-9][0-9][0-9][A-Z0-9][A-Z0-9]"))
                {
                    int measure = int.Parse(command.Substring(1, 3));
                    string channel = command.Substring(4, 2);
                    if (channel == "01" || Regex.IsMatch(channel, @"[1-4]."))
                    {
                        ConvertOneChannel(measure, remainder);
                    }
                    else if (channel == "02")
                    {
                        meterWarning = true;
                    }
                    else if (channel == "03")
                    {
                        // TODO: handle BPM event
                        ignoredChannels.Add(channel);
                    }
                    else if (channel == "04")
                    {
                        // TODO: handle BGA
                        ignoredChannels.Add(channel);
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

            // Generate warning message.
            StringWriter writer = new StringWriter();
            if (ignoredCommands.Count > 0)
            {
                writer.WriteLine("The following commands are unsupported, and therefore ignored:");
                foreach (string c in ignoredCommands)
                {
                    writer.Write(c + ", ");
                }
                writer.WriteLine();
                writer.WriteLine();
            }
            if (ignoredChannels.Count > 0)
            {
                writer.WriteLine("The following channels are unsupported, and therefore ignored:");
                foreach (string c in ignoredChannels)
                {
                    writer.Write(c + ", ");
                }
                writer.WriteLine();
                writer.WriteLine();
            }
            if (meterWarning)
            {
                writer.WriteLine("Channel 02 is unsupported; conversion will assume 4/4 meter.");
            }
            report = writer.ToString();
            if (report == "")
            {
                report = "No problems found.";
            }

            return track.Serialize();
        }

        private void ConvertOneChannel(int measure, string notes)
        {
            // TODO: support long notes.
            int denom = notes.Length / 2;
            for (int num = 0; num < denom; num++)
            {
                string fileIndex = notes.Substring(num * 2, 2);
                if (fileIndex == "00") continue;

                string filename = "";
                if (fileIndexToName.ContainsKey(fileIndex))
                {
                    filename = fileIndexToName[fileIndex];
                }

                int pulse = measure * pulsesPerScan + num * pulsesPerScan / denom;
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

        private void ConvertBpmEvent(int measure, string notes)
        {
            // TODO
        }
    }
}
