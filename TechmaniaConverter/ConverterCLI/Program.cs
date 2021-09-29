using ConverterBackend;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConverterCLI
{
    class Program
    {
        private static void PrintHelp()
        {
            Console.WriteLine(@"
TECHMANIA Converter

Usage: ConverterCLI.exe [--bmsPath=<path to .bms file> | --ptFolder=<path .pt folder>] --tracksFolder=<output folder> [--silent]

This command-line tool is provided as a complement to the GUI converter (ConverterWinForm.exe), enabling you to write a Windows batch file to convert tracks in batches. We assume you are already familiar with the GUI version.

This tool doesn't support changing the .pt converter options. Use the GUI version for that.

Set exactly 1 of --bmsPath and --ptFolder. The output will be written to a subfolder of --tracksFolder.

Set --silent to operate in silent mode. The tool won't print the report, won't ask for confirmation, and won't print the conversion result.
");
        }

        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return 1;
            }

            string bmsPath = null, ptFolder = null, tracksFolder = null;
            bool silent = false;
            foreach (string a in args)
            {
                if (a.StartsWith("--bmsPath="))
                {
                    bmsPath = a.Substring("--bmsPath=".Length);
                }
                else if (a.StartsWith("--ptFolder="))
                {
                    ptFolder = a.Substring("--ptFolder=".Length);
                }
                else if (a.StartsWith("--tracksFolder="))
                {
                    tracksFolder = a.Substring("--tracksFolder=".Length);
                }
                else if (a == "--silent")
                {
                    silent = true;
                }
                else
                {
                    Console.WriteLine($"Unrecognized flag: " + a);
                    return 1;
                }
            }

            if (bmsPath == null && ptFolder == null)
            {
                Console.WriteLine("Error: missing input. Set one of --bmsPath or --ptFolder.");
                return 1;
            }
            if (bmsPath != null && ptFolder != null)
            {
                Console.WriteLine("Error: --bmsPath and --ptFolder can't both be set.");
            }
            if (tracksFolder == null)
            {
                Console.WriteLine("Error: missing --tracksFolder.");
                return 1;
            }

            Program p = new Program();
            p.DoWork(bmsPath, ptFolder, tracksFolder, silent);

            return 0;
        }

        private void DoWork(string bmsPath, string ptFolder, string tracksFolder, bool silent)
        {
            string tech, techFolder, report;
            List<Tuple<string, string>> filesToCopy = new List<Tuple<string, string>>();  // Full paths.
            List<Tuple<string, string>> filesToConvert = new List<Tuple<string, string>>();  // Full paths.
            if (bmsPath != null)
            {
                Utils.LoadAndConvertBms(bmsPath, tracksFolder, out tech, out techFolder, out report, filesToCopy);
            }
            else
            {
                LoadAndConvertPt(ptFolder, tracksFolder, out tech, out techFolder, out report, filesToCopy, filesToConvert);
            }

            if (!silent)
            {
                Console.WriteLine("Converted track will be written to:");
                Console.WriteLine(techFolder);
                Console.WriteLine();
                Console.WriteLine(report);
                Console.WriteLine();
                Console.WriteLine("Continue with conversion? (Y/n)");
                string confirmation = Console.ReadLine();
                if (!confirmation.ToLower().StartsWith('y'))
                {
                    Console.WriteLine("Cancelled.");
                    return;
                }
            }

            WriteOutput(tech, techFolder, filesToCopy, filesToConvert);
            if (!silent)
            {
                Console.WriteLine("Conversion successful.");
            }
        }

        private void LoadAndConvertPt(string ptFolder, string tracksFolder, out string tech, out string techFolder, out string report, List<Tuple<string, string>> filesToCopy, List<Tuple<string, string>> filesToConvert)
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
                throw new Exception("Could not open --ptFolder.", ex);
            }

            if (fullPaths.Count == 0)
            {
                throw new Exception("Did not find any .pt file in --ptFolder.");
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
            techFolder = Utils.GetTechFolder(tracksFolder, converter.track.trackMetadata);
            converter.GenerateReport();
            report = converter.GetReport();

            foreach (string file in converter.allInstruments)
            {
                filesToCopy.Add(new Tuple<string, string>(
                    Path.Combine(ptFolder, file), Path.Combine(techFolder, file)));
            }
            if (converter.sourceDiscImagePath != null)
            {
                filesToCopy.Add(new Tuple<string, string>(converter.sourceDiscImagePath,
                    Path.Combine(techFolder, converter.track.trackMetadata.eyecatchImage)));
            }
            if (converter.sourceEyecatchPath != null)
            {
                filesToCopy.Add(new Tuple<string, string>(converter.sourceEyecatchPath,
                    Path.Combine(techFolder, converter.track.patterns[0].patternMetadata.backImage)));
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
        }

        private void WriteOutput(string tech, string techFolder, List<Tuple<string, string>> filesToCopy, List<Tuple<string, string>> filesToConvert)
        {
            Directory.CreateDirectory(techFolder);
            File.WriteAllText(Path.Combine(techFolder, "track.tech"), tech);

            foreach (Tuple<string, string> pair in filesToCopy)
            {
                string source = pair.Item1;
                string dest = pair.Item2;
                if (source != dest)
                {
                    File.Copy(source, dest, overwrite: true);
                }
            }
            foreach (Tuple<string, string> pair in filesToConvert)
            {
                string source = pair.Item1;
                string dest = pair.Item2;
                Utils.ConvertVideo(source, dest);
            }
        }
    }
}
