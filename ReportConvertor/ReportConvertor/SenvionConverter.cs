using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class SenvionConverter : Converter
    {
        private WorkOrder wo;
        private WorkOrder flaggedWO;
        private string site;
        private AppInfo info;
        private Dictionary<string, int[]> fieldToCell;
        private Dictionary<string, List<string>> fieldNames;
        private List<List<string>> records;
        private List<List<string>> vendorData;
        private Dictionary<string, int> tableLoc;
        private Dictionary<string, List<string>> table;
        private PartsTable partsTable;

        public SenvionConverter(string s, AppInfo i, PartsTable p)
        {
            info = i;
            site = s;
            partsTable = p;
            vendorData = info.getVendorData("Senvion");
            tableLoc = new Dictionary<string, int>();
            getTableLoc();
            getFieldNames();
            getTableData();
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("Main");
            organizeFields();
            int[] loc = fieldToCell["ID#"];
            wo = new WorkOrder(records[loc[0]+1][loc[1]]);
            wo.Site = info.getSite(site);
            wo.Vendor = info.getVendor("Senvion");
            calcLaborHours(fieldToCell["Time"]);
            addDates();
            getDownTime();
            wo.WorkOrderType = report.checkedVals()[2];
            List<string> taskInfo = table[wo.WorkOrderType];
            wo.OutageType = taskInfo[0];
            wo.Priority = taskInfo[1];
            wo.Planning = taskInfo[2];
            wo.UnplannedType = taskInfo[3];
            wo.TaskID = taskInfo[4];
            loc = fieldToCell["Description"];
            wo.Description = records[loc[0]+2][loc[1]];
            loc = fieldToCell["Comments"];
            wo.Comments = records[loc[0]][loc[1]+1];
            wo.Status = "Closed";
            addParts();
            Validator v = new Validator();
            if (!v.isValid(wo))
            {
                flaggedWO = wo;
                wo = null;
            }
        }
        
        private void getTableData()
        {
            table = new Dictionary<string, List<string>>();
            int start = tableLoc["Table"] - 1;
            int len = Convert.ToInt32(vendorData[start][1]);
            start+=2;
            int cols;

            for (int i = start; i < start + len; i++)
            {
                List<string> line = vendorData[i];
                cols = line.Count;
                List<string> row = new List<string>();
                for (int j = 1; j < cols; j++)
                {
                    row.Add(line[j]);
                }
                table.Add(line[0], row);
            }
        }

        private void getDownTime()
        {
            TimeSpan t = wo.EndDate - wo.StartDate;
            wo.DownTime = t.TotalHours;
        }

        private void addDates()
        {
            int x = fieldToCell["Offline"][0];
            int y = fieldToCell["Offline"][1]+3;
            wo.StartDate = convertToDate(records[x][y]+","+records[x][y+1]);
            x = fieldToCell["Online"][0];
            y = fieldToCell["Online"][1]+3;
            wo.EndDate = convertToDate(records[x][y]+","+records[x][y+1]);
            x = fieldToCell["Date"][0];
            y = fieldToCell["Date"][1]+2;
            wo.OpenDate = convertToDate(records[x][y]);
        }

        private DateTime convertToDate(string s)
        {
            string val = s.Trim();
            DateTime result = Convert.ToDateTime(val);
            return result;
        }

        private void calcLaborHours(int[] loc)
        {
            double result = 0;
            int i = loc[0] + 2, j = loc[1];
            while (!records[i][j].Equals(" "))
            {
                double from = convertToTime(records[i][j]);
                double until = convertToTime(records[i][j + 1]);
                double timeTaken = until - from;
                result += timeTaken;
                i++;
            }
            wo.ActualHours = result;
        }

        private double convertToTime(string s)
        {
            int cLoc = s.IndexOf(":");
            double hour = Convert.ToDouble(s.Substring(0, cLoc));
            double min = Convert.ToDouble(s.Substring(cLoc + 1, 2));
            double result = hour + (min / 60);
            return result;
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
                    key = isField(row[j]);
                    if (key!= null && !fieldToCell.ContainsKey(key))
                    {
                        fieldToCell.Add(key, new int[] {i, j});
                    }
                }
            }
        }

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
                        return key;
                    }
                }
            }
            return null;
        }

        private void getFieldNames()
        {
            fieldNames = new Dictionary<string, List<string>>();
            int start = tableLoc["Fields"]-1;
            int len = Convert.ToInt32(vendorData[start][2]);
            start++;
            int cols;
            List<string> row;

            for (int i = start; i < start + len; i++)
            {
                cols = vendorData[i].Count;
                row = new List<string>();
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

        private void getTableLoc()
        {
            
            int i = 1;
            int loc;
            string n;

            while (!vendorData[i][0].Equals(" "))
            {
                loc = Convert.ToInt32(vendorData[i][1]);
                n = vendorData[i][0];
                tableLoc.Add(n.Trim(), loc);
                i++;
            }
        }

        private void addParts()
        {
            int x = fieldToCell["Parts"][0] + 3;
            int y = fieldToCell["Parts"][1] + 1;

            while (!records[x][y].Equals(""))
            {
                string id = records[x][y + 5];
                int qty = Convert.ToInt32(records[x][y + 6]);
                string partID = partsTable.getPartID(id, wo.Vendor.Name, qty);
                if (partID == null)
                {
                    string description = records[x][y];
                    partID = partsTable.addNewPart(id, qty, description, wo.Vendor);
                }
                wo.addPart(partID, qty);
                x++;
            }
        }

        public List<WorkOrder> getFlaggedWO()
        {
            if (flaggedWO != null)
            {
                List<WorkOrder> flaggedWOs = new List<WorkOrder>();
                flaggedWO.createMPulseID();
                flaggedWOs.Add(flaggedWO);
                return flaggedWOs;
            }
            return null;
        }

        public List<WorkOrder> getWorkOrders()
        {
            if (wo != null)
            {
                List<WorkOrder> wos = new List<WorkOrder>();
                wo.createMPulseID();
                wos.Add(wo);
                return wos;
            }
            return null;
        }
    }
}
