using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportConverter
{
    public class Framework
    {
        public AppInfo info;
        private string inputDirectory;
        public Dictionary<string, WorkOrder> newWO;
        public Dictionary<string, List<WorkOrder>> flaggedWO;
        private string archiveDirectory;
        private PartsTable partsTable;
        private AssetTable assetTable;
        private WOTable woTable;
        private ProgressBar pBar;

        public Framework(AppInfo i)
        {
            info = i;
            inputDirectory = info.getFileLoc("Input");
            archiveDirectory = info.getFileLoc("Archive");
            partsTable = new PartsTable(info.getAllVendors(), info.getFileLoc("Parts"));
            assetTable = new AssetTable(info.getSites(), info.getFileLoc("Assets"));
            woTable = new WOTable(info.getFileLoc("WOHistory"));
        }

        public void start(Main m, ProgressBar pB)
        {
            newWO = new Dictionary<string, WorkOrder>();
            flaggedWO = new Dictionary<string, List<WorkOrder>>();
            archiveOutput(m);
            pBar = pB;
            //Getting names of all the files in the input directory
            DirectoryReader dR = new DirectoryReader(inputDirectory, archiveDirectory);
            Dictionary<string,string[]> files = dR.readDirectory();
            if (files.ToArray().Count() > 0)
            {
                getPBarCount(files);
                pBar.Value = 0;
                pBar.Visible = true;
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
                                    c = new NordexExcelConverter(key, info, partsTable, assetTable, woTable);
                                }
                                else
                                {
                                    c = new NordexConverter(key, info, partsTable, assetTable, woTable);
                                }
                                break;
                            case "Highland North":
                                if (sKey.Equals("xls"))
                                {
                                    c = new NordexExcelConverter(key, info, partsTable, assetTable, woTable);
                                }
                                else
                                {
                                    c = new NordexConverter(key, info, partsTable, assetTable, woTable);
                                }
                                break;
                            case "Patton":
                                c = new GamesaConverter(info, partsTable, assetTable, woTable);
                                break;
                            case "Twin Ridges":
                                if (sKey.Equals("xlsm"))
                                {
                                    c = new EVPConverter(key, info, partsTable, assetTable, woTable);
                                }
                                else
                                {
                                    c = new SenvionConverter(key, info, partsTable, assetTable, woTable);
                                }
                                break;
                            case "Big Sky":
                                c = new SuzlonConverter(key, info, partsTable, assetTable, woTable);
                                break;
                            case "Howard":
                                if (sKey.Equals("xlsm"))
                                {
                                    c = new EVPConverter(key, info, partsTable, assetTable, woTable);
                                }
                                else
                                {
                                    c = new SenvionConverter(key, info, partsTable, assetTable, woTable);
                                }
                                break;
                            case "Mustang Hills":
                                c = new VestasConverter(key, info, partsTable, assetTable, woTable);
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
                writer.writeFiles(getListofWO(), newParts, getFlaggedWOs(), assetTable.getUnlinkedAssets());
                pBar.PerformStep();
                pBar.Visible = false;
                m.showMessage("Conversion Complete!");
            }
            else
            {
                m.showMessage("No Files in Input folder");
            }
            //m.activateForm();
            System.Diagnostics.Process.Start(info.getFileLoc("Output"));
        }

        private void getPBarCount(Dictionary<string, string[]> files)
        {
            List<string> keys = files.Keys.ToList();
            int count = 0;
            foreach (string key in keys)
            {
                count += files[key].Length;
            }
            count += 1;
            pBar.Maximum = count;
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
                    string id = wo.OriginalID;
                    if (newWO.ContainsKey(id))
                    {
                        if (flaggedWO.ContainsKey(id))
                        {
                            flaggedWO[id].Add(wo);
                            flaggedWO[id].Add(newWO[id]);
                        }
                        else
                        {
                            List<WorkOrder> wos = new List<WorkOrder>();
                            wos.Add(wo);
                            wos.Add(newWO[id]);
                            flaggedWO.Add(id, wos);
                        }
                        newWO.Remove(id);
                    }
                    else if (flaggedWO.ContainsKey(id))
                    {
                        flaggedWO[id].Add(wo);
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
                        string id = wo.OriginalID;
                        if (flaggedWO.ContainsKey(id))
                        {
                            flaggedWO[id].Add(wo);
                        }
                        else
                        {
                            List<WorkOrder> woList = new List<WorkOrder>();
                            woList.Add(wo);
                            flaggedWO.Add(id, woList);
                        }
                    }
                }
            }
        }

        private List<WorkOrder> getFlaggedWOs()
        {
            List<WorkOrder> flagged = new List<WorkOrder>();
            List<string> keys = flaggedWO.Keys.ToList();
            foreach (string key in keys)
            {
                flagged.AddRange(flaggedWO[key]);
            }
            return flagged;
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
                        fR = new ExcelFileReader(info, pBar);
                        break;
                    case "xlsm":
                        fR = new ExcelFileReader(info, pBar);
                        break;
                    case "xls":
                        fR = new XLSFileReader(info, pBar);
                        break;
                    case "pdf":
                        fR = new PDFFileReader(info, pBar);
                        break;
                    case "html":
                        fR = new HTMLFileReader(info, pBar);
                        break;
                    default:
                        ServiceReports = null;
                        break;
                }
                ServiceReports.Add(key, fR.readFiles(files));
                //pBar.PerformStep();
            }
            return ServiceReports;
        }
 
        public void changeInputDirectory(string dir)
        {
            inputDirectory = dir;
        }

        public string archiveOutput(Main m)
        {
            string oDir = info.getFileLoc("Output");
            string aDir = info.getFileLoc("Archive");
            Archive a = new Archive(oDir, aDir);
            string message = a.startArchive();
            m.activateForm();
            return message;
        }
    }
}
