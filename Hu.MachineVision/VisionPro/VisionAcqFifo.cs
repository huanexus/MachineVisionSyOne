using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using System.Windows.Forms;

using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.FGGigE;
using Cognex.VisionPro.FGGigE.Implementation.Internal;

using Hu.MachineVision.Database;
using Hu.MachineVision.Ui;

namespace Hu.MachineVision.VisionPro
{
    public class AcqFifoToolLoader
    {
        public static CogFrameGrabberGigEs MyCameras = new CogFrameGrabberGigEs();
        public const string VIDEO_FORMAT = "Generic GigEVision (Mono)";
        public const CogAcqFifoPixelFormatConstants FIFO_TYPE = CogAcqFifoPixelFormatConstants.Format8Grey;
        public List<ICogAcqFifo> MyCogAcqFifos { get; set; }
        public List<List<string>> MyVideoFormats { get; set; }
        public CogAcqFifoTool MyCogAcqFifoTool { get; set; }   
        public ICogAcqFifo MyAcqFifo { get; set; }
        public int TrigNum { get; set; }
        public ICogImage MyImage {get; set;}
        public int CcdId { get; set; }
        public static int[] CameraIndexes { get; set; }
        public int CameraIndex { get; set; }

        
        static AcqFifoToolLoader()
        {
            CameraIndexes = Enumerable.Range(0, DbHelper.GetUiParams("CcdCount")).ToArray();
        }
        public AcqFifoToolLoader(int ccd)
        {
            MyCogAcqFifoTool = new CogAcqFifoTool(); 
            MyImage = null;

            TrigNum = 0;
            CcdId = ccd;
            CameraIndex = CameraIndexes[CcdId];

            int cameraCount = MyCameras.Count;
            string videoFormat = VIDEO_FORMAT;
            CogAcqFifoPixelFormatConstants fifoType = CogAcqFifoPixelFormatConstants.Format8Grey;

            MyCogAcqFifos = new List<ICogAcqFifo>();
            MyVideoFormats = new List<List<string>>();
            ICogFrameGrabber camera;
            ICogAcqFifo acqFifo;
           
            for (int i = 0; i < cameraCount; i++)
            {
                camera = MyCameras[i];                
                MyVideoFormats.Add(new List<string>());                
                for (int j = 0; j < camera.AvailableVideoFormats.Count; j++)
                {
                    MyVideoFormats[i].Add(camera.AvailableVideoFormats[j]);
                }
                videoFormat = MyVideoFormats[i][0];

                if (!videoFormat.Contains("Mono"))
                {
                    fifoType =  CogAcqFifoPixelFormatConstants.Format3Plane;
                }

                MyCogAcqFifos.Add(camera.CreateAcqFifo(videoFormat, fifoType, 0, false));
                acqFifo = MyCogAcqFifos[i];
            }
            
        }  

        public void Aquire(double exposure = 35)
        {
            int trigNum = 0;
            try
            {
                MyAcqFifo = MyCogAcqFifos[CameraIndex];
                MyAcqFifo.OwnedExposureParams.Exposure = exposure;
                MyImage = (ICogImage)MyAcqFifo.Acquire(out trigNum);                
                VisionRoi roi = new VisionRoi(CcdId);
                MyImage = roi.Trim(MyImage as CogImage8Grey);                
                TrigNum = trigNum;
            }
            catch
            {
                UiMainForm.LogMessage(string.Format("CCD{0}相机未找到!", CcdId));
            }   
        }
    }
}
