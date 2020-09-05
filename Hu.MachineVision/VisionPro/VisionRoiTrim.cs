using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cognex.VisionPro;
using Cognex.VisionPro.ImageProcessing;

using Hu.MachineVision.Database;



namespace Hu.MachineVision.VisionPro
{
   public class VisionRoi
    {
       public int CcdId { get; set; }
       public int BrandId { get; set; }
       public int ImageIndex { get; set; }

       public CcdRoi Roi { get; set; }

       public CogImage8Grey DestinationImage { get; set; }
       public CogCopyRegionTool MyCogCopyRegionTool { get; set; }

       public VisionRoi(int ccdId)
       {
           CcdId = ccdId;
           BrandId = DbHelper.GetRunStatus("BrandId");
           ImageIndex = 0;

           var db = DbScheme.Connections["Main"];
           Roi = db.Query<CcdRoi>("select * from CcdRoi where ccdId = ? and brandId = ? and imageIndex = ?", CcdId, BrandId, ImageIndex).First();
           MyCogCopyRegionTool = new CogCopyRegionTool();
           DestinationImage = new CogImage8Grey(Roi.Width, Roi.Height);
           MyCogCopyRegionTool.Region = new CogRectangleAffine();
       }

       public CogImage8Grey Trim(CogImage8Grey inputImag)
       {
           if((inputImag.Width !=  Roi.Width) || (inputImag.Height != Roi.Height))
           {
               DestinationImage = new CogImage8Grey(Roi.Width, Roi.Height);
               MyCogCopyRegionTool.InputImage = inputImag;
               MyCogCopyRegionTool.DestinationImage = DestinationImage;
               MyCogCopyRegionTool.RunParams.ImageAlignmentEnabled = true;
               MyCogCopyRegionTool.RunParams.InputImageAlignmentX = Roi.X0;
               MyCogCopyRegionTool.RunParams.InputImageAlignmentY = Roi.Y0;
               MyCogCopyRegionTool.RunParams.DestinationImageAlignmentX = 0;
               MyCogCopyRegionTool.RunParams.DestinationImageAlignmentY = 0;
               (MyCogCopyRegionTool.Region as CogRectangleAffine).CenterX = Roi.X0 + Roi.Width / 2;
               (MyCogCopyRegionTool.Region as CogRectangleAffine).CenterY = Roi.Y0 + Roi.Height / 2;
               (MyCogCopyRegionTool.Region as CogRectangleAffine).SideXLength = Roi.Width;
               (MyCogCopyRegionTool.Region as CogRectangleAffine).SideYLength = Roi.Height;
               
               MyCogCopyRegionTool.Run();
               return DestinationImage;
           }

           return inputImag;           
       }
    }
}
