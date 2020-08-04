using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

using Hu.MachineVision.Config;
using Hu.MachineVision.Database;
using Hu.MachineVision.VisionPro;

namespace Hu.MachineVision.Ui
{
   public static class UiMainForm
    {
       public static Config.ProjectCcd Project { get; set; }
       public static Form MainForm { get; set; }
       public static MenuStrip MainMenu { get { return MainForm.MainMenuStrip; } }
       public static Dictionary<string, Panel> Panels { get; set; }

       public static Panel WorkAreaPanel { get; set; }
       public static int Width { get { return WorkAreaPanel.Width; } }
       public static int Height { get { return WorkAreaPanel.Height; } }

       public static Panel MainPanel { get { return Panels["pnlMain"];} }
       public static Panel DataPanel { get { return Panels["pnlData"]; } }
       public static Panel InfoPanel { get { return Panels["pnlInfo"]; } }
       public static Panel LogPanel { get { return Panels["pnlLog"]; } }
       public static UiLogDialog MyLog { get; set; }

       public static UiTabControls MyTabs { get; set; }

       public static UiZoneInfo MyZoneInfo { get; set; }

       public static int ScreenWidth { get; set; }
       public static int ScreenHeight { get; set; }  
       

       static UiMainForm()
       {
           DbScheme.Create();
           var db = DbScheme.Connections["Main"];

           int ccdCount = db.ExecuteScalar<int>("select data from UiParams where name = ?", "CcdCount");
           int brandCount = db.ExecuteScalar<int>("select data from UiParams where name = ?", "BrandCount");
           ScreenWidth = db.ExecuteScalar<int>("select data from UiParams where name = ?", "ScreenWidth");
           ScreenHeight = db.ExecuteScalar<int>("select data from UiParams where name = ?", "ScreenHeight");

           int[] partCounts = new int[ccdCount];

           for (int i = 0; i < ccdCount; i++)
           {
               partCounts[i] = db.ExecuteScalar<int>("select data from UiParams where name = ?", string.Format("PartCountCcd{0}", i + 1));
           }

           Project = new ProjectCcd(ccdCount, brandCount, partCounts);
           Panels = new Dictionary<string, Panel>();
           WorkAreaPanel = new Panel();
       }
       public static void Layout(Form mainForm)
       {
           MainForm = mainForm;
           int width = ScreenWidth;
           int height = ScreenHeight;
           MyTabs = new UiTabControls(Project);
           if(MainForm != null)
           {
               MainForm.Width = width;
               MainForm.Height = height;
               
               WorkAreaPanel.Location = new Point(20, 35);
               WorkAreaPanel.Width = width - 55;
               WorkAreaPanel.Height = height -105;
               var panels = MainForm.Controls.OfType<Panel>().ToArray();
               foreach(var panel in panels)
               {
                   Panels[panel.Name] = panel;
                   WorkAreaPanel.Controls.Add(panel);
               }
               MainForm.Controls.Add(WorkAreaPanel);
           }

           LayoutPanels();
       }

       private static void LayoutPanels()
       { 
           InfoPanel.Width = 360;
           LogPanel.Width = 360;          
           InfoPanel.Height = 132 * Project.CcdCount;
           LogPanel.Height = Height - InfoPanel.Height - 15;

           DataPanel.Height = 260;
           DataPanel.Width = Width - LogPanel.Width - 15;
           MainPanel.Height = Height - DataPanel.Height - 15;
           MainPanel.Width = DataPanel.Width;          

           MainPanel.Location = new Point(0, 0);
           DataPanel.Location = new Point(0, MainPanel.Bottom + 15);
           InfoPanel.Location = new Point(MainPanel.Right + 15, 0);
           LogPanel.Location = new Point(DataPanel.Right + 15, InfoPanel.Bottom + 15);
           MyLog = new UiLogDialog(LogPanel);
           MyTabs.AddTabMain(MainPanel);
           MyTabs.AddTabData(DataPanel);

           MyZoneInfo = new UiZoneInfo(Project, InfoPanel);                  
       }

       public static void LayoutDisplay()
       {
           var infos = MyZoneInfo;
           for (int i = 0; i < Project.CcdCount; i++)
           {
               infos[i].Show();
               var display = StationDisplay.GetStation(i);
           }
       }

       public static void LayoutEdit()
       {
          
           for (int i = 0; i < Project.CcdCount; i++)
           {               
               var edit = StationToolBlockEdit.GetStation(i);
               
               edit.AddAuxUi();
           }
       }

       public static void LayoutDataView()
       {

           for (int i = 0; i < Project.CcdCount; i++)
           {
               var station = new StationViewData(i);
               
           }

           //for (int i = 0; i < Project.CcdCount; i++)
           //{
           //    var station = new StationViewRawData(i);

           //}

           //for (int i = 0; i < Project.CcdCount; i++)
           //{
           //    var station = new StationViewCompValue(i);

           //}

           //for (int i = 0; i < Project.CcdCount; i++)
           //{
           //    var station = new StationViewRefValue(i);

           //}
       }

       public static void LogMessage(string message)
       {
           MyLog.WriteMessage(message);
       }
    }
}
