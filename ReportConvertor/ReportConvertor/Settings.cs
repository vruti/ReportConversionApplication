using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportConverter
{
    public partial class Settings : Form
    {
        private Framework f;
        private Main m;
        private AppInfo a;

        public Settings(Framework fw, Main main, AppInfo aInfo)
        {
            InitializeComponent();
            f = fw;
            m = main;
            a = aInfo;
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            m.activateForm();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            dirLoc.Text = f.info.getFileLoc("Input");
            outputLoc.Text = f.info.getFileLoc("Output");
            archiveLoc.Text = f.info.getFileLoc("Archive");
            partsLoc.Text = f.info.getFileLoc("Parts");
            assetsLoc.Text = f.info.getFileLoc("Assets");
            pastLoc.Text = f.info.getFileLoc("WOHistory");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = folderBrowserDialog1.SelectedPath;
                a.changeFileLoc(fileName, "Input");
                dirLoc.Text = f.info.getFileLoc("Input");
            }
        }

        private void OutputButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                a.changeFileLoc(fileName, "Output");
                dirLoc.Text = f.info.getFileLoc("Output");
            }
        }

        private void ArchiveButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = folderBrowserDialog1.SelectedPath;
                a.changeFileLoc(fileName, "Archive");
                dirLoc.Text = f.info.getFileLoc("Archive");
            }
        }

        private void PartsButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                a.changeFileLoc(fileName, "Parts");
                dirLoc.Text = f.info.getFileLoc("Parts");
            }
        }

        private void AssetsButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                a.changeFileLoc(fileName, "Assets");
                dirLoc.Text = f.info.getFileLoc("Assets");
            }
        }

        private void PastWOButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                a.changeFileLoc(fileName, "WOHistory");
                dirLoc.Text = f.info.getFileLoc("WOHistory");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string file = a.getFileLoc("AppInfo");
            FileInfo fileI = new FileInfo(file);
            if (fileI.Exists)
            {
                System.Diagnostics.Process.Start(file);
            }
        }
    }
}
