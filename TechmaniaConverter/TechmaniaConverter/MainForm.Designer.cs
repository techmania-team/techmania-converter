
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
            this.techPathTextBox = new System.Windows.Forms.TextBox();
            this.techBrowseButton = new System.Windows.Forms.Button();
            this.convertButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.reportTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.loadBmsButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.loadPtButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // techPathTextBox
            // 
            this.techPathTextBox.Location = new System.Drawing.Point(231, 398);
            this.techPathTextBox.Name = "techPathTextBox";
            this.techPathTextBox.Size = new System.Drawing.Size(377, 27);
            this.techPathTextBox.TabIndex = 3;
            // 
            // techBrowseButton
            // 
            this.techBrowseButton.Location = new System.Drawing.Point(614, 398);
            this.techBrowseButton.Name = "techBrowseButton";
            this.techBrowseButton.Size = new System.Drawing.Size(94, 29);
            this.techBrowseButton.TabIndex = 5;
            this.techBrowseButton.Text = "Browse...";
            this.techBrowseButton.UseVisualStyleBackColor = true;
            this.techBrowseButton.Click += new System.EventHandler(this.techBrowseButton_Click);
            // 
            // convertButton
            // 
            this.convertButton.Enabled = false;
            this.convertButton.Location = new System.Drawing.Point(231, 446);
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
            this.label3.Size = new System.Drawing.Size(54, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Step 1:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Step 2: Read report";
            // 
            // reportTextBox
            // 
            this.reportTextBox.Location = new System.Drawing.Point(231, 64);
            this.reportTextBox.Multiline = true;
            this.reportTextBox.Name = "reportTextBox";
            this.reportTextBox.ReadOnly = true;
            this.reportTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.reportTextBox.Size = new System.Drawing.Size(477, 252);
            this.reportTextBox.TabIndex = 9;
            this.reportTextBox.Text = "If anything in the input file(s) is not convertable to .tech, it will show up her" +
    "e.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 340);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(720, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "Step 3: If OK with the report, specify output folder. It should be a subfolder of" +
    " TECHMANIA\'s \"Tracks\" folder.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 450);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "Step 4:";
            // 
            // loadBmsButton
            // 
            this.loadBmsButton.Location = new System.Drawing.Point(231, 12);
            this.loadBmsButton.Name = "loadBmsButton";
            this.loadBmsButton.Size = new System.Drawing.Size(94, 29);
            this.loadBmsButton.TabIndex = 12;
            this.loadBmsButton.Text = "Load .bms...";
            this.loadBmsButton.UseVisualStyleBackColor = true;
            this.loadBmsButton.Click += new System.EventHandler(this.loadBmsButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(62, 360);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(313, 20);
            this.label6.TabIndex = 13;
            this.label6.Text = "Existing files in this folder may be overwritten.";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(350, 446);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(358, 29);
            this.progressBar.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(350, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 20);
            this.label1.TabIndex = 15;
            this.label1.Text = "or:";
            // 
            // loadPtButton
            // 
            this.loadPtButton.Location = new System.Drawing.Point(400, 12);
            this.loadPtButton.Name = "loadPtButton";
            this.loadPtButton.Size = new System.Drawing.Size(94, 29);
            this.loadPtButton.TabIndex = 16;
            this.loadPtButton.Text = "Load .pt...";
            this.loadPtButton.UseVisualStyleBackColor = true;
            this.loadPtButton.Click += new System.EventHandler(this.loadPtButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 493);
            this.Controls.Add(this.loadPtButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.loadBmsButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.reportTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.techBrowseButton);
            this.Controls.Add(this.techPathTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "TECHMANIA Converter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox techPathTextBox;
        private System.Windows.Forms.Button techBrowseButton;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox reportTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button loadBmsButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button loadPtButton;
    }
}

