using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class Framework
    {
        public AppInfo info;
        private string inputDirectory;
        public Dictionary<string, WorkOrder> newWO;
        public List<WorkOrder> flaggedWO;
        private string archiveDirectory;
        private PartsTable partsTable;
        private AssetTable assetTable;

        public Framework(AppInfo i)
        {
            info = i;
            inputDirectory = info.getFileLoc("Input");
            archiveDirectory = info.getFileLoc("Archive");
            partsTable = new PartsTable(info.getAllVendors(), info.getFileLoc("Parts"));
            newWO = new Dictionary<string, WorkOrder>();
            flaggedWO = new List<WorkOrder>();
            assetTable = new AssetTable(info.getSites(), info.getFileLoc("Assets"));
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
                                    c = new NordexExcelConverter(key, info, partsTable, assetTable);
                                }
                                else
                                {
                                    c = new NordexConverter(key, info, partsTable, assetTable);
                                }
                                break;
                            case "Highland North":
                                if (sKey.Equals("xls"))
                                {
                                    c = new NordexExcelConverter(key, info, partsTable, assetTable);
                                }
                                else
                                {
                                    c = new NordexConverter(key, info, partsTable, assetTable);
                                }
                                break;
                            case "Patton":
                                c = new GamesaConverter(info, partsTable, assetTable);
                                break;
                            case "Twin Ridges":
                                if (sKey.Equals("xlsm"))
                                {
                                    c = new EVPConverter(key, info, partsTable, assetTable);
                                }
                                else
                                {
                                    c = new SenvionConverter(key, info, partsTable, assetTable);
                                }
                                break;
                            case "Big Sky":
                                c = new SuzlonConverter(key, info, partsTable, assetTable);
                                break;
                            case "Howard":
                                if (sKey.Equals("xlsm"))
                                {
                                    c = new EVPConverter(key, info, partsTable, assetTable);
                                }
                                else
                                {
                                    c = new SenvionConverter(key, info, partsTable, assetTable);
                                }
                                break;
                            case "Mustang Hills":
                                c = new VestasConverter(key, info, partsTable, assetTable);
                                break;
                        }
                        foreach (Report report in ServiceReports[sKey][key])
                        {
                            c.convertReport(report);
                            addWorkOrders(c.getWorkOrders());
                            addFlaggedWO(c.getFlaggedWO());
                        }
                    }
                }
                ExcelFileWriter writer = new ExcelFileWriter(info);
                List<Part> newParts = getParts();
                writer.writeFiles(getListofWO(), newParts, flaggedWO, assetTable.getUnlinkedAssets());
                m.showDoneMessage();
            }
            m.activateForm();
        }

        private List<Part> getParts()
        {
            List<Part> newParts = new List<Part>();
            List<Vendor> vendors = info.getAllVendors();
            newParts = partsTable.getNewParts();
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
            if (workOrders != null)
            {
                foreach (WorkOrder wo in workOrders)
                {
                    string id = wo.mPulseID;
                    if (newWO.ContainsKey(id))
                    {
                        List<WorkOrder> wos = new List<WorkOrder>();
                        wos.Add(wo);
                        wos.Add(newWO[id]);
                        flaggedWO.Add(wo);
                        newWO.Remove(id);
                    }
                    else
                    {
                        newWO.Add(id, wo);
                    }
                }
            }
        }

        private void addFlaggedWO(List<WorkOrder> wos)
        {
            if (wos != null)
            {
                foreach (WorkOrder wo in wos)
                {
                    if (wo != null)
                    {
                        flaggedWO.Add(wo);
                    }
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
                        break;
                    case "xls":
                        fR = new XLSFileReader(info);
                        break;
                    case "pdf":
                        fR = new PDFFileReader(info);
                        break;
                    case "html":
                        fR = new HTMLFileReader(info);
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

        public void archiveOutput(Main m)
        {
            string oDir = info.getFileLoc("Output");
            string aDir = info.getFileLoc("Archive");
            Archive a = new Archive(oDir, aDir);
            a.startArchive();
            m.activateForm();
        }
    }
}
