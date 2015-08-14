using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Schedule_Generator
{
    public partial class Form1 : Form
    {
        ScheduleGenerator generator;
           
        public Form1()
        {
            InitializeComponent();
            this.generator = new ScheduleGenerator();
            this.generator.createTeststringsAndProfs();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.generator.generateSchedules();
        }

        

        

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }
    }
}
