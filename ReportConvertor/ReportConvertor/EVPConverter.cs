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

        public Dictionary<string, WorkOrder> getWorkOrders()
        {
            return newWOs;
        }
    }
}
