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
            //CHANGE THIS OR ELSE
            int i = 0, j = 0;
            while (id == null)
            {
                if (records[i][j].Contains("SO #"))
                {
                    id =records[i][j+1];
                }
            }
            wo = new WorkOrder(id);
            wo.WorkOrderType = report.checkedVals()[0];
        }

        private void getDownTime()
        {
            bool isStopTime = false;
            foreach (List<string> row in records)
            {
                if (isStopTime && row[1].Equals(" "))
                {
                    break;
                }
                if (isStopTime)
                {
                    int last = row.Count - 1;
                    double time = Convert.ToDouble(row[last]);
                    wo.DownTime += time;
                }
                if (row[1].Contains("Turbine Stop"))
                {
                    isStopTime = true;
                }
            }
        }

        /*Find and add all the parts that are
         * used in the work order*/
        private void addParts(int start)
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
