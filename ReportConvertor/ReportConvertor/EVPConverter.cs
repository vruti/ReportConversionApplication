using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class EVPConverter : Converter
    {
        private Dictionary<string, Part> newParts;
        private WorkOrder newWO;
        private WorkOrder flaggedWO;
        private Dictionary<string, int> fieldToCell;
        private string site;
        AppInfo info;

        public EVPConverter(string s, AppInfo i)
        {
            info = i;
            site = s;
            newParts = new Dictionary<string, Part>();
            fieldToCell = new Dictionary<string, int>();
        }

        public void convertReport(Report report)
        {
            newWO = new WorkOrder("temp ID");
            List<List<string>> rec = report.getRecords("main");
            organizeFields(rec);

            newWO.Site = info.getSite(site);
            newWO.Vendor = newWO.Site.Contractor;
            newWO.Description = rec[fieldToCell["Description"]][0];
            newWO.WorkOrderType = rec[fieldToCell["Type"]][0];
            DateTime date = toDate(rec);
            newWO.OpenDate = date;
            newWO.EndDate = date;
            newWO.StartDate = date;
            newWO.DownTime = Convert.ToInt32(rec[fieldToCell["Down Time"]][0]);
            newWO.ActualHours = Convert.ToInt32(rec[fieldToCell["Actual Hours"]][0]);
            newWO.Comments = rec[fieldToCell["Comments"]][0];
        }

        private void addParts(List<List<string>> rec)
        {
            int i = fieldToCell["Parts"] + 2;
            while (!rec[i][0].Equals(""))
            {
                string id = rec[i][0];
                string partID = newWO.Vendor.getPartID(id);
                if (partID != null)
                {
                    int qty = Convert.ToInt32(rec[i][2]);
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

        private void organizeFields(List<List<string>> records)
        {
            bool isTable = false;
            for (int i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                if (row[0].Equals("Data Field"))
                {
                    isTable = true;
                }
                if (isTable)
                {
                    if (row[0].Contains("Parts"))
                    {
                        fieldToCell.Add("Parts", i);
                        isTable = false;
                    }
                    fieldToCell.Add(row[0], i);
                }
            }
        }

        private DateTime toDate(List<List<string>> rec)
        {
            string date = rec[fieldToCell["Date"]][0];

            int day = Convert.ToInt32(date.Substring(0, 2));
            int month = Convert.ToInt32(date.Substring(3, 2));
            int year = Convert.ToInt32(date.Substring(6, 4));
            DateTime dateTime = new DateTime(year, month, day);

            return dateTime;
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
