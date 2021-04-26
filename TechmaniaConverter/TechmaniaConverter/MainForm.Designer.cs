
namespace TechmaniaConverter
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.trackFolderTextBox = new System.Windows.Forms.TextBox();
            this.trackFolderBrowseButton = new System.Windows.Forms.Button();
            this.convertButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.reportTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.loadBmsButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.loadPtButton = new System.Windows.Forms.Button();
            this.ptOptionsButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // trackFolderTextBox
            // 
            this.trackFolderTextBox.Location = new System.Drawing.Point(165, 50);
            this.trackFolderTextBox.Name = "trackFolderTextBox";
            this.trackFolderTextBox.Size = new System.Drawing.Size(377, 27);
            this.trackFolderTextBox.TabIndex = 3;
            // 
            // trackFolderBrowseButton
            // 
            this.trackFolderBrowseButton.Location = new System.Drawing.Point(559, 49);
            this.trackFolderBrowseButton.Name = "trackFolderBrowseButton";
            this.trackFolderBrowseButton.Size = new System.Drawing.Size(94, 29);
            this.trackFolderBrowseButton.TabIndex = 5;
            this.trackFolderBrowseButton.Text = "Browse...";
            this.trackFolderBrowseButton.UseVisualStyleBackColor = true;
            this.trackFolderBrowseButton.Click += new System.EventHandler(this.trackFolderBrowseButton_Click);
            // 
            // convertButton
            // 
            this.convertButton.Enabled = false;
            this.convertButton.Location = new System.Drawing.Point(165, 482);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(94, 29);
            this.convertButton.TabIndex = 6;
            this.convertButton.Text = "Convert";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(585, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Step 1: Specify output folder. It\'s usually TECHMANIA\'s Tracks folder or some sub" +
    "folder.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Step 2:";
            // 
            // reportTextBox
            // 
            this.reportTextBox.Location = new System.Drawing.Point(165, 175);
            this.reportTextBox.Multiline = true;
            this.reportTextBox.Name = "reportTextBox";
            this.reportTextBox.ReadOnly = true;
            this.reportTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.reportTextBox.Size = new System.Drawing.Size(488, 252);
            this.reportTextBox.TabIndex = 9;
            this.reportTextBox.Text = "If anything in the input file(s) is not convertable to .tech, it will show up her" +
    "e.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(140, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "Step 3: Read report.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 450);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(196, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "Step 4: If OK with the report:";
            // 
            // loadBmsButton
            // 
            this.loadBmsButton.Location = new System.Drawing.Point(165, 114);
            this.loadBmsButton.Name = "loadBmsButton";
            this.loadBmsButton.Size = new System.Drawing.Size(94, 29);
            this.loadBmsButton.TabIndex = 12;
            this.loadBmsButton.Text = "Load .bms...";
            this.loadBmsButton.UseVisualStyleBackColor = true;
            this.loadBmsButton.Click += new System.EventHandler(this.loadBmsButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(295, 482);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(358, 29);
            this.progressBar.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(280, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 20);
            this.label1.TabIndex = 15;
            this.label1.Text = "or:";
            // 
            // loadPtButton
            // 
            this.loadPtButton.Location = new System.Drawing.Point(321, 114);
            this.loadPtButton.Name = "loadPtButton";
            this.loadPtButton.Size = new System.Drawing.Size(94, 29);
            this.loadPtButton.TabIndex = 16;
            this.loadPtButton.Text = "Load .pt...";
            this.loadPtButton.UseVisualStyleBackColor = true;
            this.loadPtButton.Click += new System.EventHandler(this.loadPtButton_Click);
            // 
            // ptOptionsButton
            // 
            this.ptOptionsButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ptOptionsButton.BackgroundImage")));
            this.ptOptionsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ptOptionsButton.Location = new System.Drawing.Point(421, 114);
            this.ptOptionsButton.Name = "ptOptionsButton";
            this.ptOptionsButton.Size = new System.Drawing.Size(30, 30);
            this.ptOptionsButton.TabIndex = 17;
            this.ptOptionsButton.UseVisualStyleBackColor = true;
            this.ptOptionsButton.Click += new System.EventHandler(this.ptOptionsButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 553);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(248, 20);
            this.label6.TabIndex = 18;
            this.label6.Text = "Restart from step 2 to convert more.";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 597);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ptOptionsButton);
            this.Controls.Add(this.loadPtButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.loadBmsButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.reportTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.trackFolderBrowseButton);
            this.Controls.Add(this.trackFolderTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "TECHMANIA Converter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox trackFolderTextBox;
        private System.Windows.Forms.Button trackFolderBrowseButton;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox reportTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button loadBmsButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button loadPtButton;
        private System.Windows.Forms.Button ptOptionsButton;
        private System.Windows.Forms.Label label6;
    }
}

