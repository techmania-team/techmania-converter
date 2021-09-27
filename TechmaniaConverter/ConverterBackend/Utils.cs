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
    }
}
