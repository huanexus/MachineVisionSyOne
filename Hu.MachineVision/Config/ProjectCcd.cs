using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;


namespace Hu.MachineVision.Config
{
    public class ProjectCcd
    {
        public int CcdCount { get; set; }
        public int BrandCount { get; set; }
        public List<int> PartCount { get; set; }

        public int BrandId { get; set; }

        public ProjectCcd(int ccdCount, int brandCount, params int[] parts)
        {
            CcdCount = ccdCount;
            BrandCount = brandCount;
            PartCount = Enumerable.Repeat(1, ccdCount).ToList();

            BrandId = 0;

            for(int i = 0; i < parts.Count(); i++)
            {
                if(i < CcdCount)
                {
                    PartCount[i] = parts[i];
                }                
            }
        }

    }   

}
