using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;

using Hu.MachineVision.Ui;
using Hu.MachineVision.Database;


namespace Hu.MachineVision.VisionPro
{
    public class EditStation
    {
        public static UiTabControls MyTabs { get { return UiMainForm.MyTabs; } }
        public static Dictionary<string, Panel>[] Panels { get; set; }
        public static CogToolBlockEditV2[] EditWindows { get; set; }
        public int CcdId { get; set; }
        public TabPage Tp { get { return MyTabs["Vpp", CcdId]; } }
        public Panel ZoneMain { get { return Panels[CcdId]["Main"]; } }
        public Panel ZoneAux { get { return Panels[CcdId]["Aux"];}}
        public CogToolBlockEditV2 EditWindow { get { return EditWindows[CcdId]; } }
        public static Dictionary<int, EditStation> Stations { get; set; }

        static EditStation()
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
            }

            Stations = new Dictionary<int, EditStation>();
            for (int i = 0; i < ccdCount; i++)
            {
                Stations[i] = new EditStation(i);
            }
        }

        public EditStation(int ccd)
        {            
            CcdId = ccd;            
        }

        public string LoadVpp()
        {
            var db = DbScheme.GetConnection("Data");
            int brandId = db.ExecuteScalar<int>("select data from RunStatus where name = ?", "BrandId");
            var vpp = Helper.VppHelper.FindVpps(CcdId, brandId);
            return vpp[""];
        }

        public static EditStation GetStation(int ccd)
        {
            if (!Stations.ContainsKey(ccd))
            {
                Stations[ccd] = new EditStation(ccd);
            }
            return Stations[ccd];
        }


    }
}
