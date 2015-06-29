﻿using System;
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
        public List<WorkOrder> flaggedWO;
        public List<Part> newParts;

        public Framework()
        {
            info = new AppInfo();
            inputDirectory = info.getFileLoc("Directory");
            newWO = new Dictionary<string, WorkOrder>();
            flaggedWO = new List<WorkOrder>();
            newParts = new List<Part>();
        }

        public void start(Main m)
        {
            //Getting names of all the files in the input directory
            DirectoryReader dR = new DirectoryReader(inputDirectory);
            Dictionary<string,string[]> files = dR.readDirectory();

            //Reading data from all the files found
            Dictionary<string, Dictionary<string, List<Report>>> ServiceReports = parseFiles(files);
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
                            c = new NordexConverter("Highland 1", info);
                            break;
                        case "Highland North":
                            c = new NordexConverter("Highland North", info);
                            break;
                        case "Patton":
                            c = new GamesaConverter(info);
                            break;
                        case "Twin Ridges":
                            break;
                        case "Big Sky":
                            break;
                        case "Howard":
                            break;
                        case "Mustang Hills":
                            break;
                    }
                    foreach (Report report in ServiceReports[sKey][key])
                    {
                        c.convertReport(report);
                        addNewParts(c.getNewParts());
                    }
                    addWorkOrders(c.getWorkOrders());
                }
            }
            
            ExcelFileWriter writer = new ExcelFileWriter(info);
            writer.writeFiles(getListofWO(), newParts, null);
            m.showDoneMessage();
            m.activateForm();
        }

        private void addNewParts(List<Part> parts)
        {
            if (newParts.Count == 0)
            {
                foreach (Part part in parts)
                {
                    newParts.Add(part);
                }
            }
            else
            {
                foreach (Part part in parts)
                {
                    if (!newParts.Contains(part))
                    {
                        newParts.Add(part);
                    }
                }
            }
        }

        private List<WorkOrder> getListofWO()
        {
            List<WorkOrder> workOrders = new List<WorkOrder>();
            List<String> keys = newWO.Keys.ToList();
            foreach (string key in keys)
            {
                newWO[key].createMPulseID();
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
                else
                {
                    newWO.Add(key, wo[key]);
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
