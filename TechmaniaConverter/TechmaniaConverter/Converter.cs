﻿using System;
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

        public Dictionary<string, string> keysoundIndexToName { get; private set; }
        private Dictionary<string, double> bpmIndexToValue;
        public Dictionary<string, string> bmpIndexToName { get; private set; }

        // When a file is not found (even after looking for alternative extensions),
        // it's not a failure, but notes referencing this file will have their
        // keysound erased, and these files will also be skipped when copying.
        public HashSet<string> filesNotFoundInBmsFolder { get; private set; }
        // This includes full paths.
        public string[] allFilesInBmsFolder;

        private Dictionary<int, int> numNotesAtPulse;

        public string ConvertBmsToTech(string bms)
        {
            StringReader reader = new StringReader(bms);

            track = new Track("", "");  // To be filled later
            pattern = new Pattern();
            pattern.patternMetadata.patternName = "Converted from BMS";
            pattern.patternMetadata.controlScheme = ControlScheme.Keys;
            pattern.patternMetadata.bps = bps;
            track.patterns.Add(pattern);
            char[] delim = { ' ', ':' };
            longNoteCloser = "";

            // Lookup tables.
            keysoundIndexToName = new Dictionary<string, string>();
            filesNotFoundInBmsFolder = new HashSet<string>();
            bpmIndexToValue = new Dictionary<string, double>();
            bmpIndexToName = new Dictionary<string, string>();
            numNotesAtPulse = new Dictionary<int, int>();

            // Error reporting.
            HashSet<string> ignoredCommands = new HashSet<string>();
            HashSet<string> ignoredChannels = new HashSet<string>();
            bool meterWarning = false;
            bool nonVideoBmpWarning = false;

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
                    string filename = FindFile(remainder);
                    if (filename == null)
                    {
                        filesNotFoundInBmsFolder.Add(remainder);
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
                        if (filename == null)
                        {
                            filesNotFoundInBmsFolder.Add(remainder);
                        }
                        else
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
                    Dictionary<int, string> pulseToIndex = new Dictionary<int, string>();
                    int denom = remainder.Length / 2;
                    for (int num = 0; num < denom; num++)
                    {
                        string index = remainder.Substring(num * 2, 2);
                        if (index == "00") continue;
                        int pulse = measure * pulsesPerScan + num * pulsesPerScan / denom;
                        pulseToIndex.Add(pulse, index);
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
                        // TODO: handle BGA
                        ignoredChannels.Add(channel);
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

            // Generate warning message.
            StringWriter writer = new StringWriter();
            if (ignoredCommands.Count > 0)
            {
                writer.WriteLine("The following commands are unsupported, and will be ignored:");
                foreach (string c in ignoredCommands)
                {
                    writer.Write(c + ", ");
                }
                writer.WriteLine();
                writer.WriteLine();
            }
            if (ignoredChannels.Count > 0)
            {
                writer.WriteLine("The following channels are unsupported, and will be ignored:");
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
                writer.WriteLine();
                writer.WriteLine();
            }
            if (nonVideoBmpWarning)
            {
                writer.WriteLine("#BMP commands that do not refer to a video will be ignored.");
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
            HashSet<string> acceptedFilenames = new HashSet<string>();
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

            foreach (string fileInBmsFolder in allFilesInBmsFolder)
            {
                string filename = Path.GetFileName(fileInBmsFolder);
                if (acceptedFilenames.Contains(filename))
                {
                    return filename;
                }
            }
            return null;
        }

        private void ConvertNotes(Dictionary<int, string> pulseToKeysoundIndex)
        {
            // TODO: support long notes.
            foreach (KeyValuePair<int, string> pair in pulseToKeysoundIndex)
            {
                int pulse = pair.Key;
                string index = pair.Value;

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

        private void ConvertInlineBpmEvents(Dictionary<int, string> pulseToHexBpm)
        {
            foreach (KeyValuePair<int, string> pair in pulseToHexBpm)
            {
                int pulse = pair.Key;
                string hexBpm = pair.Value;
                int bpm = Convert.ToInt32(hexBpm, 16);
                pattern.bpmEvents.Add(new BpmEvent()
                {
                    pulse = pulse,
                    bpm = bpm
                });
            }
        }

        private void ConvertIndexedBpmEvents(Dictionary<int, string> pulseToBpmIndex)
        {
            foreach (KeyValuePair<int, string> pair in pulseToBpmIndex)
            {
                int pulse = pair.Key;
                string index = pair.Value;
                double bpm = bpmIndexToValue[index];
                pattern.bpmEvents.Add(new BpmEvent()
                {
                    pulse = pulse,
                    bpm = bpm
                });
            }
        }
    }
}
