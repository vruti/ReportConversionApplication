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
        private WorkOrder flaggedWO;
        private Dictionary<string, Part> newParts;
        private List<List<string>> records;

        public NordexExcelConverter(string s, AppInfo i)
        {
            newParts = new Dictionary<string, Part>();
            site = s;
            info = i;
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("main");
            string id = null;
            int i = 0, j = 0;
            while (id == null)
            {
                if (records[i][j].Contains("SO #"))
                {
                    id =records[i][j+1];
                }
            }
            wo = new WorkOrder(id);

        }

        private void get

        private void addNewParts(int start)
        {

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
            wo.createMPulseID();
            newWOs.Add(wo);
            return newWOs;
        }
    }
}
