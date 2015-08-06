namespace ReportConverter
{
    partial class Main
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
            this.Start = new System.Windows.Forms.Button();
            this.Settings = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Archive = new System.Windows.Forms.Button();
            this.Quit = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(62, 249);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(75, 23);
            this.Start.TabIndex = 0;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.button1_Click);
            // 
            // Settings
            // 
            this.Settings.Location = new System.Drawing.Point(248, 249);
            this.Settings.Name = "Settings";
            this.Settings.Size = new System.Drawing.Size(75, 23);
            this.Settings.TabIndex = 1;
            this.Settings.Text = "Settings";
            this.Settings.UseVisualStyleBackColor = true;
            this.Settings.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(55, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(283, 39);
            this.label1.TabIndex = 2;
            this.label1.Text = "Report Converter";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Archive
            // 
            this.Archive.Location = new System.Drawing.Point(154, 249);
            this.Archive.Name = "Archive";
            this.Archive.Size = new System.Drawing.Size(75, 23);
            this.Archive.TabIndex = 3;
            this.Archive.Text = "Archive";
            this.Archive.UseVisualStyleBackColor = true;
            this.Archive.Click += new System.EventHandler(this.button3_Click);
            // 
            // Quit
            // 
            this.Quit.Location = new System.Drawing.Point(154, 311);
            this.Quit.Name = "Quit";
            this.Quit.Size = new System.Drawing.Size(75, 23);
            this.Quit.TabIndex = 4;
            this.Quit.Text = "Quit";
            this.Quit.UseVisualStyleBackColor = true;
            this.Quit.Click += new System.EventHandler(this.button4_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(143, 193);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 23);
            this.progressBar.TabIndex = 5;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 362);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.Quit);
            this.Controls.Add(this.Archive);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Settings);
            this.Controls.Add(this.Start);
            this.Name = "Main";
            this.Text = "Report Converter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Button Settings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Archive;
        private System.Windows.Forms.Button Quit;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}

