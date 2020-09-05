using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.ImageProcessing;

namespace Hu.MachineVision.VisionPro
{
    public class CcdTerminalIn
    {
        public int CcdId { get; set; }
        public string Brand { get; set; }
        public int ImageIndex { get; set; }
        public ICogImage InputImage { get; set; }

        public CcdTerminalIn(int ccdId, ICogImage image, int index = 0)
        {
            CcdId = ccdId;
            InputImage = image;
            ImageIndex = index;
            // Brand = VisionConfig.Brand;
        }

        public CcdTerminalIn(int ccdId, ICogImage image, string brand, int index = 0)
        {
            CcdId = ccdId;
            InputImage = image;
            Brand = brand;
            ImageIndex = index;
        }

        public void RunToolBlock(CogToolBlock toolBlock)
        {
            RunToolBlock(toolBlock, InputImage, Brand, ImageIndex);
        }
        public void RunToolBlock(CogToolBlock toolBlock, ICogImage image, string brand, int index)
        {
            toolBlock.Inputs["InputImage"].Value = image;
            toolBlock.Inputs["Brand"].Value = brand;
            toolBlock.Inputs["iAcquirePositionIndex"].Value = index;
            toolBlock.Run();

            RunParams.CcdDisplayBlock[CcdId].Post(1);
            RunParams.CcdCheckBlock[CcdId].Post(0);
        }       
    }
}
