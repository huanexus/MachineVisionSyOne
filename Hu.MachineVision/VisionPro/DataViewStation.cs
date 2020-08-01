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
    public class DataViewStation
    {
        public static UiTabControls MyTabs { get { return UiMainForm.MyTabs; } }
        public static Dictionary<string, Panel>[] Panels { get; set; }
        public static DataGridView[] Dgvs { get; set; }

        public static ToolBlockEditStation[] EditStations { get; set; }

        public static int CcdCount { get { return DbHelper.GetUiParams("CcdCount"); } }
       
        public int CcdId { get; set; }
        public TabPage Tp { get { return MyTabs["Data", CcdId]; } }
        public Panel ZoneMain { get { return Panels[CcdId]["Main"]; } }
        public DataGridView Dgv { get { return Dgvs[CcdId]; } }
        public ToolBlockEditStation EditStation { get {return EditStations[CcdId];} }

        static DataViewStation()
        {
            Dgvs = new DataGridView[CcdCount];
            EditStations = new ToolBlockEditStation[CcdCount];
            Panels = new Dictionary<string, Panel>[CcdCount];
            
            for(int i = 0; i < CcdCount; i++)
            {
                var tp = MyTabs["Data", i];                
                Panels[i] = new Dictionary<string, Panel>();
                Panels[i]["Main"] = new Panel();
                Panels[i]["Main"].Width = tp.Width - 20;
                Panels[i]["Main"].Height = tp.Height - 30;
                tp.Controls.Add(Panels[i]["Main"]);
                Panels[i]["Main"].Location = new Point(0, 0);
                Dgvs[i] = new DataGridView();
                Panels[i]["Main"].Controls.Add(Dgvs[i]);
                Dgvs[i].Dock = DockStyle.Fill;

                EditStations[i] = ToolBlockEditStation.GetStation(i);
            }
        }

        public DataViewStation(int ccdId)
        {
            CcdId = ccdId;
        }
    }
}
