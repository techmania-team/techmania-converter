
namespace TechmaniaConverter
{
    partial class PtOptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wikiLinkLabel = new System.Windows.Forms.LinkLabel();
            this.volumeExponentialRadioButton = new System.Windows.Forms.RadioButton();
            this.volumeLogarithmicRadioButton = new System.Windows.Forms.RadioButton();
            this.volumeParameterNameLabel = new System.Windows.Forms.Label();
            this.volumeParameterTextBox = new System.Windows.Forms.TextBox();
            this.panExponentialRadioButton = new System.Windows.Forms.RadioButton();
            this.panLogarithmicRadioButton = new System.Windows.Forms.RadioButton();
            this.panParameterNameLabel = new System.Windows.Forms.Label();
            this.panParameterTextBox = new System.Windows.Forms.TextBox();
            this.ignoreVolumeCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.loadScrollSpeedFromTrack18CheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pop4CheckBox = new System.Windows.Forms.CheckBox();
            this.pop3CheckBox = new System.Windows.Forms.CheckBox();
            this.pop2CheckBox = new System.Windows.Forms.CheckBox();
            this.pop1CheckBox = new System.Windows.Forms.CheckBox();
            this.star4CheckBox = new System.Windows.Forms.CheckBox();
            this.star3CheckBox = new System.Windows.Forms.CheckBox();
            this.star2CheckBox = new System.Windows.Forms.CheckBox();
            this.star1CheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // wikiLinkLabel
            // 
            this.wikiLinkLabel.AutoSize = true;
            this.wikiLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(13, 4);
            this.wikiLinkLabel.Location = new System.Drawing.Point(12, 9);
            this.wikiLinkLabel.Name = "wikiLinkLabel";
            this.wikiLinkLabel.Size = new System.Drawing.Size(233, 25);
            this.wikiLinkLabel.TabIndex = 1;
            this.wikiLinkLabel.TabStop = true;
            this.wikiLinkLabel.Text = "Refer to the wiki for explanations.";
            this.wikiLinkLabel.UseCompatibleTextRendering = true;
            this.wikiLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.wikiLinkLabel_LinkClicked);
            // 
            // volumeExponentialRadioButton
            // 
            this.volumeExponentialRadioButton.AutoSize = true;
            this.volumeExponentialRadioButton.Location = new System.Drawing.Point(27, 38);
            this.volumeExponentialRadioButton.Name = "volumeExponentialRadioButton";
            this.volumeExponentialRadioButton.Size = new System.Drawing.Size(108, 24);
            this.volumeExponentialRadioButton.TabIndex = 3;
            this.volumeExponentialRadioButton.TabStop = true;
            this.volumeExponentialRadioButton.Text = "Exponential";
            this.volumeExponentialRadioButton.UseVisualStyleBackColor = true;
            this.volumeExponentialRadioButton.CheckedChanged += new System.EventHandler(this.volumeExponentialRadioButton_CheckedChanged);
            // 
            // volumeLogarithmicRadioButton
            // 
            this.volumeLogarithmicRadioButton.AutoSize = true;
            this.volumeLogarithmicRadioButton.Location = new System.Drawing.Point(162, 38);
            this.volumeLogarithmicRadioButton.Name = "volumeLogarithmicRadioButton";
            this.volumeLogarithmicRadioButton.Size = new System.Drawing.Size(109, 24);
            this.volumeLogarithmicRadioButton.TabIndex = 4;
            this.volumeLogarithmicRadioButton.TabStop = true;
            this.volumeLogarithmicRadioButton.Text = "Logarithmic";
            this.volumeLogarithmicRadioButton.UseVisualStyleBackColor = true;
            this.volumeLogarithmicRadioButton.CheckedChanged += new System.EventHandler(this.volumeLogarithmicRadioButton_CheckedChanged);
            // 
            // volumeParameterNameLabel
            // 
            this.volumeParameterNameLabel.AutoSize = true;
            this.volumeParameterNameLabel.Location = new System.Drawing.Point(45, 95);
            this.volumeParameterNameLabel.Name = "volumeParameterNameLabel";
            this.volumeParameterNameLabel.Size = new System.Drawing.Size(74, 20);
            this.volumeParameterNameLabel.TabIndex = 5;
            this.volumeParameterNameLabel.Text = "Exponent:";
            // 
            // volumeParameterTextBox
            // 
            this.volumeParameterTextBox.Location = new System.Drawing.Point(146, 92);
            this.volumeParameterTextBox.Name = "volumeParameterTextBox";
            this.volumeParameterTextBox.Size = new System.Drawing.Size(125, 27);
            this.volumeParameterTextBox.TabIndex = 6;
            // 
            // panExponentialRadioButton
            // 
            this.panExponentialRadioButton.AutoSize = true;
            this.panExponentialRadioButton.Location = new System.Drawing.Point(27, 41);
            this.panExponentialRadioButton.Name = "panExponentialRadioButton";
            this.panExponentialRadioButton.Size = new System.Drawing.Size(108, 24);
            this.panExponentialRadioButton.TabIndex = 8;
            this.panExponentialRadioButton.TabStop = true;
            this.panExponentialRadioButton.Text = "Exponential";
            this.panExponentialRadioButton.UseVisualStyleBackColor = true;
            this.panExponentialRadioButton.CheckedChanged += new System.EventHandler(this.panExponentialRadioButton_CheckedChanged);
            // 
            // panLogarithmicRadioButton
            // 
            this.panLogarithmicRadioButton.AutoSize = true;
            this.panLogarithmicRadioButton.Location = new System.Drawing.Point(162, 41);
            this.panLogarithmicRadioButton.Name = "panLogarithmicRadioButton";
            this.panLogarithmicRadioButton.Size = new System.Drawing.Size(109, 24);
            this.panLogarithmicRadioButton.TabIndex = 9;
            this.panLogarithmicRadioButton.TabStop = true;
            this.panLogarithmicRadioButton.Text = "Logarithmic";
            this.panLogarithmicRadioButton.UseVisualStyleBackColor = true;
            this.panLogarithmicRadioButton.CheckedChanged += new System.EventHandler(this.panLogarithmicRadioButton_CheckedChanged);
            // 
            // panParameterNameLabel
            // 
            this.panParameterNameLabel.AutoSize = true;
            this.panParameterNameLabel.Location = new System.Drawing.Point(45, 99);
            this.panParameterNameLabel.Name = "panParameterNameLabel";
            this.panParameterNameLabel.Size = new System.Drawing.Size(74, 20);
            this.panParameterNameLabel.TabIndex = 10;
            this.panParameterNameLabel.Text = "Exponent:";
            // 
            // panParameterTextBox
            // 
            this.panParameterTextBox.Location = new System.Drawing.Point(146, 96);
            this.panParameterTextBox.Name = "panParameterTextBox";
            this.panParameterTextBox.Size = new System.Drawing.Size(125, 27);
            this.panParameterTextBox.TabIndex = 11;
            // 
            // ignoreVolumeCheckBox
            // 
            this.ignoreVolumeCheckBox.AutoSize = true;
            this.ignoreVolumeCheckBox.Location = new System.Drawing.Point(3, 338);
            this.ignoreVolumeCheckBox.Name = "ignoreVolumeCheckBox";
            this.ignoreVolumeCheckBox.Size = new System.Drawing.Size(167, 24);
            this.ignoreVolumeCheckBox.TabIndex = 12;
            this.ignoreVolumeCheckBox.Text = "Ignore volume notes";
            this.ignoreVolumeCheckBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(123, 477);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(94, 29);
            this.okButton.TabIndex = 13;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.volumeExponentialRadioButton);
            this.groupBox1.Controls.Add(this.volumeLogarithmicRadioButton);
            this.groupBox1.Controls.Add(this.volumeParameterNameLabel);
            this.groupBox1.Controls.Add(this.volumeParameterTextBox);
            this.groupBox1.Location = new System.Drawing.Point(3, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 136);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Volume curve";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panExponentialRadioButton);
            this.groupBox2.Controls.Add(this.panLogarithmicRadioButton);
            this.groupBox2.Controls.Add(this.panParameterNameLabel);
            this.groupBox2.Controls.Add(this.panParameterTextBox);
            this.groupBox2.Location = new System.Drawing.Point(3, 178);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 143);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Pan curve";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 519);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 40);
            this.label1.TabIndex = 16;
            this.label1.Text = "New options will apply the next time\r\nyou load .pt files.";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 37);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(324, 420);
            this.tabControl1.TabIndex = 17;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.ignoreVolumeCheckBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(316, 387);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Volume & pan";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.loadScrollSpeedFromTrack18CheckBox);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(316, 387);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Scroll speed";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 200);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(294, 60);
            this.label2.TabIndex = 2;
            this.label2.Text = "If scroll speed is not found by other means,\r\nthe ticked patterns will default to" +
    " 1 (8 BPS),\r\nunticked patterns will default to 2 (4 BPS).";
            // 
            // loadScrollSpeedFromTrack18CheckBox
            // 
            this.loadScrollSpeedFromTrack18CheckBox.AutoSize = true;
            this.loadScrollSpeedFromTrack18CheckBox.Location = new System.Drawing.Point(9, 323);
            this.loadScrollSpeedFromTrack18CheckBox.Name = "loadScrollSpeedFromTrack18CheckBox";
            this.loadScrollSpeedFromTrack18CheckBox.Size = new System.Drawing.Size(239, 24);
            this.loadScrollSpeedFromTrack18CheckBox.TabIndex = 1;
            this.loadScrollSpeedFromTrack18CheckBox.Text = "Load scroll speed from track 18";
            this.loadScrollSpeedFromTrack18CheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pop4CheckBox);
            this.groupBox3.Controls.Add(this.pop3CheckBox);
            this.groupBox3.Controls.Add(this.pop2CheckBox);
            this.groupBox3.Controls.Add(this.pop1CheckBox);
            this.groupBox3.Controls.Add(this.star4CheckBox);
            this.groupBox3.Controls.Add(this.star3CheckBox);
            this.groupBox3.Controls.Add(this.star2CheckBox);
            this.groupBox3.Controls.Add(this.star1CheckBox);
            this.groupBox3.Location = new System.Drawing.Point(3, 23);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(249, 170);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Scroll speed defaults to 1:";
            // 
            // pop4CheckBox
            // 
            this.pop4CheckBox.AutoSize = true;
            this.pop4CheckBox.Location = new System.Drawing.Point(125, 127);
            this.pop4CheckBox.Name = "pop4CheckBox";
            this.pop4CheckBox.Size = new System.Drawing.Size(68, 24);
            this.pop4CheckBox.TabIndex = 7;
            this.pop4CheckBox.Text = "Pop 4";
            this.pop4CheckBox.UseVisualStyleBackColor = true;
            // 
            // pop3CheckBox
            // 
            this.pop3CheckBox.AutoSize = true;
            this.pop3CheckBox.Location = new System.Drawing.Point(125, 97);
            this.pop3CheckBox.Name = "pop3CheckBox";
            this.pop3CheckBox.Size = new System.Drawing.Size(68, 24);
            this.pop3CheckBox.TabIndex = 6;
            this.pop3CheckBox.Text = "Pop 3";
            this.pop3CheckBox.UseVisualStyleBackColor = true;
            // 
            // pop2CheckBox
            // 
            this.pop2CheckBox.AutoSize = true;
            this.pop2CheckBox.Location = new System.Drawing.Point(125, 67);
            this.pop2CheckBox.Name = "pop2CheckBox";
            this.pop2CheckBox.Size = new System.Drawing.Size(68, 24);
            this.pop2CheckBox.TabIndex = 5;
            this.pop2CheckBox.Text = "Pop 2";
            this.pop2CheckBox.UseVisualStyleBackColor = true;
            // 
            // pop1CheckBox
            // 
            this.pop1CheckBox.AutoSize = true;
            this.pop1CheckBox.Location = new System.Drawing.Point(125, 37);
            this.pop1CheckBox.Name = "pop1CheckBox";
            this.pop1CheckBox.Size = new System.Drawing.Size(68, 24);
            this.pop1CheckBox.TabIndex = 4;
            this.pop1CheckBox.Text = "Pop 1";
            this.pop1CheckBox.UseVisualStyleBackColor = true;
            // 
            // star4CheckBox
            // 
            this.star4CheckBox.AutoSize = true;
            this.star4CheckBox.Location = new System.Drawing.Point(6, 127);
            this.star4CheckBox.Name = "star4CheckBox";
            this.star4CheckBox.Size = new System.Drawing.Size(69, 24);
            this.star4CheckBox.TabIndex = 3;
            this.star4CheckBox.Text = "Star 4";
            this.star4CheckBox.UseVisualStyleBackColor = true;
            // 
            // star3CheckBox
            // 
            this.star3CheckBox.AutoSize = true;
            this.star3CheckBox.Location = new System.Drawing.Point(6, 97);
            this.star3CheckBox.Name = "star3CheckBox";
            this.star3CheckBox.Size = new System.Drawing.Size(69, 24);
            this.star3CheckBox.TabIndex = 2;
            this.star3CheckBox.Text = "Star 3";
            this.star3CheckBox.UseVisualStyleBackColor = true;
            // 
            // star2CheckBox
            // 
            this.star2CheckBox.AutoSize = true;
            this.star2CheckBox.Location = new System.Drawing.Point(6, 67);
            this.star2CheckBox.Name = "star2CheckBox";
            this.star2CheckBox.Size = new System.Drawing.Size(69, 24);
            this.star2CheckBox.TabIndex = 1;
            this.star2CheckBox.Text = "Star 2";
            this.star2CheckBox.UseVisualStyleBackColor = true;
            // 
            // star1CheckBox
            // 
            this.star1CheckBox.AutoSize = true;
            this.star1CheckBox.Location = new System.Drawing.Point(6, 37);
            this.star1CheckBox.Name = "star1CheckBox";
            this.star1CheckBox.Size = new System.Drawing.Size(69, 24);
            this.star1CheckBox.TabIndex = 0;
            this.star1CheckBox.Text = "Star 1";
            this.star1CheckBox.UseVisualStyleBackColor = true;
            // 
            // PtOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 574);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.wikiLinkLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "PtOptionsForm";
            this.Text = "Pt Converter Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel wikiLinkLabel;
        private System.Windows.Forms.RadioButton volumeExponentialRadioButton;
        private System.Windows.Forms.RadioButton volumeLogarithmicRadioButton;
        private System.Windows.Forms.Label volumeParameterNameLabel;
        private System.Windows.Forms.TextBox volumeParameterTextBox;
        private System.Windows.Forms.RadioButton panExponentialRadioButton;
        private System.Windows.Forms.RadioButton panLogarithmicRadioButton;
        private System.Windows.Forms.Label panParameterNameLabel;
        private System.Windows.Forms.TextBox panParameterTextBox;
        private System.Windows.Forms.CheckBox ignoreVolumeCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox loadScrollSpeedFromTrack18CheckBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox pop4CheckBox;
        private System.Windows.Forms.CheckBox pop3CheckBox;
        private System.Windows.Forms.CheckBox pop2CheckBox;
        private System.Windows.Forms.CheckBox pop1CheckBox;
        private System.Windows.Forms.CheckBox star4CheckBox;
        private System.Windows.Forms.CheckBox star3CheckBox;
        private System.Windows.Forms.CheckBox star2CheckBox;
        private System.Windows.Forms.CheckBox star1CheckBox;
        private System.Windows.Forms.Label label2;
    }
}