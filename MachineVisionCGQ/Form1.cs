﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Hu.MachineVision;
using Hu.MachineVision.Config;
using Hu.MachineVision.Ui;
using Hu.MachineVision.Database;
using Hu.MachineVision.VisionPro;
using Hu.MachineVision.Helper;

using Hu.Mes.Fins;

namespace MachineVisionCGQ
{
    public partial class Form1 : Form
    {
        public long Timestamp { get; set; }

        public FinsTcp MyFinsTcp { get; set; }
        public Form1()
        {
            InitializeComponent();           
        }

        private void Form1_Load(object sender, EventArgs e)
        {           
            UiMainForm.Layout(this);
            UiMainForm.LayoutDisplay();
            UiMainForm.LayoutEdit();
            UiMainForm.LayoutDataView();

            Timestamp = DateTime.Now.Ticks;

            MyFinsTcp = new FinsTcp();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var infos = UiMainForm.MyZoneInfo;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            
            infos[0].Show(false);            
            infos[1].Show(true);
            sw.Stop();

            UiMainForm.LogMessage(sw.ElapsedMilliseconds.ToString());
            var a = VppHelper.FindVpps(0, 0);  

            foreach(var kv in a)
            {
                UiMainForm.LogMessage(kv.Key);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Timestamp = DateTime.Now.Ticks;
            UiMainForm.LogMessage(Timestamp.ToString());
            MyFinsTcp.WriteTimestamp(Timestamp);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            long stamp = MyFinsTcp.ReadTimestamp();
            UiMainForm.LogMessage(stamp.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DbHelper.SendData("");
        }
    }
}
