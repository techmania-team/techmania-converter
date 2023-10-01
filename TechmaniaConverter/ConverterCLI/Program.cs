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
                PtOptionsUtils.LoadOrCreateOptions();
                Utils.LoadAndConvertPt(ptFolder, tracksFolder, out tech, out techFolder, out report, filesToCopy, filesToConvert);
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
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                    File.Copy(source, dest, overwrite: true);
                }
            }
            foreach (Tuple<string, string> pair in filesToConvert)
            {
                string source = pair.Item1;
                string dest = pair.Item2;
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                Utils.ConvertVideo(source, dest);
            }
        }
    }
}
