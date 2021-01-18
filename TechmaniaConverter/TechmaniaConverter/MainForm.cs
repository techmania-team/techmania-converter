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

        private string bms;
        private string bmsPath;
        private string bmsFolder;
        private string tech;
        private string techFolder;
        private List<string> filesToCopy;
        private void loadButton_Click(object sender, EventArgs e)
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
                tech = converter.ConvertBmsToTech(bms);
            }
            catch (Exception ex)
            {
                reportTextBox.Text = "An error occurred when parsing .bms file:\r\n\r\n" + ex.Message;
                return;
            }

            filesToCopy = new List<string>(converter.keysoundIndexToName.Values);
            filesToCopy.AddRange(converter.bmpIndexToName.Values);
            reportTextBox.Text = converter.report;
            convertButton.Enabled = true;
        }

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
            string bmsFolder = Path.GetDirectoryName(bmsPath);
            if (bmsFolder != techFolder)
            {
                for (int i = 0; i < filesToCopy.Count; i++)
                {
                    string filename = filesToCopy[i];
                    if (filename == "") continue;
                    File.Copy(Path.Combine(bmsFolder, filename),
                        Path.Combine(techFolder, filename),
                        overwrite: true);
                    worker.ReportProgress(i * 100 / filesToCopy.Count);
                }
            }
            worker.ReportProgress(100);
        }
    }
}
