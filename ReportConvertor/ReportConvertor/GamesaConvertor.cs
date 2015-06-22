using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public class GamesaConvertor : Convertor
    {
        //create a dictionary with all the fields and related titles?
        private Dictionary<string, ArrayList> fieldNames;
        private string generalTab;
        private AppInfo info;

        public GamesaConvertor(AppInfo i)
        {
            fieldNames = new Dictionary<string, ArrayList>();
            generalTab = null;
            info = i;
        }

        public Dictionary<string, WorkOrder> convertReport(Report report)
        {
            Dictionary<string, WorkOrder> result = null;
            List<string> keys = report.getKeys();

            foreach (string key in keys){
                if(key.ToLower().Contains("general")){
                    generalTab = key;
                }
            }

            List<List<string>> generalInfo = report.getRecords(generalTab);
            Dictionary<string, int> fieldToCells = organizeFields(generalInfo, generalTab);
            List<WorkOrder> woInits = generalInfoReader(fieldToCells, generalInfo);

            //CHANGE THIS
            foreach (WorkOrder wo in woInits)
            {
                result.Add(wo.mPulseID, wo);
            }

            return result;
        }

        public Dictionary<string, int> organizeFields(List<List<string>> reportInfo, string tabName)
        {
            List<string> line = reportInfo[1];
            ArrayList fields = fieldNames[tabName];
            Dictionary<string, int> fieldToCell = new Dictionary<string, int>();
            foreach (string field in fields)
            {
                for (int i = 1; i <= line.Count; i++)
                {
                    string cell = line[i].ToLower();
                    if ((field.ToLower().Contains(cell)) || cell.Contains(field.ToLower()))
                    {
                        fieldToCell.Add(field, i);
                    }
                }
            }
            return fieldToCell;
        }

        public List<WorkOrder> generalInfoReader(Dictionary<string, int> fieldToCell, List<List<string>> rows)
        {
            List<WorkOrder> newWOs = new List<WorkOrder>();
            foreach (List<string> row in rows)
            {
                WorkOrder wo = new WorkOrder(row[fieldToCell["orderID"]]);
                wo.createMPulseID();
                //CHANGE THIS
                wo.WorkOrderType = row[fieldToCell["Order Type"]];
                wo.Description = row[fieldToCell["Description"]];
                wo.Vendor = info.findVendor("gamesa");
                wo.Site = info.getSite("patton");
                //add work order to list
                newWOs.Add(wo);

                //if the dates are available in the general tab
                if (fieldToCell.ContainsKey("Start Date") && fieldToCell.ContainsKey("End Date"))
                {
                    wo.StartDate = getDate(row[fieldToCell["Start Date"]]);
                    wo.EndDate = getDate(row[fieldToCell["End Date"]]);
                }
            }
            return newWOs;
        }

        private DateTime getDate(string s)
        {
            string sDate = s.Substring(1, 2);
            string sMonth = s.Substring(4, 2);
            string sYear = s.Substring(7, 4);

            int nDate = Convert.ToInt32(sDate);
            int nMonth = Convert.ToInt32(sMonth);
            int nYear = Convert.ToInt32(sYear);

            DateTime date = new DateTime(nDate, nMonth, nYear);

            return date;
        }

        public int getDownTime()
        {
            return 0;
        }
    }
}
