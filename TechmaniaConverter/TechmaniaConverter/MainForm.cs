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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void bmsBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "BMS Files (*.bms;*.bme;*.bml;*.pms)|*.bms;*.bme;*.bml;*.pms|All Files (*.*)|*.*";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bmsPathTextBox.Text = dialog.FileName;
            }
        }

        private void techBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Save converted track to:";
            dialog.ShowNewFolderButton = true;
            dialog.UseDescriptionForTitle = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                techPathTextBox.Text = dialog.SelectedPath;
            }
        }

        string bms;
        string tech;
        private void loadButton_Click(object sender, EventArgs e)
        {
            convertButton.Enabled = false;
            try
            {
                bms = File.ReadAllText(bmsPathTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load .bms file:\n\n" + ex.Message);
                return;
            }

            Converter converter = new Converter();
            try
            {
                tech = converter.ConvertBmsToTech(bms);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred when generating report:\n\n" + ex.Message);
                return;
            }

            reportTextBox.Text = converter.report;
            convertButton.Enabled = true;
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            string techFilename = Path.Combine(techPathTextBox.Text, "track.tech");
            try
            {
                File.WriteAllText(techFilename, tech);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred when writing:\n\n" + ex.Message);
                return;
            }

            MessageBox.Show("Conversion successful.");
        }
    }
}
