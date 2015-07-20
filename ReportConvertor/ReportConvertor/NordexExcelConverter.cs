using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class NordexExcelConverter : Converter
    {
        private string site;
        private AppInfo info;
        private WorkOrder wo;
        //private WorkOrder flaggedWO;
        private Dictionary<string, Part> newParts;
        private List<List<string>> records;
        private Dictionary<string, List<string>> fieldNames;
        private Dictionary<string, int[]> fieldToCell;

        public NordexExcelConverter(string s, AppInfo i)
        {
            newParts = new Dictionary<string, Part>();
            site = s;
            info = i;
            
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("Main");
            getFieldNames();
            organizeFields();
            string id = getID();
            wo = new WorkOrder(id);
            wo.WorkOrderType = report.checkedVals()[0];
            wo.Site = info.getSite(site);
            wo.Vendor = info.getVendor("Nordex");
            wo.Status = "Closed";
            addDescription();
            addActualHours();
            addDownTime();
            addMaterials();
            addParts();
            addDates();
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

        private void getFieldNames()
        {
            List<List<string>> data = info.getVendorData("Nordex");
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
                    for (j = 1; j < row.Count; j++)
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
                int count = time.IndexOf(":");
                double hour = Convert.ToDouble(time.Substring(0, count));
                double min = Convert.ToDouble(time.Substring(count+1, 2));
                total += hour + (min / 60);
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
                    string partID = wo.Vendor.getPartID(id, qty);
                    if (partID != null)
                    {
                        wo.addPart(partID, qty);
                    }
                    else
                    {
                        string description = records[i][y + 3];
                        partID = wo.Vendor.addNewPart(id, qty, description);
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
                    string partID = wo.Vendor.getPartID(id, qty);
                    if (partID != null)
                    {
                        wo.addPart(partID, qty);
                    }
                    else
                    {
                        string description = records[i][y + 3];
                        partID = wo.Vendor.addNewPart(id, qty, description);
                        wo.addPart(partID, qty);
                    }
                }
            }
        }

        /*Returns a list of the new parts in the
         * work order so that they can be put into 
         * the output file for upload*/
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
            wo.createMPulseID();
            newWOs.Add(wo);
            return newWOs;
        }
    }
}
