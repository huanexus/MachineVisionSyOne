using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

using System.IO;
using System.Drawing;

using System.Threading.Tasks.Dataflow;

using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.ImageProcessing;

using Hu.MachineVision.Database;
using Hu.MachineVision.Ui;
using Hu.MachineVision.SerialSy;

namespace Hu.MachineVision.VisionPro
{
   public class StationToolBlock
    {
       public int CcdId { get; set; }

       public int BrandId { get; set; }
       public int OfflineImageCycle { get; set; }
       public string VppFileName { get; set; }
       public CogToolBlock MyCogToolBlock { get; set; }

       public  ActionBlock<CcdTerminalIn> VtInBlock { get; set; }

       public AcqFifoToolLoader AcqFifoTool { get; set; }

       public StationToolBlock(int ccdId)
       {
           CcdId = ccdId;
           OfflineImageCycle = 1;
           var db = DbScheme.GetConnection("Data");
           int brandId = db.ExecuteScalar<int>("select data from RunStatus where name = ?", "BrandId");
           BrandId = brandId;
           VppFileName = Helper.VppHelper.FindVpps(CcdId, brandId)[""];          
           LoadVpp();
           VtInBlock = new ActionBlock<CcdTerminalIn>(x => x.RunToolBlock(MyCogToolBlock));

           if(!RunParams.CcdGrabBlock.ContainsKey(CcdId))
           {
               RunParams.CcdGrabBlock[CcdId] = new ActionBlock<int>(x => OnGrabImage(x));
               RunParams.CcdOfflineBlock[CcdId] = new ActionBlock<int>(x => RunOffline(x));
               RunParams.CcdCheckBlock[CcdId] = new ActionBlock<int>(x => CheckResult(x));
           }
       }

       private void CheckResult(int x)
       {
           var db = DbScheme.Connections["Main"];
           int row = db.ExecuteScalar<int>("select row from CcdTerminal where ccdId = ?", CcdId);
           int column = db.ExecuteScalar<int>("select column from CcdTerminal where ccdId = ?", CcdId);

           DataTable tbl = CreateDataTable(row, column);
           FillRawDataTable(tbl, row, column);
           var dgv = StationViews.GetDgv("Data", CcdId);

           if(dgv.InvokeRequired)
           {
               dgv.Invoke(new Action(() => 
               {
                   dgv.DataSource = tbl;
                   dgv.DgvLayout();
               }));
           }
           else
           {
               dgv.DataSource = tbl;
               dgv.DgvLayout();
           
           }


       }

       private void FillRawDataTable(DataTable tbl, int row, int column)
       {
          int columnP1 = 1;
          for(int i = 0; i < row; i++)
          {

              double[] rowData = (MyCogToolBlock.Outputs[string.Format("strRow{0}", i + 1)].Value as string).Split(',').Select(x => double.Parse(x)).ToArray();

              DataRow labelRow = tbl.NewRow();

              tbl.Rows.Add(labelRow);

              DataRow dataRow = tbl.NewRow();
              for(int j = 0; j < column; j++)
              {
                  dataRow[j + columnP1] = rowData[j];
              }

              tbl.Rows.Add(dataRow);
          }
           
       }

       private DataTable CreateDataTable(int row, int column)
       {
           DataTable tbl = new DataTable();
           //DataColumn col = new DataColumn("Id", typeof(Int32));
           //col.AutoIncrement = true;
           //col.AutoIncrementSeed = 1;
           //col.AutoIncrementStep = 1;
           //tbl.Columns.Add(col);

           tbl.Columns.Add("Name", typeof(string));
           int rowCount = row;
           int columnCount = column;
           for (int i = 0; i < columnCount; i++)
           {
               tbl.Columns.Add(string.Format("P{0}", i + 1), typeof(object));
           }
           return tbl;
       }

       private void RunOffline(int x)
       {
           string vppHome = Path.GetDirectoryName(VppFileName);
           DirectoryInfo diImage = new DirectoryInfo(Path.Combine(vppHome, "image"));
           if (!diImage.Exists)
           {
               diImage.Create();
           }

           int imageCount = 1;
           var toolBlock = MyCogToolBlock;

           int offlineImageCycle = x;

           if (offlineImageCycle == 0)
           {
               CogIPOneImageTool[] myCogIPOneImageTools = toolBlock.Tools.OfType<CogIPOneImageTool>().ToArray();
               for (int i = 0; i < imageCount; i++)
               {
                   CogImage8Grey inputImage = myCogIPOneImageTools[i].OutputImage as CogImage8Grey;
                   CcdTerminalIn vtIn = new CcdTerminalIn(inputImage, i);
                   VtInBlock.Post(vtIn);
               }

               return;
           }

           for (int i = 0; i < imageCount; i++)
           {
               var imageName = string.Format("{0}-{1}-{2}.bmp", CcdId, offlineImageCycle, i + 1).Trim('-');
               var imageFile = Path.Combine(diImage.FullName, imageName);
               if (File.Exists(imageFile))
               {
                   Bitmap bmpFile = new Bitmap(imageFile);
                   CogImage8Grey inputImage = new CogImage8Grey(bmpFile);
                   CcdTerminalIn vtIn = new CcdTerminalIn(inputImage, i);
                   VtInBlock.Post(vtIn);
               }
           }
       }

       private void OnGrabImage(int x)
       {
           var grabBlobk = RunParams.CcdGrabBlock[CcdId];
           int imageIndex = x;

           var vIo = VirtualIo.GetDevice(CcdId);

           if(imageIndex == 0)
           {
               for(int i = 0; i < 3; i++)
               {
                   vIo.ResetPort(i);
               }
           }
           double exposure = DbScheme.GetCcdParams(CcdId, BrandId, x, "Exposure");
           CcdTerminalIn vtIn = GrabImage(imageIndex, exposure);
           VtInBlock.Post(vtIn);
           
       }

       private CcdTerminalIn GrabImage(int imageIndex, double exposure)
       {
           CcdTerminalIn vtIn = null;
           var vIo = VirtualIo.GetDevice(CcdId);

           if(vIo != null)
           {
               if(AcqFifoTool == null)
               {
                   AcqFifoTool = new AcqFifoToolLoader(CcdId);
               }

               AcqFifoTool.Aquire(exposure);
               vtIn = new CcdTerminalIn(AcqFifoTool.MyImage, imageIndex);
           }

           return vtIn;           
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

           if(!MyCogToolBlock.Inputs.Contains("InputImage"))
           {
               MyCogToolBlock.Inputs.Add(new CogToolBlockTerminal("InputImage", new CogImage8Grey()));
           }

           if (!MyCogToolBlock.Inputs.Contains("Brand"))
           {
               MyCogToolBlock.Inputs.Add(new CogToolBlockTerminal("Brand", ""));
           }

           if (!MyCogToolBlock.Inputs.Contains("iAcquirePositionIndex"))
           {
               MyCogToolBlock.Inputs.Add(new CogToolBlockTerminal("iAcquirePositionIndex", 0));
           }

           if(!MyCogToolBlock.Inputs.Contains("iViewRow"))
           {
               MyCogToolBlock.Inputs.Add(new CogToolBlockTerminal("iViewRow", 1));
           }

           if (!MyCogToolBlock.Inputs.Contains("iViewColumn"))
           {
               MyCogToolBlock.Inputs.Add(new CogToolBlockTerminal("iViewColumn", 20));
           }          

           return MyCogToolBlock;
       }

       public void Run()
       {
           MyCogToolBlock.Run();
           RunParams.CcdCheckBlock[CcdId].Post(0);
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
           RunParams.CcdOfflineBlock[CcdId].Post(OfflineImageCycle);
           RunParams.CcdCheckBlock[CcdId].Post(0);
       }
    }
}
