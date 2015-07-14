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
        private Dictionary<string, int> fieldToCell;

        public NordexExcelConverter(string s, AppInfo i)
        {
            newParts = new Dictionary<string, Part>();
            site = s;
            info = i;
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("main");
            string id = getID();
            wo = new WorkOrder(id);
            wo.WorkOrderType = report.checkedVals()[0];
        }

        private string getID()
        {
            string id = null;
            List<string> idFields = fieldNames["ID"];
            bool isID = false;
            for (int i = 0; i < records.Count; i++)
            {
                if (!isID)
                {
                    List<string> row = records[i];
                    for (int j = 0; j < row.Count; j++)
                    {
                        if (isID)
                        {
                            if (!row[j].Equals(" "))
                            {
                                id = row[j];
                                break;
                            }
                        }
                        else
                        {
                            foreach (string field in idFields)
                            {
                                if (row[j].ToLower().Contains(field.ToLower()))
                                {
                                    isID = true;
                                }
                            }
                        }
                    }
                }
            }
                return id;
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
