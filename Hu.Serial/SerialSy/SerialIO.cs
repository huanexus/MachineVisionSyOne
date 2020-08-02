using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace Vision.SerialSy
{
    public class SerialIO
    {
        public int CcdId { get; set; }
        public int Channel { get; set; }
        public VirtualIoUi Ui { get; set; }
        public VirtualIo Io { get { return VirtualIo.GetDevice(Channel); } }
        public SerialSy Sy { get { return SerialSy.GetDevice(Channel); } }
        private static int IoCount { get; set; }
        private static SerialIO[] Serials { get; set; }

       
        static SerialIO()
        {
            IoCount = VisionAppConfig.CcdCount;
            Serials = new SerialIO[IoCount];
           
            for (int i = 0; i < IoCount; i++)
            {
                Serials[i] = new SerialIO(i);
            }
        }

        public static SerialIO GetSerialIO(int ccdId)
        {           
            return Serials[ccdId];
        }

        private SerialIO(int ccd)
        {
            CcdId = ccd;
            Channel = VirtualIo.GetChannel(CcdId);     
            Ui = new VirtualIoUi(Io);
            if(Io != null)
            {
                Io.StartWatch();
            }            
        }

        public void StartWatch()
        {
            Io.StartWatch();
        }

        public void AddUi(TabPage tp)
        {
            Ui.Associate(tp);
        }
    }
}
