
namespace ConverterWinForm
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
            trackFolderTextBox = new System.Windows.Forms.TextBox();
            trackFolderBrowseButton = new System.Windows.Forms.Button();
            convertButton = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            reportTextBox = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            loadBmsButton = new System.Windows.Forms.Button();
            progressBar = new System.Windows.Forms.ProgressBar();
            label1 = new System.Windows.Forms.Label();
            loadPtButton = new System.Windows.Forms.Button();
            ptOptionsButton = new System.Windows.Forms.Button();
            label6 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // trackFolderTextBox
            // 
            trackFolderTextBox.Location = new System.Drawing.Point(165, 50);
            trackFolderTextBox.Name = "trackFolderTextBox";
            trackFolderTextBox.Size = new System.Drawing.Size(377, 27);
            trackFolderTextBox.TabIndex = 3;
            trackFolderTextBox.TextChanged += trackFolderTextBox_TextChanged;
            // 
            // trackFolderBrowseButton
            // 
            trackFolderBrowseButton.Location = new System.Drawing.Point(559, 49);
            trackFolderBrowseButton.Name = "trackFolderBrowseButton";
            trackFolderBrowseButton.Size = new System.Drawing.Size(94, 29);
            trackFolderBrowseButton.TabIndex = 5;
            trackFolderBrowseButton.Text = "Browse...";
            trackFolderBrowseButton.UseVisualStyleBackColor = true;
            trackFolderBrowseButton.Click += trackFolderBrowseButton_Click;
            // 
            // convertButton
            // 
            convertButton.Enabled = false;
            convertButton.Location = new System.Drawing.Point(165, 482);
            convertButton.Name = "convertButton";
            convertButton.Size = new System.Drawing.Size(94, 29);
            convertButton.TabIndex = 6;
            convertButton.Text = "Convert";
            convertButton.UseVisualStyleBackColor = true;
            convertButton.Click += convertButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 16);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(585, 20);
            label3.TabIndex = 7;
            label3.Text = "Step 1: Specify output folder. It's usually TECHMANIA's Tracks folder or some subfolder.";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(13, 118);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(54, 20);
            label4.TabIndex = 8;
            label4.Text = "Step 2:";
            // 
            // reportTextBox
            // 
            reportTextBox.Location = new System.Drawing.Point(165, 175);
            reportTextBox.Multiline = true;
            reportTextBox.Name = "reportTextBox";
            reportTextBox.ReadOnly = true;
            reportTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            reportTextBox.Size = new System.Drawing.Size(488, 252);
            reportTextBox.TabIndex = 9;
            reportTextBox.Text = "If anything in the input file(s) is not convertable to .tech, it will show up here.";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(13, 178);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(140, 20);
            label5.TabIndex = 10;
            label5.Text = "Step 3: Read report.";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 450);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(196, 20);
            label2.TabIndex = 11;
            label2.Text = "Step 4: If OK with the report:";
            // 
            // loadBmsButton
            // 
            loadBmsButton.Location = new System.Drawing.Point(165, 114);
            loadBmsButton.Name = "loadBmsButton";
            loadBmsButton.Size = new System.Drawing.Size(94, 29);
            loadBmsButton.TabIndex = 12;
            loadBmsButton.Text = "Load .bms...";
            loadBmsButton.UseVisualStyleBackColor = true;
            loadBmsButton.Click += loadBmsButton_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new System.Drawing.Point(295, 482);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(358, 29);
            progressBar.TabIndex = 14;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(280, 118);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(26, 20);
            label1.TabIndex = 15;
            label1.Text = "or:";
            // 
            // loadPtButton
            // 
            loadPtButton.Location = new System.Drawing.Point(321, 114);
            loadPtButton.Name = "loadPtButton";
            loadPtButton.Size = new System.Drawing.Size(94, 29);
            loadPtButton.TabIndex = 16;
            loadPtButton.Text = "Load .pt...";
            loadPtButton.UseVisualStyleBackColor = true;
            loadPtButton.Click += loadPtButton_Click;
            // 
            // ptOptionsButton
            // 
            ptOptionsButton.BackgroundImage = (System.Drawing.Image)resources.GetObject("ptOptionsButton.BackgroundImage");
            ptOptionsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ptOptionsButton.Location = new System.Drawing.Point(421, 114);
            ptOptionsButton.Name = "ptOptionsButton";
            ptOptionsButton.Size = new System.Drawing.Size(30, 30);
            ptOptionsButton.TabIndex = 17;
            ptOptionsButton.UseVisualStyleBackColor = true;
            ptOptionsButton.Click += ptOptionsButton_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(13, 553);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(248, 20);
            label6.TabIndex = 18;
            label6.Text = "Restart from step 2 to convert more.";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(677, 597);
            Controls.Add(label6);
            Controls.Add(ptOptionsButton);
            Controls.Add(loadPtButton);
            Controls.Add(label1);
            Controls.Add(progressBar);
            Controls.Add(loadBmsButton);
            Controls.Add(label2);
            Controls.Add(label5);
            Controls.Add(reportTextBox);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(convertButton);
            Controls.Add(trackFolderBrowseButton);
            Controls.Add(trackFolderTextBox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "TECHMANIA Converter";
            ResumeLayout(false);
            PerformLayout();
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

