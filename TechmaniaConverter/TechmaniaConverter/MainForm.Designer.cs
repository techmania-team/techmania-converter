
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
            this.label2 = new System.Windows.Forms.Label();
            this.bmsPathTextBox = new System.Windows.Forms.TextBox();
            this.techPathTextBox = new System.Windows.Forms.TextBox();
            this.bmsBrowseButton = new System.Windows.Forms.Button();
            this.techBrowseButton = new System.Windows.Forms.Button();
            this.convertButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Open .bms at:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Save .tech to:";
            // 
            // bmsPathTextBox
            // 
            this.bmsPathTextBox.Location = new System.Drawing.Point(119, 11);
            this.bmsPathTextBox.Name = "bmsPathTextBox";
            this.bmsPathTextBox.Size = new System.Drawing.Size(300, 27);
            this.bmsPathTextBox.TabIndex = 2;
            // 
            // techPathTextBox
            // 
            this.techPathTextBox.Location = new System.Drawing.Point(119, 58);
            this.techPathTextBox.Name = "techPathTextBox";
            this.techPathTextBox.Size = new System.Drawing.Size(300, 27);
            this.techPathTextBox.TabIndex = 3;
            // 
            // bmsBrowseButton
            // 
            this.bmsBrowseButton.Location = new System.Drawing.Point(425, 10);
            this.bmsBrowseButton.Name = "bmsBrowseButton";
            this.bmsBrowseButton.Size = new System.Drawing.Size(94, 29);
            this.bmsBrowseButton.TabIndex = 4;
            this.bmsBrowseButton.Text = "Browse...";
            this.bmsBrowseButton.UseVisualStyleBackColor = true;
            this.bmsBrowseButton.Click += new System.EventHandler(this.bmsBrowseButton_Click);
            // 
            // techBrowseButton
            // 
            this.techBrowseButton.Location = new System.Drawing.Point(425, 57);
            this.techBrowseButton.Name = "techBrowseButton";
            this.techBrowseButton.Size = new System.Drawing.Size(94, 29);
            this.techBrowseButton.TabIndex = 5;
            this.techBrowseButton.Text = "Browse...";
            this.techBrowseButton.UseVisualStyleBackColor = true;
            this.techBrowseButton.Click += new System.EventHandler(this.techBrowseButton_Click);
            // 
            // convertButton
            // 
            this.convertButton.Location = new System.Drawing.Point(217, 105);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(94, 29);
            this.convertButton.TabIndex = 6;
            this.convertButton.Text = "Convert";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 147);
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.techBrowseButton);
            this.Controls.Add(this.bmsBrowseButton);
            this.Controls.Add(this.techPathTextBox);
            this.Controls.Add(this.bmsPathTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "BMS To Tech Converter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox bmsPathTextBox;
        private System.Windows.Forms.TextBox techPathTextBox;
        private System.Windows.Forms.Button bmsBrowseButton;
        private System.Windows.Forms.Button techBrowseButton;
        private System.Windows.Forms.Button convertButton;
    }
}

