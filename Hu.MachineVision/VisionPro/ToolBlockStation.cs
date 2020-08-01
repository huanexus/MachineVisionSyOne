using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;

using System.Threading.Tasks.Dataflow;

using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.ImageProcessing;

using Hu.MachineVision.Database;
using Hu.MachineVision.Ui;

namespace Hu.MachineVision.VisionPro
{
   public class ToolBlockStation
    {
       public int CcdId { get; set; }
       public int OfflineImageCycle { get; set; }
       public string VppFileName { get; set; }
       public CogToolBlock MyCogToolBlock { get; set; }

       public  ActionBlock<CcdTerminalIn> VtInBlock { get; set; }

       public ToolBlockStation(int ccdId)
       {
           CcdId = ccdId;
           OfflineImageCycle = 1;
           var db = DbScheme.GetConnection("Data");
           int brandId = db.ExecuteScalar<int>("select data from RunStatus where name = ?", "BrandId");
           VppFileName = Helper.VppHelper.FindVpps(CcdId, brandId)[""];
           LoadVpp();
       }

       public CogToolBlock LoadVpp()
       {           
           if (File.Exists(VppFileName))
           {
               MyCogToolBlock = CogSerializer.LoadObjectFromFile(VppFileName) as CogToolBlock;
           }
           else
           {
               MyCogToolBlock = new CogToolBlock();
           }

           return MyCogToolBlock;
       }

       public void Run()
       {
           MyCogToolBlock.Run();
       }

       public void Save()
       {           
           CogSerializer.SaveObjectToFile(MyCogToolBlock, VppFileName);
           LogMessage(string.Format("CCD{0}检查程序已保存", CcdId + 1));
       }

       private void LogMessage(string message)
       {
           UiMainForm.LogMessage(message);
       }

       public void Load()
       {           
           MyCogToolBlock = CogSerializer.LoadObjectFromFile(VppFileName) as CogToolBlock;
       }

       public void SaveImage()
       {           
           string vppHome = Path.GetDirectoryName(VppFileName);
           DirectoryInfo diImage = new DirectoryInfo(Path.Combine(vppHome, "image"));
           if (!diImage.Exists)
           {
               diImage.Create();
           }

           var toolBlock = MyCogToolBlock;
           CogIPOneImageTool[] myCogIPOneImageTools = toolBlock.Tools.OfType<CogIPOneImageTool>().ToArray();
           int imageCount = myCogIPOneImageTools.Count();
           for (int i = 0; i < imageCount; i++)
           {
               var myCogIPOneImageTool = myCogIPOneImageTools[i];
               var imageName = string.Format("{0}-{1}-{2}.bmp", CcdId, OfflineImageCycle, i + 1);
               var imageFile = Path.Combine(diImage.FullName, imageName);
               var inputImage = myCogIPOneImageTool.InputImage as CogImage8Grey;
               Bitmap imageBmp = inputImage.ToBitmap();
               imageBmp.Save(imageFile);
           }
       }


       internal void RunOffline()
       {
          // throw new NotImplementedException();
       }
    }
}
