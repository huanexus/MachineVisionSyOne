using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Text.RegularExpressions;

using Hu.MachineVision.Database;

namespace Hu.MachineVision.Helper
{
    public static class VppHelper
    {
        public static Dictionary<string, string> FindVpps(int ccdId, int brandId)
        {
            var db = DbScheme.GetConnection("Main");
            var vppFiles = new Dictionary<string, string>();

            string brand = db.ExecuteScalar<string>("select brand from CcdBrand where brandId = ?", brandId);     

            CcdVpp myCcdVpp = db.Query<CcdVpp>("select * from CcdVpp where ccdId = ?", ccdId).First();
            var home = new DirectoryInfo(Path.Combine(myCcdVpp.VppHome, brand));
            var files = home.GetFiles("*.vpp");

            string pattern = string.Format("_(?<ccdName>{0})(.*?)(?<partId>{1})$", myCcdVpp.NamePattern, myCcdVpp.PartPattern);

            Regex patternVpp = new Regex(pattern);

            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file.FullName);
                if (patternVpp.IsMatch(name))
                {
                    var match = patternVpp.Match(name);
                    var groups = match.Groups;
                    vppFiles[groups["partId"].Value] = file.FullName;
                }
            }

            return vppFiles;
        }

    }
}
