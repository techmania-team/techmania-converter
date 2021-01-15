
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
            this.label1 = new System.Windows.Forms.Label();
            this.bmsPathTextBox = new System.Windows.Forms.TextBox();
            this.techPathTextBox = new System.Windows.Forms.TextBox();
            this.bmsBrowseButton = new System.Windows.Forms.Button();
            this.techBrowseButton = new System.Windows.Forms.Button();
            this.convertButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.reportTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.loadButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Step 1: Specify .bms file";
            // 
            // bmsPathTextBox
            // 
            this.bmsPathTextBox.Location = new System.Drawing.Point(231, 11);
            this.bmsPathTextBox.Name = "bmsPathTextBox";
            this.bmsPathTextBox.Size = new System.Drawing.Size(377, 27);
            this.bmsPathTextBox.TabIndex = 2;
            // 
            // techPathTextBox
            // 
            this.techPathTextBox.Location = new System.Drawing.Point(231, 445);
            this.techPathTextBox.Name = "techPathTextBox";
            this.techPathTextBox.Size = new System.Drawing.Size(377, 27);
            this.techPathTextBox.TabIndex = 3;
            // 
            // bmsBrowseButton
            // 
            this.bmsBrowseButton.Location = new System.Drawing.Point(614, 9);
            this.bmsBrowseButton.Name = "bmsBrowseButton";
            this.bmsBrowseButton.Size = new System.Drawing.Size(94, 29);
            this.bmsBrowseButton.TabIndex = 4;
            this.bmsBrowseButton.Text = "Browse...";
            this.bmsBrowseButton.UseVisualStyleBackColor = true;
            this.bmsBrowseButton.Click += new System.EventHandler(this.bmsBrowseButton_Click);
            // 
            // techBrowseButton
            // 
            this.techBrowseButton.Location = new System.Drawing.Point(614, 445);
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
            this.convertButton.Location = new System.Drawing.Point(231, 493);
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
            this.label3.Location = new System.Drawing.Point(12, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Step 2:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Step 3: Read report";
            // 
            // reportTextBox
            // 
            this.reportTextBox.Location = new System.Drawing.Point(231, 111);
            this.reportTextBox.Multiline = true;
            this.reportTextBox.Name = "reportTextBox";
            this.reportTextBox.ReadOnly = true;
            this.reportTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.reportTextBox.Size = new System.Drawing.Size(477, 252);
            this.reportTextBox.TabIndex = 9;
            this.reportTextBox.Text = "If anything in the .bms file is not convertable to .tech, it will show up here.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 387);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(710, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "Step 4: If OK with the report, specify Track folder. It should be a subfolder of " +
    "TECHMANIA\'s \"Tracks\" folder.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 497);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "Step 5:";
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(231, 59);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(94, 29);
            this.loadButton.TabIndex = 12;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(62, 407);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(313, 20);
            this.label6.TabIndex = 13;
            this.label6.Text = "Existing files in this folder may be overwritten.";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 555);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.reportTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.techBrowseButton);
            this.Controls.Add(this.bmsBrowseButton);
            this.Controls.Add(this.techPathTextBox);
            this.Controls.Add(this.bmsPathTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "TECHMANIA Converter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox bmsPathTextBox;
        private System.Windows.Forms.TextBox techPathTextBox;
        private System.Windows.Forms.Button bmsBrowseButton;
        private System.Windows.Forms.Button techBrowseButton;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox reportTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Label label6;
    }
}

