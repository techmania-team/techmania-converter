
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
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.ignoreVolumeCheckBox.Location = new System.Drawing.Point(12, 398);
            this.ignoreVolumeCheckBox.Name = "ignoreVolumeCheckBox";
            this.ignoreVolumeCheckBox.Size = new System.Drawing.Size(167, 24);
            this.ignoreVolumeCheckBox.TabIndex = 12;
            this.ignoreVolumeCheckBox.Text = "Ignore volume notes";
            this.ignoreVolumeCheckBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(121, 448);
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
            this.groupBox1.Location = new System.Drawing.Point(12, 59);
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
            this.groupBox2.Location = new System.Drawing.Point(12, 226);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 143);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Pan curve";
            // 
            // PtOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 504);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.ignoreVolumeCheckBox);
            this.Controls.Add(this.wikiLinkLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "PtOptionsForm";
            this.Text = "Pt Converter Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
    }
}