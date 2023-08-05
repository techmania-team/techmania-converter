using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConverterBackend;

namespace ConverterWinForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            PtOptionsUtils.LoadOrCreateOptions();
            RefreshLoadButtons();
        }

        private void RefreshLoadButtons()
        {
            bool enabled = !string.IsNullOrEmpty(tracksFolder);
            loadBmsButton.Enabled = enabled;
            loadPtButton.Enabled = enabled;
        }

        #region Bms input
        private void loadBmsButton_Click(object sender, EventArgs e)
        {
            convertButton.Enabled = false;

            string bmsPath;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "BMS Files (*.bms;*.bme;*.bml;*.pms)|*.bms;*.bme;*.bml;*.pms|All Files (*.*)|*.*";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bmsPath = dialog.FileName;
            }
            else
            {
                return;
            }

            string report = "";
            filesToCopy = new List<Tuple<string, string>>();
            try
            {
                Utils.LoadAndConvertBms(bmsPath, tracksFolder, out tech, out techFolder, out report, filesToCopy);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    reportTextBox.Text = ex.Message + ex.InnerException.ToString();
                }
                else
                {
                    reportTextBox.Text = ex.ToString();
                }
                return;
            }

            reportTextBox.Text = $"Converted track will be written to:\r\n{techFolder}\r\n\r\n" + report;
            convertButton.Enabled = true;
        }
        #endregion

        #region Pt input
        private void loadPtButton_Click(object sender, EventArgs e)
        {
            convertButton.Enabled = false;

            string ptFolder;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select folder containing .pt files:";
            dialog.UseDescriptionForTitle = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ptFolder = dialog.SelectedPath;
            }
            else
            {
                return;
            }

            string report = "";
            filesToCopy = new List<Tuple<string, string>>();
            filesToConvert = new List<Tuple<string, string>>();
            try
            {
                Utils.LoadAndConvertPt(ptFolder, tracksFolder, out tech, out techFolder, out report, filesToCopy, filesToConvert);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    reportTextBox.Text = ex.Message + ex.InnerException.ToString();
                }
                else
                {
                    reportTextBox.Text = ex.ToString();
                }
                return;
            }

            reportTextBox.Text = $"Converted track will be written to:\r\n{techFolder}\r\n\r\n" + report;
            convertButton.Enabled = true;
        }
        #endregion

        #region Output
        private void trackFolderBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Specify output folder:";
            dialog.ShowNewFolderButton = true;
            dialog.UseDescriptionForTitle = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                trackFolderTextBox.Text = dialog.SelectedPath;
                tracksFolder = trackFolderTextBox.Text;
            }
            RefreshLoadButtons();
        }

        private void trackFolderTextBox_TextChanged(object sender, EventArgs e)
        {
            tracksFolder = trackFolderTextBox.Text;
            RefreshLoadButtons();
        }

        private string tech;
        private string tracksFolder;
        private string techFolder;
        private List<Tuple<string, string>> filesToCopy;  // Full paths.
        private List<Tuple<string, string>> filesToConvert;  // Full paths.

        private void convertButton_Click(object sender, EventArgs e)
        {
            convertButton.Enabled = false;
            progressBar.Value = 0;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += ConversionWork;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += ConversionProgressChanged;
            worker.RunWorkerCompleted += ConversionCompleted;
            worker.RunWorkerAsync();
        }

        private void ConversionCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("An error occurred during conversion:\n\n" + e.Error.Message);
            }
            else
            {
                MessageBox.Show("Conversion successful.");
            }

            convertButton.Enabled = true;
            progressBar.Value = 0;
        }

        private void ConversionProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void ConversionWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.ReportProgress(0);

            Directory.CreateDirectory(techFolder);
            File.WriteAllText(Path.Combine(techFolder, "track.tech"), tech);

            if (filesToCopy == null) filesToCopy = new List<Tuple<string, string>>();
            if (filesToConvert == null) filesToConvert = new List<Tuple<string, string>>();
            int numTasks = filesToCopy.Count + filesToConvert.Count;
            int tasksDone = 0;
            foreach (Tuple<string, string> pair in filesToCopy)
            {
                string source = pair.Item1;
                string dest = pair.Item2;
                if (source != dest)
                {
                    File.Copy(source, dest, overwrite: true);
                }
                tasksDone++;
                worker.ReportProgress(tasksDone * 100 / numTasks);
            }
            foreach (Tuple<string, string> pair in filesToConvert)
            {
                string source = pair.Item1;
                string dest = pair.Item2;
                Utils.ConvertVideo(source, dest);
                tasksDone++;
                worker.ReportProgress(tasksDone * 100 / numTasks);
            }
            worker.ReportProgress(100);
        }
        #endregion

        private void ptOptionsButton_Click(object sender, EventArgs e)
        {
            PtOptionsForm optionsForm = new PtOptionsForm();
            optionsForm.ShowDialog();
        }
    }
}
