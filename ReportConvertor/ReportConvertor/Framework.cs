using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public class Framework
    {
        private string inputDirectory = "C://Documents//test c#";

        public void start()
        {
            DirectoryReader dR = new DirectoryReader(inputDirectory);
            Dictionary<string,string[]> files = dR.readDirectory();
            Dictionary<string, List<Report>> ServiceReports = parseFiles(files);
            foreach (var pair in ServiceReports) {
                //start convertors based on location
                //run convertors
                //
            }
        }

        private Dictionary<string, List<Report>> parseFiles(Dictionary<string, string[]> fDict) {
            Dictionary<string, List<Report>> ServiceReports = null;
            foreach (string key in fDict.Keys)
            {
                switch (key)
                {
                    case "xlsx":
                        FileReader fR = new ExcelFileReader();
                        ServiceReports = fR.readFiles(fDict[key]);
                        break;
                    case "pdf":
                        //Write code
                        break;
                    default:
                        //Write code
                        break;
                }
            }
            return ServiceReports;
        }
 
        public void changeInputDirectory(string dir)
        {
            inputDirectory = dir;
        }
    }
}
