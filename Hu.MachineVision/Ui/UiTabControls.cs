using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

using Hu.MachineVision.Config;

namespace Hu.MachineVision.Ui
{
    public class UiTabControls
    {
        public static Dictionary<string, TabControl> Tabs { get; set; }

        public static Dictionary<string, Panel> Panels { get; set; }

        public static ProjectCcd Project { get; set; }

        public static Dictionary<string, string> Names { get; set; }

        static UiTabControls()
        {
            Tabs = new Dictionary<string, TabControl>();
            Panels = new Dictionary<string, Panel>();
            Names = new Dictionary<string, string>();
        }

        public UiTabControls(ProjectCcd project)
        {
            Project = project;
        }

        public void AddTabMain(Panel panel)
        {
            string[] displayNames = { "Display" };
            string[] displayTexts = { "显示" };

            //string[] names = { "Vpp", "Serial", "Parameter" };
            //string[] texts = { "调试画面", "通讯设置", "参数设置" };

            string[] names = { "Vpp", "Serial" };
            string[] texts = { "调试画面", "通讯设置"};

            int groupCount = names.Count();
            int ccdCount = Project.CcdCount;
            int displayCount = displayNames.Count();

            int pageCount = ccdCount * groupCount + displayCount;
            var tab = AddTab(panel, pageCount);

            for (int i = 0; i < groupCount; i++)
            {
                Names[names[i]] = "Main";
            }

            for (int i = 0; i < displayCount; i++)
            {
                Names[displayNames[i]] = "Main";
            }

            for (int i = 0; i < displayCount; i++)
            {
                var tp = tab.TabPages[i];
                tp.Name = string.Format("{0}{1}", displayNames[i], i + 1);
                tp.Text = string.Format("{0}{1}", displayTexts[i], i + 1);
            }

            for (int i = 0; i < groupCount; i++)
            {
                for (int j = 0; j < ccdCount; j++)
                {
                    int pageIndex = i * ccdCount + j + displayCount;
                    var tp = tab.TabPages[pageIndex];
                    tp.Name = string.Format("{0}{1}", names[i], j + 1);
                    tp.Text = string.Format("{0}{1}", texts[i], j + 1);
                }
            }

            //string[] auxNames = { "Utility" };
            //string[] auxTexts = { "辅助工具" };

            //for (int i = 0; i < auxNames.Count(); i++)
            //{
            //    TabPage tp = new TabPage();
            //    tp.BackColor = Color.White;
            //    Names[auxNames[i]] = "Main";
            //    tp.Name = string.Format("{0}", auxNames[i]);
            //    tp.Text = string.Format("{0}", auxTexts[i]);
            //    tab.TabPages.Add(tp);
            //}

            Tabs["Main"] = tab;
            Panels["Main"] = panel;
        }

        public void AddTabData(Panel panel)
        {
            string[] names = { "Data", "RawData", "CompValue", "RefValue" };
            string[] texts = { "结果值", "原始值", "补偿值", "标准值" };

            int groupCount = names.Count();
            int ccdCount = Project.CcdCount;

            int pageCount = ccdCount * groupCount;
            var tab = AddTab(panel, pageCount);

            for (int i = 0; i < groupCount; i++)
            {
                Names[names[i]] = "Data";
            }

            for (int i = 0; i < groupCount; i++)
            {
                for (int j = 0; j < ccdCount; j++)
                {
                    int pageIndex = i * ccdCount + j;
                    var tp = tab.TabPages[pageIndex];
                    tp.Name = string.Format("{0}{1}", names[i], j + 1);
                    tp.Text = string.Format("{0}{1}", texts[i], j + 1);
                }
            }

            //string[] auxNames = { "Config", "Opinion", "Tool" };
            //string[] auxTexts = { "设置", "选项", "工具" };

            //string[] auxNames = { "Config" };
            //string[] auxTexts = { "设置"};

            //for (int i = 0; i < auxNames.Count(); i++)
            //{
            //    TabPage tp = new TabPage();
            //    tp.BackColor = Color.White;
            //    Names[auxNames[i]] = "Data";
            //    tp.Name = string.Format("{0}", auxNames[i]);
            //    tp.Text = string.Format("{0}", auxTexts[i]);
            //    tab.TabPages.Add(tp);
            //}

            Tabs["Data"] = tab;
            Panels["Data"] = panel;
        }



        public TabPage this[string name, int ccdId]
        {
            get
            {
                string tabName = Names[name];
                var tab = Tabs[tabName];
                var tp = tab.TabPages[string.Format("{0}{1}", name, ccdId + 1)];
                return tp;
            }
        }

        public TabPage this[string name]
        {
            get
            {
                string tabName = Names[name];
                var tab = Tabs[tabName];
                var tp = tab.TabPages[name];
                return tp;
            }
        }

        public TabControl AddTab(Panel panel, int pages)
        {
            TabControl tab = panel.Controls.OfType<TabControl>().First();
            tab.Dock = DockStyle.Fill;

            int tabCount0 = tab.TabCount;
            for (int i = tabCount0; i < pages; i++)
            {
                TabPage tp = new TabPage();
                tp.BackColor = Color.White;
                tab.TabPages.Add(tp);
            }

            return tab;
        }
    }
}
