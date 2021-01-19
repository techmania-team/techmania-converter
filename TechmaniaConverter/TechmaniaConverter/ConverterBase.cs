using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechmaniaConverter
{
    abstract class ConverterBase
    {
        public string report { get; protected set; }

        protected Track track;
        protected int bgaStartPulse;
        protected const int bps = 4;
        protected const int pulsesPerScan = Pattern.pulsesPerBeat * bps;
        protected const int maxLanes = 64;
    }
}
