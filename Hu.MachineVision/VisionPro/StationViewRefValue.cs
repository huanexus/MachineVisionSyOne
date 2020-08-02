using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data;

using System.Drawing;
using System.Windows.Forms;


using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;

using Hu.MachineVision.Ui;
using Hu.MachineVision.Database;

namespace Hu.MachineVision.VisionPro
{
    class StationViewRefValue
    {
        public static UiTabControls MyTabs { get { return UiMainForm.MyTabs; } }
        public static Dictionary<string, Panel>[] Panels { get; set; }
        public static DataGridView[] Dgvs { get; set; }
        public static StationToolBlockEdit[] EditStations { get; set; }
        public static int CcdCount { get { return DbHelper.GetUiParams("CcdCount"); } }
        public static string StationName { get { return "RefValue"; } }
        public static StationViewRefValue[] Stations { get; set; } 
       

        public int CcdId { get; set; }
        public TabPage Tp { get { return MyTabs[StationName, CcdId]; } }
        public Panel ZoneMain { get { return Panels[CcdId]["Main"]; } }
        public DataGridView Dgv { get { return Dgvs[CcdId]; } }
        public StationToolBlockEdit EditStation { get { return EditStations[CcdId]; } }

        static StationViewRefValue()
        {
            Dgvs = new DataGridView[CcdCount];
            EditStations = new StationToolBlockEdit[CcdCount];
            Panels = new Dictionary<string, Panel>[CcdCount];
            Stations = new StationViewRefValue[CcdCount];

            for (int i = 0; i < CcdCount; i++)
            {
                var tp = MyTabs[StationName, i];
                Panels[i] = new Dictionary<string, Panel>();
                Panels[i]["Main"] = new Panel();
                Panels[i]["Main"].Width = tp.Width - 20;
                Panels[i]["Main"].Height = tp.Height - 30;
                tp.Controls.Add(Panels[i]["Main"]);
                Panels[i]["Main"].Location = new Point(0, 0);
                Dgvs[i] = new DataGridView();
                Panels[i]["Main"].Controls.Add(Dgvs[i]);
                Dgvs[i].Dock = DockStyle.Fill;

                EditStations[i] = StationToolBlockEdit.GetStation(i);
            }

            for (int i = 0; i < CcdCount; i++)
            {
                Stations[i] = new StationViewRefValue(i);
            }
        }

        public StationViewRefValue(int ccdId)
        {
            CcdId = ccdId;
        }

        public DataGridView this[int ccdId]
        {
            get { return Dgvs[ccdId]; }
        }

        public IDataViewStation GetStation(int ccd)
        {
            return Stations[ccd] as IDataViewStation;
        }
    }
}
