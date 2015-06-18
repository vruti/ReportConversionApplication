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

        public Dictionary<string, WorkOrder> startConversion(Report report)
        {
            Dictionary<string, WorkOrder> result = null;
            List<string> keys = report.getKeys();

            foreach (string key in keys){
                if(key.ToLower().Contains("general")){
                    generalTab = key;
                }
            }

            List<List<string>> generalInfo = report.getRecords(generalTab);


            return result;
        }

        public Dictionary<string, int> organizeFields(List<List<string>> reports, string tabName)
        {
            List<string> line = reports[1];
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

        public List<WorkOrder> generalTab(Dictionary<string, int> fieldToCell, List<List<string>> rows)
        {
            List<WorkOrder> newWOs = new List<WorkOrder>();
            foreach (List<string> row in rows)
            {
                WorkOrder wo = new WorkOrder(row[fieldToCell["orderID"]]);
                wo.createMPulseID();
                //CHANGE THIS
                wo.WorkOrderType = row[fieldToCell["Order Type"]];

            }
        }
    }
}
