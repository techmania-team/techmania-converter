using System;
using System.Collections.Generic;
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

        public void ExtractSongIdFrom(string filename)
        {
            Match match = Regex.Match(filename, @"(.*)_(star|pop)_[1-4]\.pt");
            if (!match.Success)
            {
                throw new Exception($"Unrecognized file name: {filename}. {unrecognizedFilenameMessage}");
            }

            track = new Track(match.Groups[1].Value, "");
            allInstruments = new HashSet<string>();
        }

        public void ConvertAndAddPattern(string filename,
            DJMaxEditor.DJMax.PlayerData parsedPt)
        {
            Match match = Regex.Match(filename, @"(.*)_(star|pop)_([1-4])\.pt");
            if (!match.Success)
            {
                throw new Exception($"Unrecognized file name: {filename}. {unrecognizedFilenameMessage}");
            }

            StringBuilder patternNameBuilder = new StringBuilder();
            if (match.Groups[2].Value == "star")
            {
                patternNameBuilder.Append("Star");
            }
            else
            {
                patternNameBuilder.Append("Pop");
            }
            patternNameBuilder.Append(' ');
            // C# 8.0 Hyyyyyyyyype!
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
            track.patterns.Add(pattern);

            // TODO: the other stuff
        }

        public string Serialize()
        {
            return track.Serialize();
        }
    }
}
