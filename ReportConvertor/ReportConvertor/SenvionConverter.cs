using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class SenvionConverter : Converter
    {
        private WorkOrder newWO;
        //private WorkOrder flaggedWO;
        private string site;
        AppInfo info;
        private Dictionary<string, int[]> fieldToCell;
        Dictionary<string, List<string>> fieldNames;
        List<List<string>> records;

        public SenvionConverter(string s, AppInfo i)
        {
            info = i;
            site = s;
            getFieldNames();
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("Main");
            organizeFields();
            int[] loc = fieldToCell["ID#"];
            newWO = new WorkOrder(records[loc[0]+1][loc[1]]);
            newWO.Site = info.getSite(site);
            newWO.Vendor = info.getVendor("Senvion");
            calcLaborHours(fieldToCell["Time"]);
            addDates();
            getDownTime();
            newWO.WorkOrderType = report.checkedVals()[2];
            loc = fieldToCell["Description"];
            newWO.Description = records[loc[0]+2][loc[1]];
            loc = fieldToCell["Comments"];
            newWO.Comments = records[loc[0]][loc[1]+1];
            newWO.Status = "Closed";
            addParts();
        }

        private void getDownTime()
        {
            TimeSpan t = newWO.EndDate - newWO.StartDate;
            newWO.DownTime = t.TotalHours;
        }

        private void addDates()
        {
            int x = fieldToCell["Offline"][0];
            int y = fieldToCell["Offline"][1]+3;
            newWO.StartDate = convertToDate(records[x][y]+","+records[x][y+1]);
            x = fieldToCell["Online"][0];
            y = fieldToCell["Online"][1]+3;
            newWO.EndDate = convertToDate(records[x][y]+","+records[x][y+1]);
            x = fieldToCell["Date"][0];
            y = fieldToCell["Date"][1]+2;
            newWO.OpenDate = convertToDate(records[x][y]);
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
            newWO.ActualHours = result;
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
            List<List<string>> data = info.getVendorData("Senvion");
            fieldNames = new Dictionary<string, List<string>>();
            int i, j;
            List<string> row;
            bool isFieldTable = false;

            for (i = 0; i < data.Count; i++)
            {
                row = data[i];
                if (row[0].Contains("Field Name"))
                {
                    isFieldTable = true;
                    i++;
                }
                if (row[0].Equals(" "))
                {
                    isFieldTable = false;
                    break;
                }
                if (isFieldTable)
                {
                    row = data[i];
                    List<string> fields = new List<string>();
                    for (j = 0; j < row.Count; j++)
                    {
                        if (!row[j].Equals(" "))
                        {
                            fields.Add(row[j].ToLower());
                        }
                    }
                    fieldNames.Add(row[0], fields);
                }
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
                string partID = newWO.Vendor.getPartID(id, qty);
                if (partID != null)
                {
                    newWO.addPart(partID, qty);
                }
                else
                {
                    string description = records[x][y];
                    partID = newWO.Vendor.addNewPart(id, qty, description);
                    newWO.addPart(partID, qty);
                }
                x++;
            }
        }

        public List<WorkOrder> getWorkOrders()
        {
            List<WorkOrder> newWOs = new List<WorkOrder>();
            newWO.createMPulseID();
            newWOs.Add(newWO);
            return newWOs;
        }
    }
}
