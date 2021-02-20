using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechmaniaConverter
{
    public partial class PtOptionsForm : Form
    {
        private const string exponentLabel = "Exponent:";
        private const string baseLabel = "Base:";

        #region Save and load
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

        private static void SaveOptions()
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
        #endregion

        public PtOptionsForm()
        {
            InitializeComponent();

            // Initialize UI.
            // Setting Checked will trigger events and set the proper label texts.
            volumeExponentialRadioButton.Checked = PtOptions.instance.volumeCurve == CurveType.Exponential;
            volumeLogarithmicRadioButton.Checked = PtOptions.instance.volumeCurve == CurveType.Logarithmic;
            volumeParameterTextBox.Text = PtOptions.instance.volumeParam.ToString();
            panExponentialRadioButton.Checked = PtOptions.instance.panCurve == CurveType.Exponential;
            panLogarithmicRadioButton.Checked = PtOptions.instance.panCurve == CurveType.Logarithmic;
            panParameterTextBox.Text = PtOptions.instance.panParam.ToString();
            ignoreVolumeCheckBox.Checked = PtOptions.instance.ignoreVolumeNotes;
        }

        private void wikiLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "https://github.com/techmania-team/techmania-converter/wiki/.pt-converter-options");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            float volumeParam = 0f, panParam = 0f;
            try
            {
                volumeParam = float.Parse(volumeParameterTextBox.Text);
                panParam = float.Parse(panParameterTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid parameter: " + ex.Message);
                return;
            }

            PtOptions.instance.volumeCurve = volumeExponentialRadioButton.Checked ?
                CurveType.Exponential : CurveType.Logarithmic;
            PtOptions.instance.volumeParam = volumeParam;
            PtOptions.instance.panCurve = panExponentialRadioButton.Checked ?
                CurveType.Exponential : CurveType.Logarithmic;
            PtOptions.instance.panParam = panParam;
            PtOptions.instance.ignoreVolumeNotes = ignoreVolumeCheckBox.Checked;

            SaveOptions();
            Close();
        }

        private void volumeExponentialRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            volumeParameterNameLabel.Text = exponentLabel;
        }

        private void volumeLogarithmicRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            volumeParameterNameLabel.Text = baseLabel;
        }

        private void panExponentialRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            panParameterNameLabel.Text = exponentLabel;
        }

        private void panLogarithmicRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            panParameterNameLabel.Text = baseLabel;
        }
    }
}
