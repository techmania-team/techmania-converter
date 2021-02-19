using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechmaniaConverter
{
    abstract class ConverterBase
    {
        protected StringWriter reportWriter;
        public string GetReport()
        {
            string report = reportWriter.ToString();
            if (report == "") return "No problems found.";
            return report;
        }

        public Track track { get; protected set; }
        protected int bgaStartPulse;
        protected const int bps = Pattern.defaultBps;
        protected const int pulsesPerScan = Pattern.pulsesPerBeat * bps;
        protected const int maxLanes = 64;

        public string Serialize()
        {
            return track.Serialize(optimizeForSaving: true);
        }
    }
}
