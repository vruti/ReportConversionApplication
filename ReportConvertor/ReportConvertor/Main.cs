using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportConvertor
{
    public partial class Main : Form
    {
        private Framework f;
        private bool active;

        public Main(Framework framework)
        {
            InitializeComponent();
            f = framework;
            active = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (active)
            {
                active = false;
                f.start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (active)
            {
                Console.WriteLine("In Go to Settings");
                active = false;
                Settings s = new Settings(f, this);
                s.Show();
            }
        }

        public void activateForm()
        {
            Console.WriteLine("In Activate Form");
            active = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
