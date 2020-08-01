using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Drawing;
using System.Windows.Forms;

using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;

using Hu.MachineVision.Ui;
using Hu.MachineVision.Database;


namespace Hu.MachineVision.VisionPro
{
    public class StationToolBlockEdit
    {
        public static UiTabControls MyTabs { get { return UiMainForm.MyTabs; } }
        public static Dictionary<string, Panel>[] Panels { get; set; }
        public static CogToolBlockEditV2[] EditWindows { get; set; }
        public int CcdId { get; set; }
        public TabPage Tp { get { return MyTabs["Vpp", CcdId]; } }
        public Panel ZoneMain { get { return Panels[CcdId]["Main"]; } }
        public Panel ZoneAux { get { return Panels[CcdId]["Aux"];}}
        public CogToolBlockEditV2 EditWindow { get { return EditWindows[CcdId]; } }
        public static Dictionary<int, StationToolBlockEdit> Stations { get; set; }

        public StationToolBlock MyToolBlockStation { get; set; }

        static StationToolBlockEdit()
        {
            var db = DbScheme.GetConnection("Main");
            int ccdCount = db.ExecuteScalar<int>("select data from UiParams where name = ?", "CcdCount");
            Panels = new Dictionary<string, Panel>[ccdCount];
            EditWindows = new CogToolBlockEditV2[ccdCount];

            for (int i = 0; i < ccdCount; i++)
            {
                Panels[i] = new Dictionary<string,Panel>();
                EditWindows[i] = new CogToolBlockEditV2();
                var tp = MyTabs["Vpp", i];
                Panels[i]["Main"] = new Panel();
                EditWindows[i] = new CogToolBlockEditV2();
                EditWindows[i].Dock = DockStyle.Fill;
                Panels[i]["Main"].Controls.Add(EditWindows[i]);

                tp.Controls.Add(Panels[i]["Main"]);
                int width = tp.Width;
                Panels[i]["Main"].Location = new Point(0, 0);
                Panels[i]["Main"].Size = new Size(width, tp.Height - 60);
                Panels[i]["Main"].Tag = "Main";

                Panels[i]["Aux"] = new Panel();
                tp.Controls.Add(Panels[i]["Aux"]);
                Panels[i]["Aux"].Location = new Point(0, Panels[i]["Main"].Bottom);
                Panels[i]["Aux"].Size = new Size(width, 60);
                Panels[i]["Aux"].Tag = "Aux";
                
            }

            Stations = new Dictionary<int, StationToolBlockEdit>();
            for (int i = 0; i < ccdCount; i++)
            {
                Stations[i] = new StationToolBlockEdit(i);
            }
        }

        public StationToolBlockEdit(int ccd)
        {            
            CcdId = ccd;
            MyToolBlockStation = new StationToolBlock(CcdId);
            EditWindow.Subject = MyToolBlockStation.MyCogToolBlock;
        }

        public static StationToolBlockEdit GetStation(int ccd)
        {
            if (!Stations.ContainsKey(ccd))
            {
                Stations[ccd] = new StationToolBlockEdit(ccd);
            }
            return Stations[ccd];
        }

        public void AddAuxUi()
        {
            int left = 30;
            int top = 20;

            Panel panel = ZoneAux;
            NumericUpDown nudImageCycle = new NumericUpDown();
            nudImageCycle.Value = 0;
            nudImageCycle.Width = 80;
            nudImageCycle.Maximum = 10000;
            nudImageCycle.Location = new Point(left, top);

            nudImageCycle.ValueChanged += nudImageCycle_ValueChanged;

            panel.Controls.Add(nudImageCycle);

            nudImageCycle.Value = 1;

            Button btnVppRunOffline = new Button();
            btnVppRunOffline.Text = "离线运行";
            btnVppRunOffline.Name = "RunOffline";
            btnVppRunOffline.Click += RunAuxCommand;
            btnVppRunOffline.Location = new Point(nudImageCycle.Right + 20, top);
            panel.Controls.Add(btnVppRunOffline);

            Button btnVppSaveImage = new Button();
            btnVppSaveImage.Text = "图片保存";
            btnVppSaveImage.Name = "SaveImage";
            btnVppSaveImage.Click += RunAuxCommand;
            btnVppSaveImage.Location = new Point(btnVppRunOffline.Right + 20, top);
            panel.Controls.Add(btnVppSaveImage);

            Button[] btnCommands = new Button[3];
            string[] btnNames = { "Run", "Load", "Save" };
            string[] btnTexts = { "调试运行", "重新加载", "程序保存" };


            left = panel.Width - 300;

            int btnCount = btnCommands.Count();

            for (int i = 0; i < btnCount; i++)
            {
                btnCommands[i] = new Button();
                btnCommands[i].Name = btnNames[i];
                btnCommands[i].Text = btnTexts[i];
                panel.Controls.Add(btnCommands[i]);
                if (i == 0)
                {
                    btnCommands[i].Location = new Point(left, top);
                }
                else
                {
                    btnCommands[i].Location = new Point(btnCommands[i - 1].Right + 20, top);
                }

                btnCommands[i].Click += RunAuxCommand;
            }

        }

        private void RunAuxCommand(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var name = btn.Name;
            switch (name)
            {
                case "RunOffline":
                    MyToolBlockStation.RunOffline();
                    break;
                case "Run":
                    MyToolBlockStation.Run();
                    break;
                case "Load":
                    MyToolBlockStation.Load();
                    EditWindow.Subject = MyToolBlockStation.MyCogToolBlock;
                    break;
                case "Save":
                    MyToolBlockStation.Save();
                    break;

                case "SaveImage":
                    MyToolBlockStation.SaveImage();
                    break;
                default:
                    break;
            }      
        }

        private void nudImageCycle_ValueChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }


    }
}
