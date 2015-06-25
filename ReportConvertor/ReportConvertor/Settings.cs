using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportConverter
{
    public partial class Settings : Form
    {
        Framework f;
        Main m;
        public Settings(Framework fw, Main main)
        {
            InitializeComponent();
            f = fw;
            m = main;
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            m.activateForm();
        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }

    }
}
