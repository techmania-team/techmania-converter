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

    public class PtOptions
    {
        public static PtOptions instance;

        public CurveType volumeCurve;
        public float volumeParam;  // Exponent for exponential curve, base for logarithmic curve
        public CurveType panCurve;
        public float panParam;
        public bool ignoreVolumeNotes;

        public PtOptions()
        {
            volumeCurve = CurveType.Exponential;
            volumeParam = 1.5f;
            panCurve = CurveType.Exponential;
            panParam = 0.25f;
            ignoreVolumeNotes = false;
        }
    }
}
