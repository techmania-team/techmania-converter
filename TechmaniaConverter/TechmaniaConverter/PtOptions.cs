using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechmaniaConverter
{
    public enum CurveType
    {
        Exponential,
        Logarithmic
    }

    static class PtOptions
    {
        public static CurveType volumeCurve;
        public static float volumeParam;  // Exponent for exponential curve, base for logarithmic curve
        public static CurveType panCurve;
        public static float panParam;
        public static bool ignoreVolumeNotes;

        static PtOptions()
        {
            volumeCurve = CurveType.Exponential;
            volumeParam = 1.5f;
            panCurve = CurveType.Exponential;
            panParam = 0.25f;
            ignoreVolumeNotes = false;
        }
    }
}
