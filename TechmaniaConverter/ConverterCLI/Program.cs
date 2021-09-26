using System;

namespace ConverterCLI
{
    class Program
    {
        private static void PrintHelp()
        {
            Console.WriteLine(@"
TECHMANIA Converter

Usage: ConverterCLI.exe [--bmsInput=<path to .bms file> | --ptInput=<path .pt folder>] --out=<output folder> [--noReport] [--noConfirm]

This command-line tool is provided as a complement to the GUI converter (ConverterWinForm.exe), enabling you to write a Windows batch file to convert tracks in batches. We assume you are already familiar with the GUI version.

This tool doesn't support changing the .pt converter options. Use the GUI version for that.

Set exactly 1 of --bmsInput and --ptInput. The output will be written to a subfolder of --out.

Set --noReport to skip the report.

Set --noConfirm to skip the confirmation, even when there are issues.
");
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }
        }
    }
}
