using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechmaniaConverter
{
    public enum VolumnPanCurveType
    {
        Exponential,
        Logarithmic
    }

    public class PtOptions
    {
        public static PtOptions instance;

        // Volume and pan

        public VolumnPanCurveType volumeCurve;
        public float volumeParam;  // Exponent for exponential curve, base for logarithmic curve
        public VolumnPanCurveType panCurve;
        public float panParam;
        public bool ignoreVolumeNotes;

        // Scroll speed

        public List<bool> scrollSpeedDefaultsToOneOnStar;
        public List<bool> scrollSpeedDefaultsToOneOnPop;
        public bool loadScrollSpeedFromTrack18;

        public PtOptions()
        {
            volumeCurve = VolumnPanCurveType.Exponential;
            volumeParam = 1.5f;
            panCurve = VolumnPanCurveType.Exponential;
            panParam = 0.25f;
            ignoreVolumeNotes = false;

            scrollSpeedDefaultsToOneOnStar = new List<bool>() { false, false, false, false };
            scrollSpeedDefaultsToOneOnPop = new List<bool>() { false, false, false, false };
            loadScrollSpeedFromTrack18 = false;
        }
    }
}
