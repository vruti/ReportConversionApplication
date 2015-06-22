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
        private AppInfo info;
        private List<WorkOrder> newWOs;

        public GamesaConvertor(AppInfo i)
        {
            fieldNames = new Dictionary<string, ArrayList>();
            info = i;
            newWOs = new List<WorkOrder>();
        }

        public Dictionary<string, WorkOrder> convertReport(Report report)
        {
            Dictionary<string, WorkOrder> result = null;
            List<string> keys = report.getKeys();

            //Parsing the general info tab
            generalInfoReader(keys, report);

            //Parsing the hours/labor tab
            hoursTabReader(keys, report);

            //CHANGE THIS
            foreach (WorkOrder wo in newWOs)
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

        public void generalInfoReader(List<string> keys, Report report)
        {
            string tab = null;
            foreach (string key in keys)
            {
                if (key.ToLower().Contains("general"))
                {
                    tab = key;
                }
            }

            //Parsing the general info tab
            List<List<string>> rows = report.getRecords(tab);
            Dictionary<string, int> fieldToCell = organizeFields(rows, tab);

            foreach (List<string> row in rows)
            {
                /* If the WO Type is ZPM7, it will not be imported*/
                if (!row[fieldToCell["Order Type"]].Contains("ZPM7"))
                {
                    WorkOrder wo = new WorkOrder(row[fieldToCell["orderID"]]);
                    wo.createMPulseID();
                    //CHANGE THIS
                    string oType = row[fieldToCell["Order Type"]];
                    List<string> taskInfo = getTableData()[oType];
                    wo.WorkOrderType = taskInfo[1];
                    wo.TaskID = taskInfo[2];
                    wo.OutageType = taskInfo[3];
                    wo.Planning = taskInfo[4];
                    wo.UnplannedType = taskInfo[5];
                    wo.Priority = taskInfo[6];
                    wo.Description = row[fieldToCell["Description"]];
                    wo.Status = "Closed";
                    wo.Vendor = info.findVendor("gamesa");
                    wo.Site = info.getSite("patton");
                    //get asset ID
                    wo.AssetID = getAssetNo(row[fieldToCell["Turbine No."]]);

                    //if the dates are available in the general tab
                    if (fieldToCell.ContainsKey("Start Date") && fieldToCell.ContainsKey("End Date"))
                    {
                        wo.StartDate = getDate(row[fieldToCell["Start Date"]]);
                        wo.EndDate = getDate(row[fieldToCell["End Date"]]);
                        wo.OpenDate = wo.StartDate;
                    }
                    //add work order to list
                    newWOs.Add(wo);
                }
            }
        }

        private Dictionary<string, List<string>> getTableData()
        {
            Dictionary<string, List<string>> table = new Dictionary<string, List<string>>();

            List<List<string>> wk = info.getVendorData("Gamesa");
            List<string> row;
            bool isTable = false;
            int rows = wk.Count;
            int tableR = 0;

            for(int i =1; i<=rows; i++)
            {
                row = new List<string>();
                int cols = wk[i].Count;
                for (int j = 2; j <= cols; j++)
                {
                    if (isTable)
                    {
                        if (i > tableR + 1)
                        {
                            row.Add(wk[i][j]);
                        }
                    }
                    else
                    {
                        if (wk[i][j].ToLower().Contains("type"))
                        {
                            isTable = true;
                            tableR = i;
                        }
                    }
                }
                table.Add(wk[i][1], row);
            }

            return table;
        }

        private void hoursTabReader(List<string> keys, Report report)
        {
            string tab = null;
            foreach (string key in keys)
            {
                if (key.ToLower().Contains("hours") || key.ToLower().Contains("labor"))
                {
                    tab = key;
                }
            }

            //Parsing the general info tab
            List<List<string>> rows = report.getRecords(tab);
            Dictionary<string, int> fieldToCell = organizeFields(rows, tab);
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

        private string getAssetNo(string n)
        {
            string num = "WTG-";
            int count=0;
            for (int i = 1; i < n.Length; i++)
            {
                if (count == 2)
                {
                    if (!n[i].Equals(""))
                    {
                        if (n[i].Equals("-"))
                        {
                            return info.getAssetID(num);
                        }
                        num += n[i];
                    }
                }
                else
                {
                    if (n[i].Equals("/"))
                    {
                        count++;
                    }
                }
            }

            return null;
        }
    }
}
