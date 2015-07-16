using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class EVPConverter : Converter
    {
        private WorkOrder newWO;
        private WorkOrder flaggedWO;
        private Dictionary<string, int> fieldToCell;
        private string site;
        AppInfo info;
        private Dictionary<string, List<string>> fieldNames;
        List<List<string>> records;
 
        public EVPConverter(string s, AppInfo i)
        {
            info = i;
            site = s;
            fieldToCell = new Dictionary<string, int>();
        }

        public void convertReport(Report report)
        {
            newWO = new WorkOrder("temp ID");
            records = report.getRecords("Entry");
            organizeFields();

            newWO.Site = info.getSite(site);
            newWO.Vendor = newWO.Site.Contractor;
            newWO.Description = records[fieldToCell["Description"]][1];
            newWO.WorkOrderType = records[fieldToCell["Type"]][1];
            DateTime date = toDate(records);
            newWO.OpenDate = date;
            newWO.EndDate = date;
            newWO.StartDate = date;
            newWO.DownTime = Convert.ToDouble(records[fieldToCell["Down Time"]][1]);
            newWO.ActualHours = Convert.ToDouble(records[fieldToCell["Actual Hours"]][1]);
            newWO.Comments = records[fieldToCell["Comments"]][1];
            newWO.Status = "Closed";
            addParts();
        }

        private void addParts()
        {
            int i = fieldToCell["Parts"] + 2;
            while (!records[i][0].Equals(" "))
            {
                string id = records[i][0];
                int qty = Convert.ToInt32(records[i][2]);
                string partID = newWO.Vendor.getPartID(id, qty);
                if (partID != null)
                {
                    newWO.addPart(partID, qty);
                }
                else
                {
                    flaggedWO = newWO;
                    newWO = null;
                }
                i++;
            }
        }

        private void addNewParts(List<List<string>> rec)
        {
            int i = fieldToCell["Parts"] + 2;
            while (!rec[i][4].Equals(""))
            {
                string id = rec[i][4];
                Part p = new Part(id, newWO.Vendor);
                p.Description = rec[i][5];
                p.Qty = Convert.ToInt32(rec[i][6]);
                p.generateID(newWO.Vendor.newestPartID());
                newWO.addPart(p.ID, p.Qty);
            }
        }

        private void organizeFields()
        {
            fieldToCell = new Dictionary<string, int>();
            getFieldNames();
            string key;

            for (int i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                for (int j = 0; j < row.Count; j++)
                {
                    key = isField(row[j]);
                    if (key != null && !fieldToCell.ContainsKey(key))
                    {
                        fieldToCell.Add(key, i);
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
            List<List<string>> data = info.getVendorData("EverPower");
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
                    for(j=0; j<row.Count; j++)
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

        private DateTime toDate(List<List<string>> rec)
        {
            string date = rec[fieldToCell["Date"]][1];
            DateTime dateTime = Convert.ToDateTime(date);

            return dateTime;
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
