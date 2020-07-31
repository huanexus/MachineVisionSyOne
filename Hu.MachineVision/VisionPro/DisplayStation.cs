using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hu.MachineVision.Ui;
using Hu.MachineVision.Config;
using Hu.MachineVision.Database;

using System.Drawing;
using System.Windows.Forms;

using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;

namespace Hu.MachineVision.VisionPro
{
    public class DisplayStation
    {
        public static UiTabControls MyTabs { get { return UiMainForm.MyTabs; } }
        public static Panel[] Panels { get; set; }
        public static CogRecordDisplay[] DisplayWindows { get; set; }
        public int CcdId { get; set; }
        public TabPage Tp { get { return MyTabs["Display", 0]; } }
        public Panel Window { get { return Panels[CcdId]; } }
        public CogRecordDisplay Display { get { return DisplayWindows[CcdId]; } }
        public static Dictionary<int, DisplayStation> Stations { get; set; }
        public static int CcdCount { get { return DbScheme.GetUiParams("CcdCount"); } }

        static DisplayStation()
        {
            var db = DbScheme.GetConnection("Main");
            int ccdCount = CcdCount;
            Panels = new Panel[ccdCount];
            DisplayWindows = new CogRecordDisplay[ccdCount];
            var tp = MyTabs["Display", 0];
            int width = tp.Width / 2;
            int height = tp.Height / 2;

            for (int i = 0; i < ccdCount; i++)
            {
                Panels[i] = new Panel();
                DisplayWindows[i] = new CogRecordDisplay();

                Panels[i] = new Panel();
                DisplayWindows[i] = new CogRecordDisplay();
                DisplayWindows[i].Dock = DockStyle.Fill;
                DisplayWindows[i].Tag = false;
                Panels[i].Controls.Add(DisplayWindows[i]);
                tp.Controls.Add(Panels[i]);
            }
            
            Panels[0].Location = new Point(0, 0);
            Panels[0].Size = new Size(width, height);           

            Panels[1].Location = new Point(width, 0);
            Panels[1].Size = new Size(width, height);            

            Panels[2].Location = new Point(0, height);
            Panels[2].Size = new Size(width * 2, height);

            for (int i = 0; i < CcdCount; i++)
            {
                Panels[i].Tag = i;  
            }                       

            Stations = new Dictionary<int, DisplayStation>();
            for (int i = 0; i < ccdCount; i++)
            {
                Stations[i] = new DisplayStation(i);
                Stations[i].SetDoubleClick();
                   
                Label myLabel = new Label();
                myLabel.Text = string.Format("画面{0}", i + 1);
                myLabel.Location = new Point(20, 20);
                myLabel.ForeColor = Color.Yellow;
                Stations[i][i].Controls.Add(myLabel);                    
            }            
        }

        public static DisplayStation GetStation(int ccd)
        {
            if (!Stations.ContainsKey(ccd))
            {
                Stations[ccd] = new DisplayStation(ccd);
            }
            return Stations[ccd];
        }

        public DisplayStation(int ccdId)
        {
            CcdId = ccdId;
        }

        public CogRecordDisplay this[int index]
        {
            get { return DisplayWindows[index]; }
        }

        private void SetDoubleClick()
        {
            Display.DoubleClick += DisplayStation_DoubleClick;
        }

        private void DisplayStation_DoubleClick(object sender, EventArgs e)
        {
            CogRecordDisplay display = sender as CogRecordDisplay;
            Panel panel = display.Parent as Panel;
            int displayIndex = (int)panel.Tag;
            bool status = (bool)display.Tag;

            int ccdCount = DbScheme.GetUiParams("CcdCount");
            if (!status)
            {
                panel.Location = new Point(0, 0);
                panel.Size = new Size(Tp.Width, Tp.Height);
                for (int i = 0; i < ccdCount ; i++)
                {
                    if (i != displayIndex)
                    {
                        Panels[i].Hide();
                    }
                    else
                    {
                        Panels[i].Show();
                    }
                }
                display.Tag = true;
            }
            else
            {
                var tp = MyTabs["Display", 0];
                int width = tp.Width / 2;
                int height = tp.Height / 2;
                Panels[0].Location = new Point(0, 0);
                Panels[0].Size = new Size(width, height);
                Panels[1].Location = new Point(width, 0);
                Panels[1].Size = new Size(width, height);
                Panels[2].Location = new Point(0, height);
                Panels[2].Size = new Size(width * 2, height);

                for (int i = 0; i < CcdCount; i++)
                {
                    Panels[i].Show();
                }

                display.Tag = false;
            }
        }
    }
}
