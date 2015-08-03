﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class NordexExcelConverter : Converter
    {
        private Site site;
        private AppInfo info;
        private WorkOrder wo;
        private WorkOrder flaggedWO;
        private List<List<string>> records;
        private Dictionary<string, List<string>> fieldNames;
        private Dictionary<string, int[]> fieldToCell;
        private PartsTable partsTable;
        private List<List<string>> vendorData;
        private AssetTable aTable;


        public NordexExcelConverter(string s, AppInfo i, PartsTable p, AssetTable a)
        {
            site = i.getSite(s);
            info = i;
            partsTable = p;
            aTable = a;
//            vendorData = info.getVendorData("Nordex");
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("Main");
            Vendor ven = info.getVendor("Nordex");
            fieldNames = ven.getFieldNames("Main");
            //getFieldNames();
            organizeFields();

            string id = getID();
            wo = new WorkOrder(id);
            string workOrderType = report.checkedVals()[0];
            List<string> taskInfo = info.getTypeInfo(workOrderType);
            wo.WorkOrderType = taskInfo[0];
            wo.TaskID = taskInfo[1];
            wo.OutageType = taskInfo[2];
            wo.Planning = taskInfo[3];
            wo.UnplannedType = taskInfo[4];
            wo.Priority = taskInfo[5];
            wo.Site = site;
            wo.Vendor = ven;
            wo.Status = "Closed";

            //adding the asset to the work order
            int[] loc = fieldToCell["Asset"];
            string asset = records[loc[0]][loc[1] + 2];
            wo.AssetID = aTable.getAssetID(asset, site.Name);
            addDescription();
            addActualHours();
            addDownTime();
            addMaterials();
            addParts();
            addDates();

            Validator v = new Validator();
            if (!v.isValid(wo))
            {
                flaggedWO = wo;
                wo = null;
            }
        }

        private string getID()
        {
            int[] loc = fieldToCell["ID"];
            int x = loc[0];
            int y = loc[1]+1;
            string id = " ";
            while (id == " ")
            {
                id = records[x][y];
                y++;
            }
            return id;
        }

        private void organizeFields()
        {
            fieldToCell = new Dictionary<string, int[]>();
            string key;

            for (int i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                for (int j = 0; j < row.Count; j++)
                {
                    if (!row[j].Equals(" "))
                    {
                        key = isField(row[j]);
                        if (key != null && !fieldToCell.ContainsKey(key))
                        {
                            fieldToCell.Add(key, new int[] { i, j });
                        }
                    }
                }
            }
        }

        /*
         * This function checks if a value in the 
         * report is a field header
         */
        private string isField(string s)
        {
            List<string> fieldKeys = fieldNames.Keys.ToList();
            string name = s.ToLower();
            foreach (string key in fieldKeys)
            {
                foreach (string field in fieldNames[key])
                {
                    if (name.Contains(field.ToLower()))
                    {
                        fieldNames.Remove(key);
                        return key;
                    }
                }
            }
            return null;
        }
        /*
        private void getFieldNames()
        {
            fieldNames = new Dictionary<string, List<string>>();
            double l = Convert.ToDouble(vendorData[0][2]);
            int len = Convert.ToInt32(l) + 1;
            int cols;
            List<string> row;

            for (int i = 0; i < len; i++)
            {
                row = new List<string>();
                cols = vendorData[i].Count;
                for (int j = 1; j < cols; j++)
                {
                    if (!vendorData[i][j].Equals(" "))
                    {
                        row.Add(vendorData[i][j]);
                    }
                }
                fieldNames.Add(vendorData[i][0], row);
            }
        }
        */
        private void addDownTime()
        {
            int i = fieldToCell["Down Time"][0]+2;
            List<string> row = records[i];
            while (!row[1].Equals(" "))
            {
                int last = fieldToCell["Stoppage"][1];
                string time = row[last].Trim();
                int count = time.IndexOf(":");
                double hour = Convert.ToDouble(time.Substring(0, count));
                double min = Convert.ToDouble(time.Substring(count + 1, 2));
                wo.DownTime += hour + (min / 60);
                i++;
                row = records[i];
            }
        }

        private void addDates()
        {
            int[] loc = fieldToCell["Open Date"];
            wo.OpenDate = Convert.ToDateTime(records[loc[0]][loc[1] + 1]);
            loc = fieldToCell["Start Date"];
            wo.StartDate = Convert.ToDateTime(records[loc[0]+1][loc[1]]);
            loc = fieldToCell["End Date"];
            wo.EndDate = Convert.ToDateTime(records[loc[0] + 1][loc[1]]);
        }

        private void addDescription()
        {
            int x = fieldToCell["Description"][0]+1;
            int y = fieldToCell["Description"][1];
            string d = records[x][y];
            int i = d.IndexOf(".");
            string first;

            if (i > 0)
            {
                first = d.Substring(0, i+1);
            }
            else
            {
                first = d;
            }
            wo.Description = first;
            wo.Comments = d;
        }

        private void addActualHours()
        {
            int start = fieldToCell["Actual Hours"][0]+2;
            int y = fieldToCell["Hrs"][1];
            int end = fieldToCell["Description"][0];
            double total = 0;
            for (int i = start; i < end; i++)
            {
                string time = records[i][y].Trim();
                if (!time.Equals(""))
                {
                    int count = time.IndexOf(":");
                    double hour = Convert.ToDouble(time.Substring(0, count));
                    double min = Convert.ToDouble(time.Substring(count + 1, 2));
                    total += hour + (min / 60);
                }
            }

            wo.ActualHours = total;
        }

        private void addMaterials()
        {
            int start = fieldToCell["Materials"][0]+3;
            int y = fieldToCell["Materials"][1];
            int end = fieldToCell["Parts"][0];

            for (int i = start; i < end; i++)
            {
                string id = records[i][y+2];
                if (!id.Equals(" "))
                {
                    int qty = Convert.ToInt32(records[i][y + 1]);
                    string partID = partsTable.getPartID(id, wo.Vendor.Name, qty);
                    if (partID != null)
                    {
                        wo.addPart(partID, qty);
                    }
                    else
                    {
                        string description = records[i][y + 3];
                        partID = partsTable.addNewPart(id, qty, description, wo.Vendor);
                        wo.addPart(partID, qty);
                    }
                }
            }
        }

        /*Find and add all the parts that are
         * used in the work order*/
        private void addParts()
        {
            int start = fieldToCell["Parts"][0] + 2;
            int y = fieldToCell["Materials"][1];
            int end = fieldToCell["Tools"][0];

            for (int i = start; i < end; i++)
            {
                string id = records[i][y + 2];
                if (!id.Equals(" "))
                {
                    int qty = Convert.ToInt32(records[i][y + 1]);
                    string partID = partsTable.getPartID(id, wo.Vendor.Name, qty);
                    if (partID != null)
                    {
                        wo.addPart(partID, qty);
                    }
                    else
                    {
                        string description = records[i][y + 3];
                        partID = partsTable.addNewPart(id, qty, description, wo.Vendor);
                        wo.addPart(partID, qty);
                    }
                }
            }
        }

        public List<WorkOrder> getWorkOrders()
        {
            List<WorkOrder> wos = new List<WorkOrder>();
            if (wo != null)
            {
                wo.createMPulseID();
                wos.Add(wo);
            }
            return wos;
        }

        public List<WorkOrder> getFlaggedWO()
        {
            List<WorkOrder> flagged = new List<WorkOrder>();
            if (flaggedWO != null)
            {
                flaggedWO.createMPulseID();
                flagged.Add(flaggedWO);
            }
            return flagged;
        }
    }
}
