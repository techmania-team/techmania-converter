using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterBackend
{
    public static class Utils
    {
        public static string GetTechFolder(string tracksFolder, TrackMetadata trackMetadata)
        {
            string filteredTitle = RemoveInvalidCharactersFromPath(trackMetadata.title);
            string filteredArtist = RemoveInvalidCharactersFromPath(trackMetadata.artist);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            return Path.Combine(tracksFolder, $"{filteredArtist} - {filteredTitle} - {timestamp}");
        }

        public static string RemoveInvalidCharactersFromPath(string input)
        {
            StringBuilder builder = new StringBuilder();
            const string invalidChars = "\\/*:?\"<>|";
            foreach (char c in input)
            {
                if (!invalidChars.Contains(c.ToString()))
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

        public static void ConvertVideo(string source, string dest)
        {
            if (source == dest) return;

            StringBuilder stderr = new StringBuilder();
            Process p = new Process();
            ProcessStartInfo startInfo = p.StartInfo;
            startInfo.FileName = "ffmpeg";
            startInfo.Arguments = $"-i \"{source}\" \"{dest}\"";
            startInfo.CreateNoWindow = false;
            startInfo.ErrorDialog = true;
            startInfo.UseShellExecute = false;
            // Uncomment this to receive stderr. However, ffmpeg writes its progress to stderr, so the ffmpeg window would be empty.
            // startInfo.RedirectStandardError = true;
            p.ErrorDataReceived += (sender, args) => stderr.AppendLine(args.Data);

            p.Start();
            if (p == null)
            {
                throw new Exception("Failed to start video converter.");
            }
            if (startInfo.RedirectStandardError)
            {
                p.BeginErrorReadLine();
            }
            p.WaitForExit();
            if (p.ExitCode != 0)
            {
                throw new Exception($"Video converter reports that it has failed.");
            }
        }

        // bmsPath: input .bms file.
        // tracksFolder: parent of output folder.
        // tech: the output track, serialized.
        // techFolder: output folder.
        // report: the report to show on UI.
        // filesToCopy: file pairs will be added to this list.
        //
        // May throw exceptions, which may contain an inner exception.
        public static void LoadAndConvertBms(string bmsPath, string tracksFolder, out string tech, out string techFolder, out string report, List<Tuple<string, string>> filesToCopy)
        {
            string bmsFolder = Path.GetDirectoryName(bmsPath);

            // Load .bms, and also enumerate all files, so the converter can look
            // for alternative extensions.
            string bms;
            string[] allFilesInBmsFolder;
            try
            {
                bms = File.ReadAllText(bmsPath);
                allFilesInBmsFolder = Directory.GetFiles(bmsFolder);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not load .bms.", ex);
            }

            BmsConverter converter = new BmsConverter();
            converter.allFilenamesInBmsFolder = new HashSet<string>();
            foreach (string file in allFilesInBmsFolder)
            {
                converter.allFilenamesInBmsFolder.Add(Path.GetFileName(file).ToLower());
            }

            try
            {
                converter.ConvertAndStore(bms);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred when parsing the .bms file.", ex);
            }

            tech = converter.Serialize();
            techFolder = GetTechFolder(tracksFolder, converter.track.trackMetadata);
            report = converter.GetReport();
            foreach (string file in converter.keysoundIndexToName.Values)
            {
                if (file == "") continue;
                filesToCopy.Add(new Tuple<string, string>(
                    Path.Combine(bmsFolder, file), Path.Combine(techFolder, file)));
            }
            foreach (string file in converter.bmpIndexToName.Values)
            {
                if (file == "") continue;
                filesToCopy.Add(new Tuple<string, string>(
                    Path.Combine(bmsFolder, file), Path.Combine(techFolder, file)));
            }
        }
    }
}
