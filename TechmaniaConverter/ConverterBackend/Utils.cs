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

        // Forces 30 fps, as iOS doesn't support weird frame rates (such as 10,000,000/333,333 fps) well.
        public static void ConvertVideo(string source, string dest)
        {
            if (source == dest) return;

            StringBuilder stderr = new StringBuilder();
            Process p = new Process();
            ProcessStartInfo startInfo = p.StartInfo;
            startInfo.FileName = "ffmpeg";
            startInfo.Arguments = $"-i \"{source}\" -r 30 \"{dest}\"";
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

        // ptFolder: input folder containing .pt files.
        // tracksFolder: parent of output folder.
        // tech: the output track, serialized.
        // techFolder: output folder.
        // report: the report to show on UI.
        // filesToCopy: file pairs will be added to this list.
        // filesToConvert: file pairs will be added to this list.
        //
        // May throw exceptions, which may contain an inner exception.
        public static void LoadAndConvertPt(string ptFolder, string tracksFolder, out string tech, out string techFolder, out string report, List<Tuple<string, string>> filesToCopy, List<Tuple<string, string>> filesToConvert)
        {
            // Look for all .pt files in the .pt folder.
            List<string> fullPaths = new List<string>();
            try
            {
                // GetFiles is case insensitive.
                foreach (string file in Directory.EnumerateFiles(ptFolder, "*_star_?.pt"))
                {
                    fullPaths.Add(file.ToLower());
                }
                foreach (string file in Directory.EnumerateFiles(ptFolder, "*_pop_?.pt"))
                {
                    fullPaths.Add(file.ToLower());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not open the .pt folder.", ex);
            }

            if (fullPaths.Count == 0)
            {
                throw new Exception("Did not find any .pt file in the .pt folder.");
            }

            PtConverter converter = new PtConverter();
            try
            {
                // Extract the song's short name. This is the only guaranteed piece of metadata we can get.
                converter.ExtractShortNameAndInitialize(Path.GetFileName(fullPaths[0]));

                // Load and parse the pt files.
                foreach (string fullPath in fullPaths)
                {
                    string filename = Path.GetFileName(fullPath);
                    DJMaxEditor.DJMax.PlayerData parsedPt = PtLoader.Load(fullPath);
                    converter.ConvertAndAddPattern(filename, parsedPt);
                }

                // Search for any other metadata we can find.
                converter.SearchForMetadata(ptFolder);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred when parsing .pt file.", ex);
            }

            tech = converter.Serialize();
            techFolder = GetTechFolder(tracksFolder, converter.track.trackMetadata);
            converter.GenerateReport();
            report = converter.GetReport();

            foreach (string file in converter.allInstruments)
            {
                filesToCopy.Add(new Tuple<string, string>(
                    Path.Combine(ptFolder, file), Path.Combine(techFolder, file)));
            }
            // Find the first disc image to use as TECHMANIA eyecatch.
            foreach (string path in converter.sourceDiscImagePaths)
            {
                if (path != null)
                {
                    filesToCopy.Add(new Tuple<string, string>(path,
                        Path.Combine(techFolder, converter.track.trackMetadata.eyecatchImage)));
                    break;
                }
            }
            // Also copy the disc images themselves.
            string[] difficultyNames = { "", "NM", "HD", "MX", "EX" };
            for (int i = 1; i <= 4; i++)
            {
                if (converter.sourceDiscImagePaths[i] != null)
                {
                    string source = converter.sourceDiscImagePaths[i];
                    string extension = Path.GetExtension(source);
                    filesToCopy.Add(new Tuple<string, string>(source,
                        Path.Combine(techFolder, $"t3_disc_{difficultyNames[i]}{extension}")));
                }
            }
            if (converter.sourceEyecatchPath != null)
            {
                filesToCopy.Add(new Tuple<string, string>(converter.sourceEyecatchPath,
                    Path.Combine(techFolder, converter.track.patterns[0].patternMetadata.backImage)));
            }
            if (converter.sourceMiniEyecatchPath != null)
            {
                string extension = Path.GetExtension(converter.sourceMiniEyecatchPath);
                filesToCopy.Add(new Tuple<string, string>(converter.sourceMiniEyecatchPath,
                    Path.Combine(techFolder, $"t3_eyecatch_mini{extension}")));
            }
            if (converter.sourcePreviewPath != null)
            {
                filesToCopy.Add(new Tuple<string, string>(converter.sourcePreviewPath,
                    Path.Combine(techFolder, converter.track.trackMetadata.previewTrack)));
            }
            if (converter.sourceBgaPath != null)
            {
                Tuple<string, string> bgaTuple = new Tuple<string, string>(converter.sourceBgaPath,
                    Path.Combine(techFolder, converter.track.patterns[0].patternMetadata.bga));
                if (converter.bgaConversionRequired)
                {
                    filesToConvert.Add(bgaTuple);
                }
                else
                {
                    filesToCopy.Add(bgaTuple);
                }
            }
            if (converter.sourceBgaPreviewPath != null)
            {
                filesToConvert.Add(new Tuple<string, string>(converter.sourceBgaPreviewPath,
                    Path.Combine(techFolder, converter.track.trackMetadata.previewBga)));
            }
        }
    }
}
