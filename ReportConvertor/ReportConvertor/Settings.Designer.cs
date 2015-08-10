namespace ReportConverter
{
    partial class Settings
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.Fields = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.FileLocations = new System.Windows.Forms.TabPage();
            this.dirLoc = new System.Windows.Forms.TextBox();
            this.InputFileButton = new System.Windows.Forms.Button();
            this.outputLoc = new System.Windows.Forms.TextBox();
            this.archiveLoc = new System.Windows.Forms.TextBox();
            this.partsLoc = new System.Windows.Forms.TextBox();
            this.assetsLoc = new System.Windows.Forms.TextBox();
            this.pastLoc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.OutputButton = new System.Windows.Forms.Button();
            this.ArchiveButton = new System.Windows.Forms.Button();
            this.PartsButton = new System.Windows.Forms.Button();
            this.AssetsButton = new System.Windows.Forms.Button();
            this.PastWOButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Fields.SuspendLayout();
            this.FileLocations.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Fields
            // 
            this.Fields.BackColor = System.Drawing.SystemColors.Control;
            this.Fields.Controls.Add(this.button1);
            this.Fields.Controls.Add(this.label9);
            this.Fields.Controls.Add(this.label8);
            this.Fields.Controls.Add(this.label7);
            this.Fields.Location = new System.Drawing.Point(4, 22);
            this.Fields.Name = "Fields";
            this.Fields.Padding = new System.Windows.Forms.Padding(3);
            this.Fields.Size = new System.Drawing.Size(314, 260);
            this.Fields.TabIndex = 2;
            this.Fields.Text = "Fields";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(57, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(194, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Modify alternate field names by clicking ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(57, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(187, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "on the button below to open the Excel";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(98, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(108, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = " file that contain them";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(121, 98);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Open File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // FileLocations
            // 
            this.FileLocations.BackColor = System.Drawing.SystemColors.Control;
            this.FileLocations.Controls.Add(this.PastWOButton);
            this.FileLocations.Controls.Add(this.AssetsButton);
            this.FileLocations.Controls.Add(this.PartsButton);
            this.FileLocations.Controls.Add(this.ArchiveButton);
            this.FileLocations.Controls.Add(this.OutputButton);
            this.FileLocations.Controls.Add(this.label6);
            this.FileLocations.Controls.Add(this.label5);
            this.FileLocations.Controls.Add(this.label4);
            this.FileLocations.Controls.Add(this.label3);
            this.FileLocations.Controls.Add(this.label2);
            this.FileLocations.Controls.Add(this.label1);
            this.FileLocations.Controls.Add(this.pastLoc);
            this.FileLocations.Controls.Add(this.assetsLoc);
            this.FileLocations.Controls.Add(this.partsLoc);
            this.FileLocations.Controls.Add(this.archiveLoc);
            this.FileLocations.Controls.Add(this.outputLoc);
            this.FileLocations.Controls.Add(this.dirLoc);
            this.FileLocations.Controls.Add(this.InputFileButton);
            this.FileLocations.Location = new System.Drawing.Point(4, 22);
            this.FileLocations.Name = "FileLocations";
            this.FileLocations.Padding = new System.Windows.Forms.Padding(3);
            this.FileLocations.Size = new System.Drawing.Size(314, 260);
            this.FileLocations.TabIndex = 0;
            this.FileLocations.Text = "File Locations";
            // 
            // dirLoc
            // 
            this.dirLoc.Location = new System.Drawing.Point(24, 25);
            this.dirLoc.Name = "dirLoc";
            this.dirLoc.Size = new System.Drawing.Size(160, 20);
            this.dirLoc.TabIndex = 0;
            // 
            // InputFileButton
            // 
            this.InputFileButton.Location = new System.Drawing.Point(200, 25);
            this.InputFileButton.Name = "InputFileButton";
            this.InputFileButton.Size = new System.Drawing.Size(51, 23);
            this.InputFileButton.TabIndex = 1;
            this.InputFileButton.Text = "Choose";
            this.InputFileButton.UseVisualStyleBackColor = true;
            this.InputFileButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // outputLoc
            // 
            this.outputLoc.Location = new System.Drawing.Point(24, 65);
            this.outputLoc.Name = "outputLoc";
            this.outputLoc.Size = new System.Drawing.Size(160, 20);
            this.outputLoc.TabIndex = 2;
            // 
            // archiveLoc
            // 
            this.archiveLoc.Location = new System.Drawing.Point(24, 105);
            this.archiveLoc.Name = "archiveLoc";
            this.archiveLoc.Size = new System.Drawing.Size(160, 20);
            this.archiveLoc.TabIndex = 3;
            // 
            // partsLoc
            // 
            this.partsLoc.Location = new System.Drawing.Point(24, 145);
            this.partsLoc.Name = "partsLoc";
            this.partsLoc.Size = new System.Drawing.Size(160, 20);
            this.partsLoc.TabIndex = 4;
            // 
            // assetsLoc
            // 
            this.assetsLoc.Location = new System.Drawing.Point(24, 185);
            this.assetsLoc.Name = "assetsLoc";
            this.assetsLoc.Size = new System.Drawing.Size(160, 20);
            this.assetsLoc.TabIndex = 5;
            // 
            // pastLoc
            // 
            this.pastLoc.Location = new System.Drawing.Point(24, 225);
            this.pastLoc.Name = "pastLoc";
            this.pastLoc.Size = new System.Drawing.Size(160, 20);
            this.pastLoc.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Input Directory";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Output Directory";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Archive Directory";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Parts File";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 170);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Assets File";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 210);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Past Work Order File";
            // 
            // OutputButton
            // 
            this.OutputButton.Location = new System.Drawing.Point(200, 65);
            this.OutputButton.Name = "OutputButton";
            this.OutputButton.Size = new System.Drawing.Size(51, 23);
            this.OutputButton.TabIndex = 13;
            this.OutputButton.Text = "Choose";
            this.OutputButton.UseVisualStyleBackColor = true;
            this.OutputButton.Click += new System.EventHandler(this.OutputButton_Click);
            // 
            // ArchiveButton
            // 
            this.ArchiveButton.Location = new System.Drawing.Point(200, 105);
            this.ArchiveButton.Name = "ArchiveButton";
            this.ArchiveButton.Size = new System.Drawing.Size(51, 23);
            this.ArchiveButton.TabIndex = 14;
            this.ArchiveButton.Text = "Choose";
            this.ArchiveButton.UseVisualStyleBackColor = true;
            this.ArchiveButton.Click += new System.EventHandler(this.ArchiveButton_Click);
            // 
            // PartsButton
            // 
            this.PartsButton.Location = new System.Drawing.Point(200, 145);
            this.PartsButton.Name = "PartsButton";
            this.PartsButton.Size = new System.Drawing.Size(51, 23);
            this.PartsButton.TabIndex = 15;
            this.PartsButton.Text = "Choose";
            this.PartsButton.UseVisualStyleBackColor = true;
            this.PartsButton.Click += new System.EventHandler(this.PartsButton_Click);
            // 
            // AssetsButton
            // 
            this.AssetsButton.Location = new System.Drawing.Point(200, 185);
            this.AssetsButton.Name = "AssetsButton";
            this.AssetsButton.Size = new System.Drawing.Size(51, 23);
            this.AssetsButton.TabIndex = 16;
            this.AssetsButton.Text = "Choose";
            this.AssetsButton.UseVisualStyleBackColor = true;
            this.AssetsButton.Click += new System.EventHandler(this.AssetsButton_Click);
            // 
            // PastWOButton
            // 
            this.PastWOButton.Location = new System.Drawing.Point(200, 225);
            this.PastWOButton.Name = "PastWOButton";
            this.PastWOButton.Size = new System.Drawing.Size(51, 23);
            this.PastWOButton.TabIndex = 17;
            this.PastWOButton.Text = "Choose";
            this.PastWOButton.UseVisualStyleBackColor = true;
            this.PastWOButton.Click += new System.EventHandler(this.PastWOButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.FileLocations);
            this.tabControl1.Controls.Add(this.Fields);
            this.tabControl1.Location = new System.Drawing.Point(2, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(322, 286);
            this.tabControl1.TabIndex = 0;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 297);
            this.Controls.Add(this.tabControl1);
            this.Name = "Settings";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Settings_FormClosing);
            this.Load += new System.EventHandler(this.Settings_Load);
            this.Fields.ResumeLayout(false);
            this.Fields.PerformLayout();
            this.FileLocations.ResumeLayout(false);
            this.FileLocations.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabPage Fields;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage FileLocations;
        private System.Windows.Forms.Button PastWOButton;
        private System.Windows.Forms.Button AssetsButton;
        private System.Windows.Forms.Button PartsButton;
        private System.Windows.Forms.Button ArchiveButton;
        private System.Windows.Forms.Button OutputButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pastLoc;
        private System.Windows.Forms.TextBox assetsLoc;
        private System.Windows.Forms.TextBox partsLoc;
        private System.Windows.Forms.TextBox archiveLoc;
        private System.Windows.Forms.TextBox outputLoc;
        private System.Windows.Forms.TextBox dirLoc;
        private System.Windows.Forms.Button InputFileButton;
        private System.Windows.Forms.TabControl tabControl1;


    }
}