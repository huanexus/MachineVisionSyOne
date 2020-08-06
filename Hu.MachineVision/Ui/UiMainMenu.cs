using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;

using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro;

using Hu.MachineVision.Database;

namespace Hu.MachineVision.Ui
{
    public static class UiMainMenu
    {
        public static int CcdCount { get; set; }

        public static int RunMode { get; set; }
        public static int MenuCount { get; set; }
        public static string[] MenuNameEn { get; set; }
        public static string[] MenuNameCn { get; set; }
        public static MenuStrip Menu { get; set; }
        public static Dictionary<string, ToolStripMenuItem> Menus = new Dictionary<string, ToolStripMenuItem>();

        public static Action[] LayoutRunMode { get; set; }
        static UiMainMenu()
        {
            CcdCount = DbHelper.GetUiParams("CcdCount");
            RunMode = DbHelper.GetRunStatus("RunMode");
            MenuNameEn = new string[] { "File", "Account",  "Debug", "Help"};
            MenuNameCn = new string[] { "文件", "权限",  "调试", "帮助"};
            MenuCount = MenuNameEn.Count();

            LayoutRunMode = new Action[2];
            LayoutRunMode[0] = new Action(() =>
            { 
                
            });
            LayoutRunMode[1] = new Action( () =>
            {

            });
        }

        public static void LayoutMenuZone(this Form mainForm)
        {
            Menu = mainForm.MainMenuStrip;
            int menuCount = Menu.Items.Count;
            for (int i = menuCount; i < MenuCount; i++)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem();
                Menu.Items.Add(tsmi);
            }

            for (int i = 0; i < MenuCount; i++)
            {
                ToolStripMenuItem tsmi = Menu.Items[i] as ToolStripMenuItem;
                tsmi.Text = MenuNameCn[i];
                tsmi.Name = MenuNameEn[i];
                tsmi.Font = new Font("SimSun", 10);
               
                Menus[tsmi.Name] = tsmi;
            }

            AddMenuFile();
            AddMenuAccount();
            AddMenuDebug();
            AddMenuHelp();

        }

        private static void AddMenuFile()
        {
            ToolStripMenuItem tsmi = GetMenuItem("File");            
            string[] menuList = { "退出" };
            AddMenuList(tsmi, menuList);
            tsmi.DropDown.ItemClicked += FileDropDown_ItemClicked;
        }

        private static void FileDropDown_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem tsItem = e.ClickedItem;
            int tag = (int)tsItem.Tag;
            if (tsItem.Text == "退出")
            {
                Application.Exit();
            }
        }

        private static void AddMenuAccount()
        {
            ToolStripMenuItem tsmi = GetMenuItem("Account");
            string[] menuList = { "管理员" };
            string[] menuListSub = { "登陆", "退出" };
            AddMenuList(tsmi, menuList);
            for (int i = 0; i < menuList.Length; i++)
            {
                ToolStripMenuItem tsmiSub = tsmi.DropDown.Items[i] as ToolStripMenuItem;
                AddMenuList(tsmiSub, menuListSub);
                tsmiSub.DropDown.ItemClicked += LoginDropDown_ItemClicked;
                (tsmiSub.DropDown.Items[0] as ToolStripMenuItem).Enabled = true;
                (tsmiSub.DropDown.Items[1] as ToolStripMenuItem).Enabled = false;
            }
        }

        public static int IsLogin
        {
            get 
            {
                return DbHelper.GetRunStatus("IsLogin");
            }
            set 
            {
                var db = DbScheme.Connections["Data"];
                db.InsertOrReplace(new RunStatus() { Name = "IsLogin", Data = value });
            }

        }
        

        private static void LoginDropDown_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var tsItem = e.ClickedItem;
            int tagItem = (int)tsItem.Tag;
            if (0 == tagItem)
            {
                var logForm = new Login();
                logForm.StartPosition = FormStartPosition.CenterParent;
                logForm.ShowDialog();
                if (IsLogin == 1)
                {
                    tsItem.Enabled = false;
                    ((tsItem.OwnerItem as ToolStripMenuItem).DropDown.Items[1] as ToolStripMenuItem).Enabled = true;
                    RunMode = 1;
                    LayoutRunMode[RunMode]();
                }
                else
                {
                    tsItem.Enabled = true;
                    ((tsItem.OwnerItem as ToolStripMenuItem).DropDown.Items[1] as ToolStripMenuItem).Enabled = false;
                }
            }
            if (1 == tagItem)
            {
                IsLogin = 0;
                tsItem.Enabled = false;
                ((tsItem.OwnerItem as ToolStripMenuItem).DropDown.Items[0] as ToolStripMenuItem).Enabled = true;
                RunMode = 0;
                LayoutRunMode[RunMode]();
            }
        }

        private static string GetCcdStationName(int ccdId, string name = "")
        {
            return string.Format("检测位{0}{1}", ccdId + 1, name);
        }

        private static void AddMenuDebug()
        {
            ToolStripMenuItem tsmi = GetMenuItem("Debug");

            string[] menuList = { "调试运行" };
            string[] menuListSub = Enumerable.Range(0, CcdCount).Select(p => GetCcdStationName(p, "程序")).ToArray();

            AddMenuList(tsmi, menuList);
            for (int i = 0; i < menuList.Length; i++)
            {
                ToolStripMenuItem tsmiSub = tsmi.DropDown.Items[i] as ToolStripMenuItem;
                AddMenuList(tsmiSub, menuListSub);
                tsmiSub.DropDown.ItemClicked += DebugDropDown_ItemClicked;
            }
        }

        private static void DebugDropDown_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var tsItem = e.ClickedItem;
            int ccdId = (int)tsItem.Tag;

            ToolStripMenuItem debugItem = tsItem.OwnerItem as ToolStripMenuItem;
            if (debugItem.Text == "调试运行")
            {
                RunParams.CcdOfflineBlock[ccdId].Post(0); 
            }
        }
       
        public static ToolStripMenuItem GetMenuItem(string name)
        {
            return Menus[name];
        }

        public static ToolStripMenuItem GetMenuItem(int index)
        {
            return Menus[MenuNameEn[index]];
        }

        private static void AddMenuList(ToolStripMenuItem tsmi, params string[] menuList)
        {
            for (int i = 0; i < menuList.Length; i++)
            {
                ToolStripMenuItem subMenuItem = new ToolStripMenuItem();
                subMenuItem.Text = menuList[i];
                subMenuItem.Tag = i;
                tsmi.DropDown.Items.Add(subMenuItem);
            }
        }

        private static void AddMenuHelp()
        {
            ToolStripMenuItem tsmi = GetMenuItem("Help");
            AddMenuList(tsmi, "关于");
            tsmi.DropDown.ItemClicked += HelpDropDown_ItemClicked;        
        }

        private static void HelpDropDown_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Form aboutForm = new About();
            aboutForm.StartPosition = FormStartPosition.CenterParent;
            aboutForm.Text = "帮助";
            aboutForm.ShowDialog();
        }


    }
}
