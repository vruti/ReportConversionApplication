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

        public EVPConverter(AppInfo i, string s)
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

        public void organizeFields(List<List<string>> records)
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
