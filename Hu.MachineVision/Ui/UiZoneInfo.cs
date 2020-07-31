using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

using System.Threading.Tasks.Dataflow;



using Hu.MachineVision.Config;
using Hu.MachineVision.Database;

namespace Hu.MachineVision.Ui
{
    public class UiZoneInfo
    {
        public static Panel MainPanel { get; set; }
        public static GroupBox[] Zones { get; set; }
        public static ZoneInfo[] Infos { get; set; }

        public static ActionBlock<CcdCycle>[] ZoneBlocks { get; set; }
        public static ActionBlock<bool>[] StatusBlocks { get; set; }

        public static ProjectCcd Project { get; set; }

        public UiZoneInfo(ProjectCcd project, Panel panel)
        {
            Project = project;
            MainPanel = panel.Controls.OfType<Panel>().First();
            MainPanel.Dock = DockStyle.Fill;

            int zoneCount = Project.CcdCount;
            Zones = new GroupBox[zoneCount];
            Infos = new ZoneInfo[zoneCount];
            ZoneBlocks = new ActionBlock<CcdCycle>[zoneCount];
            StatusBlocks = new ActionBlock<bool>[zoneCount];
            int width = panel.Width;
            int height = panel.Height;

            int height0 = height / zoneCount;
            int width0 = width;

            var db = DbScheme.GetConnection("Main");
            string text = string.Empty;

            for (int i = 0; i < zoneCount; i++)
            {
                Zones[i] = new GroupBox();
                Zones[i].Width = width0;
                Zones[i].Height = height0 - 5;
                Zones[i].Left = 0;
                Zones[i].Top = height0 * i;
                text = db.ExecuteScalar<string>("select nameCn from CcdInfo where ccdId = ?", i);
                Zones[i].Text = text; // string.Format("CCD{0}", i + 1);
                MainPanel.Controls.Add(Zones[i]);
                Infos[i] = new ZoneInfo(i, Project.BrandId);
            }

            for(int i = 0; i < zoneCount; i++)
            {
                AddZoneUi(i);
            }
        }

        public void ShowInfo(int zoneIndex)
        {
            var info = Infos[zoneIndex];
            var block = ZoneBlocks[zoneIndex];
            block.Post(info.GetInfo());
        }

        public void AddZoneUi(int zoneIndex)
        {
            GroupBox zone = Zones[zoneIndex];
            ZoneInfo info = Infos[zoneIndex];           

            var uiBox = zone;

            Label lblOkNg = new Label();
            lblOkNg.Text = "NG";
            lblOkNg.ForeColor = Color.Red;
            uiBox.Controls.Add(lblOkNg);

            Button btnReset = new Button();
            btnReset.Text = "产量清零";
            uiBox.Controls.Add(btnReset);

            string[] names = { "All", "Ok", "Ng" };
            string[] texts = { "产  量", "OK数量", "NG数量" };

            TextBox[] boxes = new TextBox[3];
            Label[] labels = new Label[3];

            for(int i = 0; i < 3; i++)
            {
                boxes[i] = new TextBox();
                labels[i] = new Label();

                labels[i].Text = texts[i] + ":";
                labels[i].Tag = i;
                uiBox.Controls.Add(labels[i]);
                uiBox.Controls.Add(boxes[i]);
                labels[i].Width = 50;
                boxes[i].Width = 110;
            }

            int left = 20;
            int top = 30;
            int heightItem = (uiBox.Height - top) / 3;

            for (int i = 0; i < 3; i++)
            {
                labels[i].Location = new Point(left, top + heightItem * i);
                boxes[i].Location = new Point(labels[i].Right + left, labels[i].Top - 5);
            }

            lblOkNg.Size = new Size(65, 60);
            lblOkNg.Font = new Font("SimSun", 32);
            lblOkNg.Location = new Point(boxes[1].Right + 5, uiBox.Height / 2 - 15);

            btnReset.AutoSize = false;
            btnReset.Font = new Font("SimSun", 18);
            btnReset.Size = new Size(70, 60);
            btnReset.Left = lblOkNg.Right;
            btnReset.Top = uiBox.Height / 2 - btnReset.Height / 2;
            btnReset.UseVisualStyleBackColor = true;

            ZoneBlocks[zoneIndex] = new ActionBlock<CcdCycle>(x =>
            {
                if(uiBox.InvokeRequired)
                {
                    uiBox.Invoke(new Action(() =>
                        {
                            boxes[0].Text = x.All.ToString();
                            boxes[1].Text = x.Ok.ToString();
                            boxes[2].Text = x.Ng.ToString();
                        }));
                }
                else
                {
                    boxes[0].Text = x.All.ToString();
                    boxes[1].Text = x.Ok.ToString();
                    boxes[2].Text = x.Ng.ToString();
                }

                info.UpdateInfo(x);
            });

            StatusBlocks[zoneIndex] = new ActionBlock<bool>(x =>
            {
                if(uiBox.InvokeRequired)
                {
                    uiBox.Invoke(new Action(() =>
                    {
                        if (x)
                        {
                            lblOkNg.Text = "OK";
                            lblOkNg.ForeColor = Color.Green;
                        }
                        else
                        {
                            lblOkNg.Text = "NG";
                            lblOkNg.ForeColor = Color.Red;
                        }
                    }));
                }
                else
                {
                    if (x)
                    {
                        lblOkNg.Text = "OK";
                        lblOkNg.ForeColor = Color.Green;
                    }
                    else
                    {
                        lblOkNg.Text = "NG";
                        lblOkNg.ForeColor = Color.Red;
                    }
                }
            });

            info.UiBlock = ZoneBlocks[zoneIndex];
            info.StatusBlock = StatusBlocks[zoneIndex];
            btnReset.Click += (s, e) => info.Reset();  
        }

        public ZoneInfo this[int index]
        {
            get { return Infos[index]; }
        }
    }

    public class ZoneInfo
    {
        public int Index { get; set; }
        public int BrandId { get; set; }
        public int CcdId { get; set; }

        public ActionBlock<CcdCycle> UiBlock { get; set; }
        public ActionBlock<bool> StatusBlock { get; set; }        
        public ZoneInfo(int ccdId, int brandId)
        {
            BrandId = brandId;
            CcdId = ccdId;

            var db = DbScheme.GetConnection("Data");
            Index = db.ExecuteScalar<int>("select id from CcdCycle where brandId = ? and ccdId = ?", BrandId, CcdId);
        }

        public CcdCycle GetInfo()
        {
            var db = DbScheme.GetConnection("Data");
            var query = db.Query<CcdCycle>("select * from CcdCycle where id = ?", Index);
            return query.First();
        }

        public int UpdateInfo(CcdCycle ccdCycle)
        {
            var db = DbScheme.GetConnection("Data");
            return db.Update(ccdCycle);          
        }

        public int ResetInfo()
        {
            var ccdCycle = GetInfo();
            ccdCycle.All = 0;
            ccdCycle.Ok = 0;
            ccdCycle.Ng = 0;
            return UpdateInfo(ccdCycle);
        }

        public int UpdateInfo(bool status)
        {
            var ccdCycle = GetInfo();
            if (status)
            {
                ccdCycle.All++;
                ccdCycle.Ok++;
            }
            else
            {
                ccdCycle.All++;
                ccdCycle.Ng++;
            }            

           return UpdateInfo(ccdCycle);            
        }

        public void Show()
        {
            if(UiBlock != null)
            {
                UiBlock.Post(GetInfo());
            }
        }

        public void Show(bool status)
        {
          //  UpdateInfo(status);
           // Show();

            var ccdCycle = GetInfo();
            if (status)
            {
                ccdCycle.All++;
                ccdCycle.Ok++;
            }
            else
            {
                ccdCycle.All++;
                ccdCycle.Ng++;
            }

            UiBlock.Post(ccdCycle);
            StatusBlock.Post(status);
        }

        public void Reset()
        {
            ResetInfo();
            Show();
            StatusBlock.Post(false);
        }
    }
}
