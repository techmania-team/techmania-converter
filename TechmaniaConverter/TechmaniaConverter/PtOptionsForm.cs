using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public PtOptionsForm()
        {
            InitializeComponent();

            // Setting Checked will trigger events and set the proper label texts.
            volumeExponentialRadioButton.Checked = PtOptions.volumeCurve == CurveType.Exponential;
            volumeLogarithmicRadioButton.Checked = PtOptions.volumeCurve == CurveType.Logarithmic;
            volumeParameterTextBox.Text = PtOptions.volumeParam.ToString();
            panExponentialRadioButton.Checked = PtOptions.panCurve == CurveType.Exponential;
            panLogarithmicRadioButton.Checked = PtOptions.panCurve == CurveType.Logarithmic;
            panParameterTextBox.Text = PtOptions.panParam.ToString();
            ignoreVolumeCheckBox.Checked = PtOptions.ignoreVolumeNotes;
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

            PtOptions.volumeCurve = volumeExponentialRadioButton.Checked ?
                CurveType.Exponential : CurveType.Logarithmic;
            PtOptions.volumeParam = volumeParam;
            PtOptions.panCurve = panExponentialRadioButton.Checked ?
                CurveType.Exponential : CurveType.Logarithmic;
            PtOptions.panParam = panParam;
            PtOptions.ignoreVolumeNotes = ignoreVolumeCheckBox.Checked;

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
