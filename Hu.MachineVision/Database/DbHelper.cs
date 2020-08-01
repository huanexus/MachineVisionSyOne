using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SimpleTCP;




namespace Hu.MachineVision.Database
{
    public static class DbHelper
    {
        public static SimpleTcpServer TcpServer { get; set; }

        static DbHelper()
        {
            TcpServer = new SimpleTcpServer();
            TcpServer.ClientConnected += TcpServer_ClientConnected;
            TcpServer.ClientDisconnected += TcpServer_ClientDisconnected;
            TcpServer.Start(1000);
        }

        private static void LogMessage(string message)
        {
            MachineVision.Ui.UiMainForm.LogMessage(message);
        }

        static void TcpServer_ClientDisconnected(object sender, System.Net.Sockets.TcpClient e)
        {
            LogMessage("客户端已断开");
        }

        static void TcpServer_ClientConnected(object sender, System.Net.Sockets.TcpClient e)
        {
            LogMessage("客户端已连接");
        }
        public static int GetUiParams(string name)
        {
            return DbScheme.GetUiParams(name);
        }

        public static int GetRunStatus(string name)
        {
            return DbScheme.GetRunStatus(name);
        }

        public static void SendData(string pd_id)
        {
            var db = DbScheme.GetConnection("Mes");
            var mesData = db.Query<CcdMes>("select * from CcdMes limit 1").ToArray();

            foreach (var data in mesData)
            {
                string s = JsonConvert.SerializeObject(data);

                MachineVision.Ui.UiMainForm.LogMessage(s);

                TcpServer.Broadcast(s);

            }
        }
    }
}
