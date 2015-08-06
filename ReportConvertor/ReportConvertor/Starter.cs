using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReportConverter
{
    public partial class Starter : Form
    {
        private string filePath;

        public Starter()
        {
            InitializeComponent();
        }

        private void InputFileButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                fileLoc.Text = filePath;
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            AppInfo appInfo = new AppInfo(filePath);
            Framework framework = new Framework(appInfo);
            Main m = new Main(framework, appInfo);
            this.Hide();
            m.Show();
        }
    }
}
