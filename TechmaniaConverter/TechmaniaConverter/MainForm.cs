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

        #region Bms input
        private string bmsPath;
        private string bmsFolder;
        private void loadBmsButton_Click(object sender, EventArgs e)
        {
            convertButton.Enabled = false;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "BMS Files (*.bms;*.bme;*.bml;*.pms)|*.bms;*.bme;*.bml;*.pms|All Files (*.*)|*.*";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bmsPath = dialog.FileName;
                bmsFolder = Path.GetDirectoryName(bmsPath);
            }
            else
            {
                return;
            }

            // Load .bms, and also enumerate all files, so the converter can look
            // for alternative extensions.
            string bms;
            string[] allFilesInBmsFolder;
            try
            {
                bms = File.ReadAllText(bmsPath);
                allFilesInBmsFolder = Directory.GetFiles(bmsFolder);
            }
            catch (Exception ex)
            {
                reportTextBox.Text = "Could not load .bms:\r\n\r\n" + ex.Message;
                return;
            }

            BmsConverter converter = new BmsConverter();
            converter.allFilenamesInBmsFolder = new HashSet<string>();
            foreach (string file in allFilesInBmsFolder)
            {
                converter.allFilenamesInBmsFolder.Add(Path.GetFileName(file).ToLower());
            }

            try
            {
                tech = converter.ConvertToTech(bms);
            }
            catch (Exception ex)
            {
                reportTextBox.Text = "An error occurred when parsing .bms file:\r\n\r\n" + ex.Message;
                return;
            }

            sourceFolder = bmsFolder;
            filesToCopy = new List<string>(converter.keysoundIndexToName.Values);
            filesToCopy.AddRange(converter.bmpIndexToName.Values);
            reportTextBox.Text = converter.report;
            convertButton.Enabled = true;
        }
        #endregion

        #region Pt input
        private string ptFolder;
        private void loadPtButton_Click(object sender, EventArgs e)
        {
            convertButton.Enabled = false;

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

            // Look for all .pt files in this folder.
            List<string> fullPaths = new List<string>();
            try
            {
                fullPaths.AddRange(Directory.GetFiles(ptFolder, "*_star_?.pt"));
                fullPaths.AddRange(Directory.GetFiles(ptFolder, "*_pop_?.pt"));
            }
            catch (Exception ex)
            {
                reportTextBox.Text = "Could not open the specified folder:\r\n\r\n" + ex.Message;
                return;
            }

            if (fullPaths.Count == 0)
            {
                reportTextBox.Text = "Did not find any .pt file in the specified folder. Please select a folder containing .pt files.";
                return;
            }

            // Extract song ID. This sadly is the only piece of metadata we can get from pts.
            PtConverter converter = new PtConverter();
            converter.ExtractSongIdFrom(Path.GetFileName(fullPaths[0]));

            // Load and parse the pt files.
            try
            {
                foreach (string fullPath in fullPaths)
                {
                    string filename = Path.GetFileName(fullPath);
                    DJMaxEditor.DJMax.PlayerData parsedPt = PtLoader.Load(fullPath);
                    converter.ConvertAndAddPattern(filename, parsedPt);
                }
            }
            catch (Exception ex)
            {
                reportTextBox.Text = "An error occurred when parsing .pt file:\r\n\r\n" + ex.Message;
                return;
            }

            converter.GenerateReport();
            tech = converter.Serialize();
            sourceFolder = ptFolder;
            filesToCopy = new List<string>(converter.allInstruments);
            reportTextBox.Text = converter.report;
            convertButton.Enabled = true;
        }
        #endregion

        #region Output
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

        private string tech;
        private string sourceFolder;
        private string techFolder;
        private List<string> filesToCopy;

        private void convertButton_Click(object sender, EventArgs e)
        {
            techFolder = techPathTextBox.Text;
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
            if (sourceFolder != techFolder)
            {
                for (int i = 0; i < filesToCopy.Count; i++)
                {
                    string filename = filesToCopy[i];
                    if (filename == "") continue;
                    File.Copy(Path.Combine(sourceFolder, filename),
                        Path.Combine(techFolder, filename),
                        overwrite: true);
                    worker.ReportProgress(i * 100 / filesToCopy.Count);
                }
            }
            worker.ReportProgress(100);
        }
        #endregion
    }
}
