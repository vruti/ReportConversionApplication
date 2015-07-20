using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class SuzlonConverter : Converter
    {
        private WorkOrder wo;
        //private WorkOrder flaggedWO;
        private AppInfo info;
        private Site site;
        private Dictionary<string, int[]> fieldToCell;
        private Dictionary<string, List<string>> fieldNames;
        private List<List<string>> records;

        public SuzlonConverter(String s, AppInfo i)
        {
            info = i;
            site = info.getSite(s);
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("Main");
            organizeFields();
            wo = new WorkOrder("none");
            wo.Site = site;
            wo.Vendor = site.Contractor;
            wo.Status = "Closed";
            int[] loc = fieldToCell["Start Date"];
            wo.StartDate = Convert.ToDateTime(records[loc[0]][loc[1]+1]);
            loc = fieldToCell["End Date"];
            wo.EndDate = Convert.ToDateTime(records[loc[0]][loc[1]+1]);
            wo.OpenDate = wo.StartDate;
            loc = fieldToCell["Outage Type"];
            wo.OutageType = records[loc[0]][loc[1]+1];
            loc = fieldToCell["Priority"];
            wo.Priority = records[loc[0]][loc[1]+1];
            loc = fieldToCell["Actual Hours"];
            wo.ActualHours = Convert.ToDouble(records[loc[0]][loc[1]+1]);
            loc = fieldToCell["Down Time"];
            wo.DownTime = Convert.ToDouble(records[loc[0]][loc[1]+1]);
            addParts();
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
                    if (key != null && !fieldToCell.ContainsKey(key))
                    {
                        fieldToCell.Add(key, new int[] { i, j });
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
            int[] loc = fieldToCell["Parts"];
            int i = loc[0];
            int idLoc = loc[1]+1;
            int qLoc = idLoc+1;
            int desLoc = qLoc+2;

            while (!records[i][idLoc].Equals(" "))
            {
                string id = records[i][idLoc];
                int qty = Convert.ToInt32(records[i][qLoc]);
                string partID = wo.Vendor.getPartID(id, qty);
                if (partID != null)
                {
                    wo.addPart(partID, qty);
                }
                else
                {
                    string description = records[i][desLoc];
                    partID = wo.Vendor.addNewPart(id, qty, description);
                    wo.addPart(partID, qty);
                }
            }
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
