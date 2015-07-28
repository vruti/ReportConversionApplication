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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pastLoc = new System.Windows.Forms.TextBox();
            this.assetsLoc = new System.Windows.Forms.TextBox();
            this.partsLoc = new System.Windows.Forms.TextBox();
            this.archiveLoc = new System.Windows.Forms.TextBox();
            this.outputLoc = new System.Windows.Forms.TextBox();
            this.InputFileButton = new System.Windows.Forms.Button();
            this.dirLoc = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.OutputButton = new System.Windows.Forms.Button();
            this.ArchiveButton = new System.Windows.Forms.Button();
            this.PartsButton = new System.Windows.Forms.Button();
            this.AssetsButton = new System.Windows.Forms.Button();
            this.PastWOButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(2, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(322, 286);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.PastWOButton);
            this.tabPage1.Controls.Add(this.AssetsButton);
            this.tabPage1.Controls.Add(this.PartsButton);
            this.tabPage1.Controls.Add(this.ArchiveButton);
            this.tabPage1.Controls.Add(this.OutputButton);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.pastLoc);
            this.tabPage1.Controls.Add(this.assetsLoc);
            this.tabPage1.Controls.Add(this.partsLoc);
            this.tabPage1.Controls.Add(this.archiveLoc);
            this.tabPage1.Controls.Add(this.outputLoc);
            this.tabPage1.Controls.Add(this.InputFileButton);
            this.tabPage1.Controls.Add(this.dirLoc);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(314, 260);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "File Locations";
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
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 170);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Assets File";
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Archive Directory";
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Input Directory";
            // 
            // pastLoc
            // 
            this.pastLoc.Location = new System.Drawing.Point(24, 225);
            this.pastLoc.Name = "pastLoc";
            this.pastLoc.Size = new System.Drawing.Size(160, 20);
            this.pastLoc.TabIndex = 6;
            // 
            // assetsLoc
            // 
            this.assetsLoc.Location = new System.Drawing.Point(24, 185);
            this.assetsLoc.Name = "assetsLoc";
            this.assetsLoc.Size = new System.Drawing.Size(160, 20);
            this.assetsLoc.TabIndex = 5;
            // 
            // partsLoc
            // 
            this.partsLoc.Location = new System.Drawing.Point(24, 145);
            this.partsLoc.Name = "partsLoc";
            this.partsLoc.Size = new System.Drawing.Size(160, 20);
            this.partsLoc.TabIndex = 4;
            // 
            // archiveLoc
            // 
            this.archiveLoc.Location = new System.Drawing.Point(24, 105);
            this.archiveLoc.Name = "archiveLoc";
            this.archiveLoc.Size = new System.Drawing.Size(160, 20);
            this.archiveLoc.TabIndex = 3;
            // 
            // outputLoc
            // 
            this.outputLoc.Location = new System.Drawing.Point(24, 65);
            this.outputLoc.Name = "outputLoc";
            this.outputLoc.Size = new System.Drawing.Size(160, 20);
            this.outputLoc.TabIndex = 2;
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
            // dirLoc
            // 
            this.dirLoc.Location = new System.Drawing.Point(24, 25);
            this.dirLoc.Name = "dirLoc";
            this.dirLoc.Size = new System.Drawing.Size(160, 20);
            this.dirLoc.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(314, 260);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Converters";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button InputFileButton;
        private System.Windows.Forms.TextBox dirLoc;
        private System.Windows.Forms.TextBox pastLoc;
        private System.Windows.Forms.TextBox assetsLoc;
        private System.Windows.Forms.TextBox partsLoc;
        private System.Windows.Forms.TextBox archiveLoc;
        private System.Windows.Forms.TextBox outputLoc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button ArchiveButton;
        private System.Windows.Forms.Button OutputButton;
        private System.Windows.Forms.Button AssetsButton;
        private System.Windows.Forms.Button PartsButton;
        private System.Windows.Forms.Button PastWOButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;


    }
}