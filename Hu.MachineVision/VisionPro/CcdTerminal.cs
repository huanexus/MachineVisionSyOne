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
        public string Brand { get; set; }
        public int ImageIndex { get; set; }
        public ICogImage InputImage { get; set; }

        public CcdTerminalIn(ICogImage image, int index = 0)
        {
            InputImage = image;
            ImageIndex = index;
           // Brand = VisionConfig.Brand;
        }

        public CcdTerminalIn(ICogImage image, string brand, int index = 0)
        {
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
        }
    }
}
