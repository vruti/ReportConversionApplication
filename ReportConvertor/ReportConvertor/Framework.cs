using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class Framework
    {
        private AppInfo info;
        private string inputDirectory;
        public Dictionary<string, WorkOrder> newWO;
        public Dictionary<string, List<WorkOrder>> flaggedWO;
        //public List<Part> newParts;
        private string archiveDirectory;

        public Framework()
        {
            info = new AppInfo();
            inputDirectory = info.getFileLoc("Directory");
            archiveDirectory = info.getFileLoc("Archive");
            newWO = new Dictionary<string, WorkOrder>();
            flaggedWO = new Dictionary<string,List<WorkOrder>>();
            //newParts = new List<Part>();
        }

        public void start(Main m)
        {
            //Getting names of all the files in the input directory
            DirectoryReader dR = new DirectoryReader(inputDirectory, archiveDirectory);
            Dictionary<string,string[]> files = dR.readDirectory();
            if (files.ToArray().Count() > 0)
            {
                //Reading data from all the files found
                Dictionary<string, Dictionary<string, List<Report>>> ServiceReports = parseFiles(files, dR);
                /*data is organized by location so go through each
                 * list and send the data to the right parsers */
                Converter c = null;
                List<string> sKeys = ServiceReports.Keys.ToList();
                foreach (string sKey in sKeys)
                {
                    List<string> keys = ServiceReports[sKey].Keys.ToList();
                    foreach (string key in keys)
                    {
                        switch (key)
                        {
                            case "Highland 1":
                                if (sKey.Equals("xls"))
                                {
                                    c = new NordexExcelConverter(key, info);
                                }
                                else
                                {
                                    c = new NordexConverter(key, info);
                                }
                                break;
                            case "Highland North":
                                if (sKey.Equals("xls"))
                                {
                                    c = new NordexExcelConverter(key, info);
                                }
                                else
                                {
                                    c = new NordexConverter(key, info);
                                }
                                break;
                            case "Patton":
                                c = new GamesaConverter(info);
                                break;
                            case "Twin Ridges":
                                if (sKey.Equals("xlsm"))
                                {
                                    c = new EVPConverter(key, info);
                                }
                                else
                                {
                                    c = new SenvionConverter(key, info);
                                }
                                break;
                            case "Big Sky":
                                //c = new VestasConverter(info);
                                break;
                            case "Howard":
                                //c = new SenvionConverter(info, "Howard");
                                break;
                            case "Mustang Hills":
                                //c = new SuzlonConverter(info);
                                break;
                        }
                        foreach (Report report in ServiceReports[sKey][key])
                        {
                            c.convertReport(report);
                            addWorkOrders(c.getWorkOrders());
                        }
                    }
                }

                ExcelFileWriter writer = new ExcelFileWriter(info);
                List<Part> newParts = getParts();
                writer.writeFiles(getListofWO(), newParts, null);
                m.showDoneMessage();
            }
            m.activateForm();
        }

        private List<Part> getParts()
        {
            List<Part> newParts = new List<Part>();
            List<Vendor> vendors = info.getAllVendors();
            foreach (Vendor v in vendors)
            {
                newParts.AddRange(v.getNewParts());
            }
            return newParts;
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

        private void addWorkOrders(List<WorkOrder> workOrders)
        {
            foreach (WorkOrder wo in workOrders)
            {
                string id = wo.mPulseID;
                if (newWO.ContainsKey(id))
                {
                    List<WorkOrder> wos = new List<WorkOrder>();
                    wos.Add(wo);
                    wos.Add(newWO[id]);
                    flaggedWO.Add(id, wos);
                    newWO.Remove(id);
                }
                else
                {
                    newWO.Add(id, wo);
                }
            }
        }

        private Dictionary<string, Dictionary<string, List<Report>>> parseFiles(Dictionary<string, string[]> fDict, DirectoryReader dR)
        {
            Dictionary<string, Dictionary<string, List<Report>>> ServiceReports = new Dictionary<string,Dictionary<string,List<Report>>>();
            FileReader fR = null;
            foreach (string key in fDict.Keys)
            {
                string[] files = fDict[key];
                /* Read data from the files based
                 * on the type of file */
                switch (key)
                {
                    case "xlsx":
                        fR = new ExcelFileReader(info);
                        
                        break;
                    case "xlsm":
                        fR = new ExcelFileReader(info);
                        //ServiceReports.Add(key, fR.readFiles(files));
                        break;
                    case "xls":
                        //fR = new XlsFileReader(info);
                        //ServiceReports.Add(key, fR.readFile(files))
                        break;
                    case "pdf":
                        fR = new PDFFileReader(info);
                        //ServiceReports.Add(key, fR.readFiles(files));
                        break;
                    case "html":
                        fR = new HTMLFileReader(info);
                        //ServiceReports.Add(key, fR.readFiles(files));
                        break;
                    default:
                        ServiceReports = null;
                        break;
                }
                ServiceReports.Add(key, fR.readFiles(files));
                dR.moveFiles(files);
            }
            return ServiceReports;
        }
 
        public void changeInputDirectory(string dir)
        {
            inputDirectory = dir;
        }
    }
}
