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
        public Dictionary<string, WorkOrder> newWO;
        public List<WorkOrder> flaggedWO;

        public Framework()
        {
            info = new AppInfo();
            inputDirectory = info.getFileLoc("Directory");
            newWO = new Dictionary<string, WorkOrder>();
            flaggedWO = new List<WorkOrder>();
        }

        public void start()
        {
            //Getting names of all the files in the input directory
            DirectoryReader dR = new DirectoryReader(inputDirectory);
            Dictionary<string,string[]> files = dR.readDirectory();

            //Reading data from all the files found
            Dictionary<string, Dictionary<string, List<Report>>> ServiceReports = parseFiles(files);
            /*data is organized by location so go through each
             * list and send the data to the right parsers */
            Convertor c = null;
            List<string> keys = ServiceReports["xlsx"].Keys.ToList();
            foreach (string key in keys) {
                //start convertors based on location
                switch (key)
                {
                    case "Highland 1":
                        break;
                    case "Highland North":
                        break;
                    case "Patton":
                        c = new GamesaConvertor(info);
                        break;
                }
                foreach (Report report in ServiceReports["xlsx"][key])
                {
                    Dictionary<string, WorkOrder> woList = c.convertReport(report);
                    addWorkOrders(woList);
                }
                //run convertors
                //
            }
            ExcelFileWriter writer = new ExcelFileWriter(info);
            writer.writeFiles(getListofWO(), null, null);

        }

        private List<WorkOrder> getListofWO()
        {
            List<WorkOrder> workOrders = new List<WorkOrder>();
            List<String> keys = newWO.Keys.ToList();
            foreach (string key in keys)
            {
                workOrders.Add(newWO[key]);
            }
            return workOrders;
        }

        private void addWorkOrders(Dictionary<string, WorkOrder> wo)
        {
            List<string> keys = wo.Keys.ToList();
            foreach (string key in keys)
            {
                if (newWO.ContainsKey(key))
                {
                    flaggedWO.Add(newWO[key]);
                    flaggedWO.Add(wo[key]);
                    wo.Remove(key);
                    newWO.Remove(key);
                }
            }
        }

        private Dictionary<string, Dictionary<string, List<Report>>> parseFiles(Dictionary<string, string[]> fDict)
        {
            Dictionary<string, Dictionary<string, List<Report>>> ServiceReports = new Dictionary<string,Dictionary<string,List<Report>>>();
            FileReader fR = null;
            foreach (string key in fDict.Keys)
            {
                /* Read data from the files based
                 * on the type of file */
                switch (key)
                {
                    case "xlsx":
                        fR = new ExcelFileReader(info);
                        string[] files = fDict[key];
                        ServiceReports.Add(key, fR.readFiles(files));
                        break;
                    case "pdf":
                        fR = new PDFFileReader(info);
                        ServiceReports.Add(key, fR.readFiles(fDict[key]));
                        break;
                    case "html":
                        fR = new HTMLFileReader(info);
                        ServiceReports.Add(key, fR.readFiles(fDict[key]));
                        break;
                    default:
                        //Write code
                        ServiceReports = null;
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
