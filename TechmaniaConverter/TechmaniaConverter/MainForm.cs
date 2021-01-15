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
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Tech Files (*.tech)|*.tech";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                techPathTextBox.Text = dialog.FileName;
            }
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            string bms = "", tech = "";
            try
            {
                bms = File.ReadAllText(bmsPathTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load BMS file:\n\n" + ex.Message);
                return;
            }

            if (File.Exists(techPathTextBox.Text))
            {
                DialogResult result = MessageBox.Show(
                    "The .tech file already exists, it will be overwritten. Continue?",
                    "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            Converter converter = new Converter();
            try
            {
                tech = converter.ConvertBmsToTech(bms);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during conversion:\n\n" + ex.Message);
                return;
            }

            string warning = converter.warningMessage;
            if (warning != null && warning != "")
            {
                DialogResult result = MessageBox.Show(
                    "Warning:\n\n" + warning + "\n\nContinue?", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            try
            {
                File.WriteAllText(techPathTextBox.Text, tech);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not write .tech file:\n\n" + ex.Message);
                return;
            }

            MessageBox.Show("Conversion successful.");
        }
    }
}
