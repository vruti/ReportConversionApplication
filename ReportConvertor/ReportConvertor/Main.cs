using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ReportConverter
{
    public partial class Main : Form
    {
        private Framework f;
        private bool active;
        private AppInfo a;
        private ExcelFileWriter eWriter;

        public Main(Framework framework, AppInfo appInfo)
        {
            InitializeComponent();
            f = framework;
            a = appInfo;
            active = true;
            eWriter = new ExcelFileWriter(a);
            progressBar.Visible = false;
            progressBar.Minimum = 0;
            progressBar.Step = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (active)
            {
                Start.Enabled = false;
                f.start(this, progressBar);
                Start.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (active)
            {
                active = false;
                Settings s = new Settings(f, this, a);
                s.Show();
            }
        }

        public void activateForm()
        {
            Console.WriteLine("In Activate Form");
            active = true;
        }

        public void showMessage(string m)
        {
            MessageBox.Show(m);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (active)
            {
                active = false;
                string message = f.archiveOutput();
                showMessage(message);
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            eWriter.writeFileLocs();
        }
    }
}
