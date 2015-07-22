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
        private PartsTable partsTable;

        public SuzlonConverter(String s, AppInfo i, PartsTable p)
        {
            info = i;
            site = info.getSite(s);
            partsTable = p;
            getFieldNames();
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
            string downtime = records[loc[0]][loc[1] + 1];
            if (downtime.Equals(" "))
            {
                wo.DownTime = 0;
            }
            else
            {
                wo.DownTime = Convert.ToDouble(records[loc[0]][loc[1] + 1]);
            }
            loc = fieldToCell["Description"];
            wo.Description = records[loc[0]][loc[1] + 1];
            wo.Comments = wo.Description;
            addParts();
        }

        private bool isEmpty(string s)
        {
            return s.Equals(" ");
        }

        /* This function maps the fields to locations so
         * that when parsing data, the locations are already
         * pre-determined*/
        private void organizeFields()
        {
            fieldToCell = new Dictionary<string, int[]>();
            string key;

            for (int i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                //for (int j = 0; j < row.Count; j++)
                {
                    key = isField(row[0]);
                    if (key != null && !fieldToCell.ContainsKey(key))
                    {
                        fieldToCell.Add(key, new int[] { i, 0 });
                    }
                }
            }
        }

        private string isField(string s)
        {
            List<string> fieldKeys = fieldNames.Keys.ToList();
            string name = s.ToLower();
            if (name.Equals(" ")) return null;
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
            List<List<string>> data = info.getVendorData("Suzlon");
            fieldNames = new Dictionary<string, List<string>>();
            int len = Convert.ToInt32(data[0][1]);
            int start = 1;
            List<string> row;
            for (int i = start; i < start + len; i++)
            {
                row = data[i];
                row = data[i];
                List<string> fields = new List<string>();
                for (int j = 0; j < row.Count; j++)
                {
                    if (!row[j].Equals(" "))
                    {
                        fields.Add(row[j].ToLower());
                    }
                }
                fieldNames.Add(row[0], fields);
            }
        }

        private void addParts()
        {
            int[] loc = fieldToCell["Parts"];
            int i = loc[0]+1;
            int j = loc[1];
            int idLoc = j+1;
            int qLoc = idLoc+1;
            int desLoc = qLoc+2;

            while (!records[i][idLoc].Equals(" "))
            {
                string id = records[i][idLoc];
                double dQty = Convert.ToDouble(records[i][qLoc]);
                int qty = Convert.ToInt32(dQty);
                string partID = partsTable.getPartID(id, wo.Vendor.Name, qty);
                //string partID = wo.Vendor.getPartID(id, qty);
                if (partID != null)
                {
                    wo.addPart(partID, qty);
                }
                else
                {
                    string description = records[i][desLoc];
                    partID = partsTable.addNewPart(id, qty, description, wo.Vendor);
                    //partID = wo.Vendor.addNewPart(id, qty, description);
                    wo.addPart(partID, qty);
                }
                i++;
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
