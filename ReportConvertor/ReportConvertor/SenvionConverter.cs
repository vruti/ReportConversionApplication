using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class SenvionConverter : Converter
    {
        private Dictionary<string, Part> newParts;
        private WorkOrder newWO;
        private WorkOrder flaggedWO;
        private string site;
        AppInfo info;
        private Dictionary<string, int[]> fieldToCell;
        Dictionary<string, List<string>> fieldNames;
        List<List<string>> records;

        public SenvionConverter(string s, AppInfo i)
        {
            info = i;
            site = s;
            newParts = new Dictionary<string, Part>();
            getFieldNames();
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("main");
            organizeFields();
            newWO = new WorkOrder(records[fieldToCell["ID#"][0]][fieldToCell["ID#"][1]]);
            newWO.Site = info.getSite(site);
            newWO.Vendor = info.getVendor("Senvion");
            calcLaborHours(fieldToCell["Time"]);
            addDates();
            getDownTime();
            newWO.WorkOrderType = report.getWOType();
            newWO.Description = records[fieldToCell["Description"][0]][fieldToCell["Description"][1]];
            newWO.Comments = records[fieldToCell["Comments"][0]][fieldToCell["Comments"][1]];
        }

        private void getDownTime()
        {
            TimeSpan t = newWO.EndDate - newWO.StartDate;
            newWO.DownTime = t.TotalHours;
        }

        private void addDates()
        {
            int x = fieldToCell["Offline"][0]++;
            int y = fieldToCell["Offline"][1]++;
            newWO.StartDate = convertToDate(records[x][y]+","+records[x][y+1]);
            x = fieldToCell["Online"][0]++;
            y = fieldToCell["Online"][1]++;
            newWO.EndDate = convertToDate(records[x][y]+","+records[x][y+1]);
            x = fieldToCell["Date"][0];
            y = fieldToCell["Date"][1]++;
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
            List<int> used = new List<int>();

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
            int i;
            List<string> row;
            bool isFieldTable = false;

            for (i = 0; i < data.Count; i++)
            {
                row = data[i];
                if (row[0].Contains("Field Name"))
                {
                    isFieldTable = true;
                }
                if (row[0].Equals(" "))
                {
                    isFieldTable = false;
                    break;
                }
                if (isFieldTable)
                {
                    i = 0;
                    List<string> fields = new List<string>();
                    while (!row[i].Equals(" "))
                    {
                        fields.Add(row[i]);
                        i++;
                    }
                    fieldNames.Add(row[0], fields);
                }
            }
        }

        private void addParts()
        {
            int x = fieldToCell["Parts"][0] + 2;
            int y = fieldToCell["Parts"][1] + 1;

            while (!records[x][y].Equals(" "))
            {
                string id = records[x][y + 4];
                int qty = Convert.ToInt32(records[x][y + 5]);
                string partID = newWO.Vendor.getPartID(id);
                if (partID == null)
                {
                    if (newParts.ContainsKey(id))
                    {
                        partID = newParts[id].ID;
                    }
                    else
                    {
                        string newID = getNewestPartID();
                        Part p = new Part(id, newWO.Vendor);
                        p.generateID(newID);
                        partID = p.ID;
                        newParts.Add(id, p);
                    }
                }
                newWO.addPart(partID, qty);
            }
        }

        private string getNewestPartID()
        {
            if (newParts.Count > 0)
            {
                List<string> keys = newParts.Keys.ToList();
                int i = keys.Count - 1;
                return keys[i];
            }
            return newWO.Vendor.newestPartID();
        }

        public List<Part> getNewParts()
        {
            List<string> keys = newParts.Keys.ToList();
            List<Part> parts = new List<Part>();
            foreach (string key in keys)
            {
                parts.Add(newParts[key]);
            }
            return parts;
        }

        public List<WorkOrder> getWorkOrders()
        {
            List<WorkOrder> newWOs = new List<WorkOrder>();
            newWOs.Add(newWO);
            return newWOs;
        }
    }
}
