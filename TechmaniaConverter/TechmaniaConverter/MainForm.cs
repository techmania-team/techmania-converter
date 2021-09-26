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

namespace TechmaniaConverter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            PtOptionsForm.LoadOrCreateOptions();
            RefreshLoadButtons();
        }

        private void RefreshLoadButtons()
        {
            loadBmsButton.Enabled = tracksFolder != null;
            loadPtButton.Enabled = tracksFolder != null;
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
                reportTextBox.Text = "Could not load .bms:\r\n\r\n" + ex.ToString();
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
                converter.ConvertAndStore(bms);
            }
            catch (Exception ex)
            {
                reportTextBox.Text = "An error occurred when parsing .bms file:\r\n\r\n" + ex.ToString();
                return;
            }

            FindTechFolder(converter.track.trackMetadata);
            string report = $"Converted track will be written to:\r\n{techFolder}\r\n\r\n"
                + converter.GetReport();
            reportTextBox.Text = report;

            tech = converter.Serialize();
            filesToCopy = new List<Tuple<string, string>>();
            foreach (string file in converter.keysoundIndexToName.Values)
            {
                if (file == "") continue;
                filesToCopy.Add(new Tuple<string, string>(
                    Path.Combine(bmsFolder, file), Path.Combine(techFolder, file)));
            }
            foreach (string file in converter.bmpIndexToName.Values)
            {
                if (file == "") continue;
                filesToCopy.Add(new Tuple<string, string>(
                    Path.Combine(bmsFolder, file), Path.Combine(techFolder, file)));
            }
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
                // GetFiles is case insensitive.
                foreach (string file in Directory.EnumerateFiles(ptFolder, "*_star_?.pt"))
                {
                    fullPaths.Add(file.ToLower());
                }
                foreach (string file in Directory.EnumerateFiles(ptFolder, "*_pop_?.pt"))
                {
                    fullPaths.Add(file.ToLower());
                }
            }
            catch (Exception ex)
            {
                reportTextBox.Text = "Could not open the specified folder:\r\n\r\n" + ex.ToString();
                return;
            }

            if (fullPaths.Count == 0)
            {
                reportTextBox.Text = "Did not find any .pt file in the specified folder. Please select a folder containing .pt files.";
                return;
            }

            PtConverter converter = new PtConverter();
            try
            {
                // Extract the song's short name. This is the only guaranteed piece of metadata we can get.
                converter.ExtractShortNameAndInitialize(Path.GetFileName(fullPaths[0]));

                // Load and parse the pt files.
                foreach (string fullPath in fullPaths)
                {
                    string filename = Path.GetFileName(fullPath);
                    DJMaxEditor.DJMax.PlayerData parsedPt = PtLoader.Load(fullPath);
                    converter.ConvertAndAddPattern(filename, parsedPt);
                }

                // Search for any other metadata we can find.
                converter.SearchForMetadata(ptFolder);
            }
            catch (Exception ex)
            {
                reportTextBox.Text = "An error occurred when parsing .pt file:\r\n\r\n" + ex.ToString();
                return;
            }

            converter.GenerateReport();
            FindTechFolder(converter.track.trackMetadata);
            string report = $"Converted track will be written to:\r\n{techFolder}\r\n\r\n"
                + converter.GetReport();
            reportTextBox.Text = report;

            // Collect files to write, copy and/or convert.
            tech = converter.Serialize();
            filesToCopy = new List<Tuple<string, string>>();
            filesToConvert = new List<Tuple<string, string>>();
            foreach (string file in converter.allInstruments)
            {
                filesToCopy.Add(new Tuple<string, string>(
                    Path.Combine(ptFolder, file), Path.Combine(techFolder, file)));
            }
            if (converter.sourceDiscImagePath != null)
            {
                filesToCopy.Add(new Tuple<string, string>(converter.sourceDiscImagePath,
                    Path.Combine(techFolder, converter.track.trackMetadata.eyecatchImage)));
            }
            if (converter.sourceEyecatchPath != null)
            {
                filesToCopy.Add(new Tuple<string, string>(converter.sourceEyecatchPath,
                    Path.Combine(techFolder, converter.track.patterns[0].patternMetadata.backImage)));
            }
            if (converter.sourcePreviewPath != null)
            {
                filesToCopy.Add(new Tuple<string, string>(converter.sourcePreviewPath,
                    Path.Combine(techFolder, converter.track.trackMetadata.previewTrack)));
            }
            if (converter.sourceBgaPath != null)
            {
                Tuple<string, string> bgaTuple = new Tuple<string, string>(converter.sourceBgaPath,
                    Path.Combine(techFolder, converter.track.patterns[0].patternMetadata.bga));
                if (converter.bgaConversionRequired)
                {
                    filesToConvert.Add(bgaTuple);
                }
                else
                {
                    filesToCopy.Add(bgaTuple);
                }
            }
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

        private void FindTechFolder(TrackMetadata trackMetadata)
        {
            string filteredTitle = FilterString(trackMetadata.title);
            string filteredArtist = FilterString(trackMetadata.artist);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            techFolder = Path.Combine(tracksFolder, $"{filteredArtist} - {filteredTitle} - {timestamp}");
        }

        private static string FilterString(string input)
        {
            StringBuilder builder = new StringBuilder();
            const string invalidChars = "\\/*:?\"<>|";
            foreach (char c in input)
            {
                if (!invalidChars.Contains(c.ToString()))
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
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
                if (source != dest)
                {
                    StringBuilder stderr = new StringBuilder();
                    Process p = new Process();
                    ProcessStartInfo startInfo = p.StartInfo;
                    startInfo.FileName = "ffmpeg";
                    startInfo.Arguments = $"-i \"{source}\" \"{dest}\"";
                    startInfo.CreateNoWindow = false;
                    startInfo.ErrorDialog = true;
                    startInfo.UseShellExecute = false;
                    // Uncomment this to receive stderr. However, ffmpeg writes its progress to stderr, so the ffmpeg window would be empty.
                    // startInfo.RedirectStandardError = true;
                    p.ErrorDataReceived += (sender, args) => stderr.AppendLine(args.Data);

                    p.Start();
                    if (p == null)
                    {
                        throw new Exception("Failed to start video converter.");
                    }
                    if (startInfo.RedirectStandardError)
                    {
                        p.BeginErrorReadLine();
                    }
                    p.WaitForExit();
                    if (p.ExitCode != 0)
                    {
                        throw new Exception($"Video converter reports that it has failed.");
                    }
                }
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
