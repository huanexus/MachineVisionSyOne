using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading.Tasks.Dataflow;

using Hu.MachineVision;
using Hu.MachineVision.Config;
using Hu.MachineVision.Ui;
using Hu.MachineVision.Database;
using Hu.MachineVision.VisionPro;
using Hu.MachineVision.Helper;

using Cognex.VisionPro;



using Hu.MachineVision.SerialSy;
using Cognex.VisionPro.FGGigE;
using Cognex.VisionPro.FGGigE.Implementation.Internal;

namespace MachineVisionSyOne
{
    public partial class Form1 : Form
    {
        public long Timestamp { get; set; }


        public ActionBlock<string> MessageBlock { get; set; }

        
        public Form1()
        {
            InitializeComponent();
            MessageBlock = new ActionBlock<string>(x => UiMainForm.LogMessage(x));
            SYMVDIO.MessageBuffer.LinkTo(MessageBlock);
        }

        private void Form1_Load(object sender, EventArgs e)
        {           
            UiMainForm.Layout(this);
            UiMainForm.LayoutDisplay();
            UiMainForm.LayoutEdit();
            UiMainForm.LayoutDataView();

            SyHelper.AddUi(0);
            SyHelper.AddUi(1);

            Timestamp = DateTime.Now.Ticks;            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CogFrameGrabberGigEs cameras = new CogFrameGrabberGigEs();
            try
            {
                foreach (CogFrameGrabberGigE item in cameras)
                {
                    item.Disconnect(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("关闭相机失败{０}", ex.Message));
            }

            UiMainForm.LogMessage("程序已退出!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double data = DbScheme.GetCcdParams(0, 0, 0, "Exposure");
            UiMainForm.LogMessage(data.ToString());
        }
         
    }
}
