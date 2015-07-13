using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    //A class to specify a range of dates
    public class DateRange
    {
        private DateTime start;
        private DateTime end;

        //Requires a start date and end date
        public DateRange(DateTime s, DateTime e)
        {
            start = s;
            end = e;
        }

        //getter and setter for start date
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value;
            }
        }

        //getter and setter for start date
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value;
            }
        }

        //get the time in hours between start and end
        public double getHours()
        {
            TimeSpan difference = end - start;
            return difference.TotalHours;
        }
    }

    public class GamesaConverter : Converter
    {
        //Global fields
        private AppInfo info;
        private Dictionary<string, WorkOrder> newWOs;
        private Dictionary<string, Part> newParts;
        private Dictionary<string, WorkOrder> flaggedWO;
        Dictionary<string, List<string>> fieldNames;

        public GamesaConverter(AppInfo i)
        {
            info = i;
            newParts = new Dictionary<string, Part>();
            flaggedWO = new Dictionary<string, WorkOrder>();
        }

        public void convertReport(Report report)
        {
            newWOs = new Dictionary<string, WorkOrder>();
            List<string> keys = report.getKeys();

            //Parsing the general info tab
            generalInfoReader(keys, report);

            //Parsing the hours/labor tab
            hoursTabReader(keys, report);
            stopTimeTabReader(keys, report);
            partsTabReader(keys, report);
        }

        public Dictionary<string, int> organizeFields(List<string> line, string tabName)
        {
            Dictionary<string, int> fieldToCell = new Dictionary<string, int>();
            getFieldNames(tabName);
            List<string> fields = fieldNames.Keys.ToList();
            List<int> used = new List<int>();

            for (int i = 0; i < line.Count; i++)
            {
                string key = isField(line[i]);
                if (key != null && !fieldToCell.ContainsKey(key) && !used.Contains(i))
                {
                    fieldToCell.Add(key, i);
                }
            }     
            return fieldToCell;
        }

        private string isField(string s)
        {
            List<string> fieldKeys = fieldNames.Keys.ToList();
            string name = s.ToLower();
            foreach (string key in fieldKeys)
            {
                foreach (string field in fieldNames[key])
                {
                    if (name.Contains(field.ToLower()))
                    {
                        fieldNames.Remove(key);
                        return key;
                    }
                }
            }           
            return null;
        }

        private void getFieldNames(string tab)
        {
            List<List<string>> data = info.getVendorData("Gamesa");
            fieldNames = new Dictionary<string, List<string>>();
            int i, j;
            List<string> row;
            bool isFieldTable = false;

            for (i = 0; i < data.Count; i++)
            {
                row = data[i];
                if (row[0].Contains("Tab") && isTab(tab, row))
                {
                    isFieldTable = true;
                    i++;
                }
                if (isFieldTable && row[0].Equals(" "))
                {
                    isFieldTable = false;
                    break;
                }
                if (isFieldTable)
                {
                    j = 0;
                    row = data[i];
                    List<string> fields = new List<string>();
                    while (!row[j].Equals(" "))
                    {
                        fields.Add(row[j].ToLower());
                        j++;
                    }
                    fieldNames.Add(row[0], fields);
                }
            }
        }

        private bool isTab(string tab, List<string> row)
        {
            for (int i = 2; i < row.Count; i++)
            {
                if (tab.Contains(row[i]) && !row[i].Equals(" "))
                {
                    return true;
                }
            }
            return false;
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
            Dictionary<string, int> fieldToCell = organizeFields(rows[0], tab);
            Dictionary<string, List<string>> table = getTableData();
            for(int i =1; i<rows.Count;i++)
            {
                List<string> row = rows[i];
                /* If the WO Type is ZPM7, it will not be imported*/
                
                WorkOrder wo = new WorkOrder(row[fieldToCell["Order ID"]]);
                string oType = row[fieldToCell["Order Type"]];
                List<string> taskInfo = table[oType];
                wo.WorkOrderType = taskInfo[0];
                wo.TaskID = taskInfo[1];
                wo.OutageType = taskInfo[2];
                wo.Planning = taskInfo[3];
                wo.UnplannedType = taskInfo[4];
                wo.Priority = taskInfo[5];
                wo.Description = row[fieldToCell["Description"]];
                //Status will always be closed
                wo.Status = "Closed";
                wo.Vendor = info.getVendor("Gamesa");
                wo.Site = info.getSite("Patton");
                //get asset ID
                wo.AssetID = getAssetNo(row[fieldToCell["Turbine No."]]);

                //if the dates are available in the general tab
                if (fieldToCell.ContainsKey("Start Date") && fieldToCell.ContainsKey("End Date"))
                {
                    wo.StartDate = getDateTime(row[fieldToCell["Start Date"]], " ");
                    wo.EndDate = getDateTime(row[fieldToCell["End Date"]], " ");
                    wo.OpenDate = wo.StartDate;
                }
                //add work order to list
                newWOs.Add(wo.OriginalID, wo);
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
            Dictionary<string, int> fieldToCell = organizeFields(rows[0], tab);

            foreach (List<string> row in rows)
            {
                if (newWOs.ContainsKey(row[fieldToCell["Order ID"]]))
                {
                    WorkOrder wo = newWOs[row[fieldToCell["Order ID"]]];
                    //If personnel no. is 00000000 then don't take the hours
                    if (!row[fieldToCell["Personnel No."]].Equals("00000000"))
                    {
                        DateTime start = getDateTime("", row[fieldToCell["Start Time"]]);
                        DateTime end = getDateTime("", row[fieldToCell["Stop Time"]]);
                        TimeSpan hours = end - start;
                        wo.ActualHours = hours.TotalHours;
                    }
                    /*if description isn't in the main tab, take it from
                     * the remarks field here*/
                    if (wo.Description == null)
                    {
                        wo.Description = row[fieldToCell["Remarks"]];
                    }
                }
            }
        }

        private Dictionary<string, int> startStopFields(List<string> row, string tab)
        {
            Dictionary<string, int> fieldToCell = organizeFields(row, tab);
            int s = fieldToCell["Start Date"];
            int e = fieldToCell["Stop Date"];
            if (s > e)
            {
                fieldToCell["Start Date"] = e;
                fieldToCell["Stop Date"] = s;
            }
            fieldToCell.Add("Start Time", fieldToCell["Start Date"]+1);
            fieldToCell.Add("Stop Time", fieldToCell["Stop Date"]+1);

            return fieldToCell;
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

            List<List<string>> records = report.getRecords(tab);
            Dictionary<string, int> fieldToCell = startStopFields(records[0], tab);
            Dictionary<string,List<DateRange>> stopTimes = new Dictionary<string,List<DateRange>>();
            //Remove the first line containing header values
            records.RemoveRange(0, 1);

            foreach (List<string> row in records)
            {
                string id = row[fieldToCell["Order ID"]];
                //By this time, the order should already exist in the dictionary of work orders
                if (newWOs.ContainsKey(id))
                {
                    if (!stopTimes.ContainsKey(id))
                    {
                        List<DateRange> list = new List<DateRange>();
                        stopTimes.Add(id, list);
                    }
                    if (!row[fieldToCell["Stop Time"]].Equals(" "))
                    {
                        DateTime start = getDateTime(row[fieldToCell["Start Date"]], row[fieldToCell["Start Time"]]);
                        DateTime stop = getDateTime(row[fieldToCell["Stop Date"]], row[fieldToCell["Stop Time"]]);
                        DateRange range = new DateRange(start, stop);
                        stopTimes[id].Add(range);
                    } else
                    {
                        newWOs.Remove(id);
                    }
                    if ((newWOs[id].Comments == null) && fieldToCell.ContainsKey("Remarks"))
                    {
                        newWOs[id].Comments = row[fieldToCell["Remarks"]];
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
            Dictionary<string, int> fieldToCell = organizeFields(rows[0], tab);

            foreach (List<string> row in rows)
            {
                string key = row[fieldToCell["Order ID"]];
                if (newWOs.ContainsKey(key))
                {
                    string part = row[fieldToCell["Material"]];
                    WorkOrder wo = newWOs[key];
                    string partID = newWOs[key].Vendor.getPartID(row[fieldToCell["Material"]]);
                    int qty = (int)Convert.ToDouble(row[fieldToCell["Quantity"]]);
                    qty = qty * (-1);

                    if (partID != null)
                    {
                        newWOs[key].addPart(partID, qty);
                    }
                    else
                    {
                        if (newParts.ContainsKey(part))
                        {
                            partID = newParts[part].ID;
                            newWOs[key].addPart(partID, qty);
                        }
                        else
                        {
                            //create new part record
                            Part newPart = new Part(part, wo.Vendor);
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
                            newWOs[key].addPart(newPart.ID, qty);
                        }
                    }
                }
            }
        }


        private double calculateStopTime(List<DateRange> times)
        {
            double downTime=0.0;
            for (int i = 0; i < times.Count - 1; i++)
            {
                DateTime start1 = times[i].Start;
                DateTime stop1 = times[i].End;
                DateTime start2 = times[i + 1].Start;
                DateTime stop2 = times[i + 1].End;

                /* Using formula based on mergeSort to find any overlapping
                 * downTimes, merge them and replace it in the list of
                 * stop times*/
                if (start1 < start2 && stop1 > start2 && stop2 > stop1)
                {
                    times[i].End = stop2;
                    times.RemoveAt(i + 1);
                    i = 0;
                }
                else if (start1 > start2 && stop2 > start1 && stop1 > stop2)
                {
                    times[i].Start = start2;
                    times.RemoveAt(i + 1);
                    i = 0;
                }
            }
            
            foreach (DateRange time in times)
            {
                downTime = downTime + time.getHours();
            }

            return downTime;
        }

        private DateTime getDateTime(string date, string time)
        {
            string combined = date.Trim() + "," + time.Trim();
            DateTime dateTime = Convert.ToDateTime(combined);

            return dateTime;
        }

        private string getAssetNo(string n)
        {
            string num = "WTG-";
            int count=0;
            for (int i = 1; i < n.Length; i++)
            {
                if (count == 2)
                {
                    int len = n.Length - i;
                    num += n.Substring(i, len).Trim();
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
                    string v = n.Substring(i, 1);
                    if (v.Equals("/"))
                    {
                        count++;
                    }
                }
            }
            return null;
        }

        public List<WorkOrder> getWorkOrders()
        {
            List<string> keys = newWOs.Keys.ToList();
            List<WorkOrder> newWOList = new List<WorkOrder>();
            foreach (string key in keys)
            {
                if (newWOs[key].DownTime > 0)
                {
                    newWOs[key].createMPulseID();
                    newWOList.Add(newWOs[key]);
                }
            }
            return newWOList;
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
    }
}
