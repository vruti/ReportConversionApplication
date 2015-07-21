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
            this.button1 = new System.Windows.Forms.Button();
            this.dirLoc = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
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
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.dirLoc);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(314, 260);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "File Locations";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(200, 25);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(51, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Choose";
            this.button1.UseVisualStyleBackColor = true;
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
        private System.Windows.Forms.Button button1;
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


    }
}