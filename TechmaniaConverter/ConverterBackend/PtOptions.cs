using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterBackend
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

    public class PtOptionsUtils
    {
        private static string GetOptionsFolder()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TECHMANIA Converter");
            Directory.CreateDirectory(folder);
            return folder;
        }

        private static string OptionsFilename()
        {
            return Path.Combine(GetOptionsFolder(), "PtOptions.json");
        }

        public static void SaveOptions()
        {
            string serialized = System.Text.Json.JsonSerializer.Serialize(PtOptions.instance,
                typeof(PtOptions),
                new System.Text.Json.JsonSerializerOptions()
                {
                    IncludeFields = true,
                    WriteIndented = true
                });
            File.WriteAllText(OptionsFilename(), serialized);
        }

        public static void LoadOrCreateOptions()
        {
            if (File.Exists(OptionsFilename()))
            {
                string serialized = File.ReadAllText(OptionsFilename());
                PtOptions.instance = System.Text.Json.JsonSerializer.Deserialize(serialized, typeof(PtOptions),
                    new System.Text.Json.JsonSerializerOptions()
                    {
                        IncludeFields = true
                    }) as PtOptions;
            }
            else
            {
                PtOptions.instance = new PtOptions();
            }
        }
    }
}
