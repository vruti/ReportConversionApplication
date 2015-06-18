using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public class Framework
    {
        private AppInfo info;
        private string inputDirectory;

        public Framework()
        {
            info = new AppInfo();
            inputDirectory = info.getFileLoc("Directory");
        }

        public void start()
        {
            //Getting names of all the files in the input directory
            DirectoryReader dR = new DirectoryReader(inputDirectory);
            Dictionary<string,string[]> files = dR.readDirectory();

            //Reading data from all the files found
            Dictionary<string, List<Report>> ServiceReports = parseFiles(files);
            /*data is organized by location so go through each
             * list and send the data to the right parsers */
            foreach (var pair in ServiceReports) {
                //start convertors based on location
                //run convertors
                //
            }
        }

        private Dictionary<string, List<Report>> parseFiles(Dictionary<string, string[]> fDict) {
            Dictionary<string, List<Report>> ServiceReports = null;
            FileReader fR = null;
            foreach (string key in fDict.Keys)
            {
                /* Read data from the files based
                 * on the type of file */
                switch (key)
                {
                    case "xlsx":
                        fR = new ExcelFileReader(info);
                        break;
                    case "pdf":
                        fR = new PDFFileReader(info);
                        break;
                    case "html":
                        fR = new HTMLFileReader(info);
                        break;
                    default:
                        //Write code
                        ServiceReports = null;
                        break;
                }
                ServiceReports = fR.readFiles(fDict[key]);
            }
            return ServiceReports;
        }
 
        public void changeInputDirectory(string dir)
        {
            inputDirectory = dir;
        }
    }
}
