﻿using System;
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
                f.start(this);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (active)
            {
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

        public void showDoneMessage()
        {
            MessageBox.Show("Conversion Complete!");
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
                f.archiveOutput(this);
            }
        }
    }
}
