using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class GamesaConverter : Converter
    {
        //create a dictionary with all the fields and related titles?
        private Dictionary<string, ArrayList> fieldNames;
        private AppInfo info;
        private Dictionary<string, WorkOrder> newWOs;
        private Dictionary<string, Part> newParts;
        private Dictionary<string, WorkOrder> flaggedWO;

        public GamesaConverter(AppInfo i)
        {
            fieldNames = new Dictionary<string, ArrayList>();
            info = i;
            newWOs = new Dictionary<string, WorkOrder>();
            newParts = new Dictionary<string, Part>();
            flaggedWO = new Dictionary<string, WorkOrder>();
        }

        public Dictionary<string, WorkOrder> convertReport(Report report)
        {
            Dictionary<string, WorkOrder> result = new Dictionary<string,WorkOrder>();
            List<string> keys = report.getKeys();

            //Parsing the general info tab
            generalInfoReader(keys, report);

            //Parsing the hours/labor tab
            hoursTabReader(keys, report);
            stopTimeTabReader(keys, report);
            partsTabReader(keys, report);

            //CHANGE THIS
            List<string> woKeys = newWOs.Keys.ToList();
            foreach (string k in woKeys)
            {
                WorkOrder wo = newWOs[k];
                wo.createMPulseID();
                result.Add(wo.mPulseID, wo);
            }

            return result;
        }

        public Dictionary<string, int> organizeFields(List<List<string>> reportInfo, string tabName)
        {
            List<string> line = reportInfo[0];
            Dictionary<string, int> fieldToCell = new Dictionary<string, int>();

            Dictionary<string, List<string>> fieldNames = getFieldNames(tabName);

            List<string> fields = fieldNames.Keys.ToList();
            List<int> used = new List<int>();
            
            foreach (string field in fields)
            {
                if (tabName.Equals("Stop Times") && field.Equals("Start Time"))
                {
                    fieldToCell.Add(field, fieldToCell["Start Date"]+1);
                }
                else if (tabName.Equals("Stop Times") && field.Equals("Stop Time"))
                {
                    fieldToCell.Add(field, fieldToCell["Stop Date"] + 1);
                }
                else
                {
                    for (int i = 0; i < line.Count; i++)
                    {
                        string cell = line[i].ToLower();
                        if (!fieldToCell.ContainsKey(field))
                        {
                            foreach (string val in fieldNames[field])
                            {
                                string vall = val.ToLower();
                                if (cell.Contains(vall) && !used.Contains(i))// || val.ToLower().Contains(cell))
                                {
                                    fieldToCell.Add(field, i);
                                    used.Add(i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return fieldToCell;
        }

        private Dictionary<string, List<string>> getFieldNames(string tab)
        {
            List<List<string>> data = info.getVendorData("Gamesa");
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            int rows = 0;
            int start = 0;
            int i;
            List<string> row;

            for (i = 0; i < data.Count; i++)
            {
                row = data[i];
                if (row[0].Contains("Tab"))
                {
                    if (tab.Contains(row[1]) && !row[1].Equals(" "))
                    {
                        start = i+1;
                        rows = Convert.ToInt32(row[2]);
                        break;
                    }
                }
            }

            for (i = start; i < start + rows; i++)
            {
                row = new List<string>();
                foreach (string val in data[i])
                {
                    if (!val.Equals(" "))
                    {
                        row.Add(val);
                    }
                }
                result.Add(row[0], row);
            }
            return result;
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
            Dictionary<string, List<string>> table = getTableData();
            for(int i =1; i<rows.Count;i++)
            {
                List<string> row = rows[i];
                /* If the WO Type is ZPM7, it will not be imported*/
                if (!row[fieldToCell["Order Type"]].Contains("ZPM7"))
                {
                    WorkOrder wo = new WorkOrder(row[fieldToCell["Order ID"]]);
                    
                    //CHANGE THIS
                    string oType = row[fieldToCell["Order Type"]];
                    List<string> taskInfo = table[oType];
                    wo.WorkOrderType = taskInfo[0];
                    wo.TaskID = taskInfo[1];
                    wo.OutageType = taskInfo[2];
                    wo.Planning = taskInfo[3];
                    wo.UnplannedType = taskInfo[4];
                    wo.Priority = taskInfo[5];
                    wo.Description = row[fieldToCell["Description"]];
                    wo.Status = "Closed";
                    wo.Vendor = info.findVendor("Gamesa");
                    wo.Site = info.getSite("Patton");
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
                    newWOs.Add(wo.OriginalID, wo);
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

            for(int i = 0; i<rows; i++)
            {
                row = new List<string>();
                int cols = wk[i].Count;
                for (int j = 1; j < cols; j++)
                {
                    if (wk[i][0].Equals(" "))
                    {
                        return table;
                    }
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
                if (isTable)
                {
                    if (i > tableR + 1)
                    {
                        table.Add(wk[i][0], row);
                    }
                }
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

            List<List<string>> rows = report.getRecords(tab);
            Dictionary<string, int> fieldToCell = organizeFields(rows, tab);

            foreach (List<string> row in rows)
            {
                if (newWOs.ContainsKey(row[fieldToCell["Order ID"]]))
                {
                    WorkOrder wo = newWOs[row[fieldToCell["Order ID"]]];
                    if (!row[fieldToCell["Personnel No."]].Equals("00000000"))
                    {
                        wo.ActualHours += getLaborHours(row[fieldToCell["Start Time"]], row[fieldToCell["Stop Time"]]);
                    }
                    if (wo.Description == null)
                    {
                        wo.Description = row[fieldToCell["Remarks"]];
                    }
                }
            }
        }

        private void stopTimeTabReader(List<string> keys, Report report)
        {
            string tab = null;
            foreach (string key in keys)
            {
                if (key.ToLower().Contains("stop") || key.ToLower().Contains("time"))
                {
                    tab = key;
                }
            }

            List<List<string>> rows = report.getRecords(tab);
            Dictionary<string, int> fieldToCell = organizeFields(rows, tab);
            Dictionary<string,List<Tuple<DateTime, DateTime>>> stopTimes = new Dictionary<string,List<Tuple<DateTime, DateTime>>>();

            foreach (List<string> row in rows)
            {
                if (newWOs.ContainsKey(row[fieldToCell["Order ID"]]))
                {
                    if (!stopTimes.ContainsKey(row[fieldToCell["Order ID"]]))
                    {
                        List<Tuple<DateTime, DateTime>> list = new List<Tuple<DateTime, DateTime>>();
                        stopTimes.Add(row[fieldToCell["Order ID"]], list);
                    }
                    if (row[fieldToCell["Stop Time"]] != null)
                    {
                        DateTime start = getDateTimeInfo(row[fieldToCell["Start Date"]+1], row[fieldToCell["Start Date"]]);
                        DateTime stop = getDateTimeInfo(row[fieldToCell["Stop Time"]], row[fieldToCell["Stop Date"]]);
                        stopTimes[row[fieldToCell["Order ID"]]].Add(Tuple.Create<DateTime, DateTime>(start, stop));
                    } else
                    {
                        newWOs.Remove(row[fieldToCell["orderID"]]);
                    }
                }
            }

            List<string> stopKeys = stopTimes.Keys.ToList();
            foreach (string key in stopKeys)
            {
                double time = calculateStopTime(stopTimes[key]);
                newWOs[key].DownTime = time;
            }
        }

        private void partsTabReader(List<string> keys, Report report)
        {
            string tab = null;
            foreach (string key in keys)
            {
                if (key.ToLower().Contains("consumption"))
                {
                    tab = key;
                }
            }

            List<List<string>> rows = report.getRecords(tab);
            Dictionary<string, int> fieldToCell = organizeFields(rows, tab);

            foreach (List<string> row in rows)
            {
                string key = row[fieldToCell["Order ID"]];
                if (newWOs.ContainsKey(key))
                {
                    if (row[fieldToCell["MvT"]].Equals("967"))
                    {
                        string part = row[fieldToCell["Material"]];
                        WorkOrder wo = newWOs[key];
                        string partID = newWOs[key].Vendor.getPartID(row[fieldToCell["Material"]]);
                        int qty = (int) Convert.ToDouble(row[fieldToCell["Quantity"]]);
                        if (partID != null)
                        {
                            newWOs[key].addPart(partID, qty);
                        }
                        else
                        {
                            if (newParts.ContainsKey(part))
                            {
                                newParts[part].Qty += qty;
                            }
                            else
                            {
                                //create new part record
                                Part newPart = new Part(part);
                                if (newParts.Count > 1)
                                {
                                    List<string> k = newParts.Keys.ToList();
                                    int len = k.Count;
                                    newPart.generateID(newParts[k[len - 1]].ID);
                                }
                                else
                                {
                                    newPart.generateID(newWOs[key].Vendor.newestPartID());
                                }
                                newPart.Qty = qty;
                                newPart.Description = row[fieldToCell["Description"]];
                                newParts.Add(part, newPart);
                            }
                        }
                    }
                }
            }
        }


        private double calculateStopTime(List<Tuple<DateTime, DateTime>> times)
        {
            double downTime=0.0;
            Tuple<DateTime, DateTime> tuple;
            TimeSpan duration;
            int len = times.Count;
            if (len > 1)
            {
                for (int i = 0; i < len-1; i++)
                {
                    DateTime start1 = times[i].Item1;
                    DateTime stop1 = times[i].Item2;
                    DateTime start2 = times[i + 1].Item1;
                    DateTime stop2 = times[i + 1].Item2;

                    /* Using formula based on mergeSort to find any overlapping
                     * downTimes, merge them and replace it in the list of
                     * stop times*/
                    if (start1 < start2 && stop1 > start2 && stop2 > stop1)
                    {
                        tuple = Tuple.Create<DateTime, DateTime>(start1, stop2);
                        times[i] = tuple;
                        times.RemoveAt(i + 1);
                        i = 1;
                    }
                    else if (start1 > start2 && stop2 > start1 && stop1 > stop2)
                    {
                        tuple = Tuple.Create<DateTime, DateTime>(start2, stop1);
                        times[i] = tuple;
                        times.RemoveAt(i + 1);
                        i = 1;
                    }
                }
            }
            len = times.Count;
            foreach (Tuple<DateTime, DateTime> time in times)
            {
                DateTime start = time.Item1;
                DateTime stop = time.Item2;
                duration = stop - start;
                downTime = downTime + (duration.TotalHours + (duration.TotalMinutes / 60));
            }

            return downTime;
        }

        private DateTime getDateTimeInfo(string time, string date)
        {
            int month = Convert.ToInt32(date.Substring(0, 2));
            int day = Convert.ToInt32(date.Substring(3, 2));
            int year = Convert.ToInt32(date.Substring(6, 4));
            int hours = Convert.ToInt32(time.Substring(0, 2));
            int min = Convert.ToInt32(time.Substring(3, 2));
            int sec = Convert.ToInt32(time.Substring(6, 2));
            DateTime dateTime = new DateTime(year, month, day, hours, min, sec);

            return dateTime;
        }

        private double getLaborHours(string start, string stop)
        {
            double time = 0;
            if(start.Contains(":"))
            {
                //hours
                double startH = Convert.ToDouble(start.Substring(0, 2));
                double stopH = Convert.ToDouble(stop.Substring(0, 2));
                double h = stopH - startH;
                //minutes
                double startM = Convert.ToDouble(start.Substring(3, 2));
                double stopM = Convert.ToDouble(stop.Substring(3, 2));
                double m = (stopM - startM) / 60;
                time = h + m;
            }
            return time;
        }

        private DateTime getDate(string s)
        {
            string sMonth = s.Substring(0, 2);
            string sDate = s.Substring(3, 2);
            string sYear = s.Substring(6, 4);

            int nDate = Convert.ToInt32(sDate);
            int nMonth = Convert.ToInt32(sMonth);
            int nYear = Convert.ToInt32(sYear);
            DateTime date = new DateTime(nYear, nMonth, nDate);

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
