using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace Hu.MachineVision.Ui
{
    public class UiLogDialog
    {
        private static TextBox mDialog = null;
        public static Timer MyTimer { get; set; }
        public static TextBox Dialog
        {
            get 
            { 
                return mDialog;
            }
            set 
            { 
                if(mDialog == null)
                {
                    mDialog = value; 
                }               
            }
        }

        static UiLogDialog()
        {
            MyTimer = null;
            Dialog = null;
        }       

        private static string StampMessage(string message, DateTime datetime)
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff}, {1}", datetime, message);
        }

        public UiLogDialog(TextBox dialog = null)
        {
            if(Dialog == null)
            {
                Dialog = dialog;
            }
        }

        public UiLogDialog(Panel panel)
        {
            Label lblLog = new Label();
            Label lblTimestamp = new Label();
            TextBox txtLog = new TextBox();

            lblLog.Text = "操作日志:";
            panel.Width = Math.Max(panel.Width, 360);
            lblLog.Location = new Point(0, 0);
            lblLog.Width = 60;
            lblTimestamp.Location = new Point(lblLog.Right);
            lblTimestamp.Width = 200;
            txtLog.Multiline = true;
            txtLog.Location = new Point(0, lblLog.Bottom);
            txtLog.Height = panel.Height - txtLog.Top;
            txtLog.Width = panel.Width;          

            panel.Controls.Add(lblLog);
            panel.Controls.Add(lblTimestamp);
            panel.Controls.Add(txtLog);

            Dialog = txtLog;
            if (MyTimer == null)
            {
                MyTimer = new Timer();
                MyTimer.Tick += (s, e) => lblTimestamp.Text = string.Format("{0:yyyy-MM-dd HH-mm-ss}", DateTime.Now);
                MyTimer.Start();
            }

            lblLog.Click += (s, e) => txtLog.Clear();
        }
        public void WriteMessage(string message, DateTime datetime)
        {
            if (Dialog != null && !string.IsNullOrWhiteSpace(message))
            {
                Dialog.Text = StampMessage(message, datetime) + Environment.NewLine + Dialog.Text;
            }
        }

        public void WriteMessage(string message)
        {
            WriteMessage(message, DateTime.Now);
        }
    }  

}
