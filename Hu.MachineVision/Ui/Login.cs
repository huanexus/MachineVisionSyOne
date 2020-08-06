using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Hu.MachineVision.Database;

namespace Hu.MachineVision.Ui
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
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

        private void btnLoginOk_Click(object sender, EventArgs e)
        {
            var password = txtPassword.Text.Trim();
            IsLogin = 0;
            if (password == "003790")
            {
                IsLogin = 1;
                Close();
            }
            else
            {
                MessageBox.Show("密码不正确！");
            } 
        }

        private void btnLoginCancel_Click(object sender, EventArgs e)
        {
            IsLogin = 0;
            Close();
        }
    }
}
