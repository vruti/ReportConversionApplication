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
        private List<WorkOrder> flaggedWO;
        private Dictionary<string, List<string>> fieldNames;
        private PartsTable partsTable;
        private AssetTable aTable;
        private Vendor ven;
        private WOTable woTable;

        public GamesaConverter(AppInfo i, PartsTable p, AssetTable a, WOTable woT)
        {
            info = i;
            partsTable = p;
            aTable = a;
            flaggedWO = new List<WorkOrder>();
            ven = info.getVendor("Gamesa");
            woTable = woT;
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

        /* Fill in the fieldToCell dictionary with the 
         * exact cell location of each of the field headers */
        public Dictionary<string, int> organizeFields(List<string> line, string tabName)
        {
            Dictionary<string, int> fieldToCell = new Dictionary<string, int>();
            fieldNames = ven.getFieldNames(tabName);
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

        /* Checks if a string is a field header name*/
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

        /* Adds information from the general info tab in the report*/
        public void generalInfoReader(List<string> keys, Report report)
        {
            string tab = null;
            foreach (string key in keys)
            {
                if (key.ToLower().Contains("main"))
                {
                    tab = key;
                    break;
                }
            }

            //Parsing the general info tab
            List<List<string>> rows = report.getRecords(tab);
            Dictionary<string, int> fieldToCell = organizeFields(rows[0], "Main");
            for (int i = 1; i < rows.Count; i++)
            {
                List<string> row = rows[i];
                /* If the WO Type is ZPM7, it will not be imported*/

                WorkOrder wo = new WorkOrder(row[fieldToCell["Order ID"]], woTable, report.Filepath);
                string oType = row[fieldToCell["Order Type"]];
                if (!oType.Contains("ZPM7"))
                {
                    //Get subsequest task information from the work order type code
                    List<string> taskInfo = info.getTypeInfo(oType);
                    wo.WorkOrderType = taskInfo[0];
                    wo.TaskID = taskInfo[1];
                    wo.TaskDescription = taskInfo[2];
                    wo.OutageType = taskInfo[3];
                    wo.Planning = taskInfo[4];
                    wo.UnplannedType = taskInfo[5];
                    wo.Priority = taskInfo[6];
                    wo.Description = row[fieldToCell["Description"]];
                    
                    //Status will always be closed
                    wo.Status = "Closed";
                    wo.Vendor = ven;
                    wo.Site = info.getSite("Patton");
                    string asset = row[fieldToCell["Asset"]];
                    List<string> a = aTable.getAssetID(asset, "Patton");
                    wo.AssetID = a[0];
                    wo.AssetDescription = a[1];

                    //if the dates are available in the general tab
                    if (fieldToCell.ContainsKey("Start Date") && fieldToCell.ContainsKey("End Date"))
                    {
                        wo.StartDate = Convert.ToDateTime(row[fieldToCell["Start Date"]]);
                        wo.EndDate = Convert.ToDateTime(row[fieldToCell["End Date"]]);
                        wo.OpenDate = wo.StartDate;
                    }
                    //add work order to list
                    newWOs.Add(wo.OriginalID, wo);
                }
            }
        }

        /* Add information from the hours/labor tab*/ 
        private void hoursTabReader(List<string> keys, Report report)
        {
            string tab = null;

            //Ensuring that the correct tab is picked
            foreach (string key in keys)
            {
                if (key.ToLower().Contains("hours") || key.ToLower().Contains("labor"))
                {
                    tab = key;
                }
            }

            //Get the data read from the excel file
            List<List<string>> rows = report.getRecords(tab);
            //Get the field header names and the corresponding columns
            Dictionary<string, int> fieldToCell = organizeFields(rows[0], "Labor");

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

        /*
         * Fills in specific column information for certain fields in the 
         * stop time tab. These fields are difficult to identify using the
         * ogranize field function due to the lack of uniformity with the
         * naming of the fields.
         */ 
        private Dictionary<string, int> startStopFields(List<string> row, string tab)
        {
            Dictionary<string, int> fieldToCell = organizeFields(row, "Stop Time");
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

        /* Add information from the stop time tab in the report */ 
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
            /* The stopTimes dictionary is used to keep track of all 
             * the stop and start times listed in the report */
            Dictionary<string,List<DateRange>> stopTimes = new Dictionary<string,List<DateRange>>();
            //Remove the first line containing header values
            records.RemoveRange(0, 1);

            foreach (List<string> row in records)
            {
                string id = row[fieldToCell["Order ID"]];
                //By this time, the order should already exist in the dictionary of work orders
                if (newWOs.ContainsKey(id))
                {
                    //If the work order isn't present in the stop time dictionary, it is added
                    if (!stopTimes.ContainsKey(id))
                    {
                        List<DateRange> list = new List<DateRange>();
                        stopTimes.Add(id, list);
                    }
                    //if the stop time is blank, the work order is invalid
                    if (row[fieldToCell["Stop Time"]].Equals(" "))
                    {
                        newWOs.Remove(id);
                    } 
                    else
                    {
                        /* Get the start and stop time including the date and put it in
                         * a DateRange, add it to the dictionary
                         */
                        DateTime start = getDateTime(row[fieldToCell["Start Date"]], row[fieldToCell["Start Time"]]);
                        DateTime stop = getDateTime(row[fieldToCell["Stop Date"]], row[fieldToCell["Stop Time"]]);
                        DateRange range = new DateRange(start, stop);
                        stopTimes[id].Add(range);
                    }
                    /* If there were no comments on the general info tab, and the Remarks
                     * field exists in this tab, use the value in the Remarks column as
                     * Comments*/
                    if ((newWOs[id].Comments == null) && fieldToCell.ContainsKey("Remarks"))
                    {
                        newWOs[id].Comments = row[fieldToCell["Remarks"]];
                    }
                }
            }
            /* Once all the rows in the report have been read and added to
             * the stopTime dictionary, calculate the down time of each 
             * work order */
            List<string> stopKeys = stopTimes.Keys.ToList();
            foreach (string key in stopKeys)
            {
                double time = calculateDownTime(stopTimes[key]);
                newWOs[key].DownTime = time;
            }
        }

        /* Calculates the downtime of the turbine in the report*/
        private double calculateDownTime(List<DateRange> times)
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

        /* Return a DateTime value obtained from given date and time strings */
        private DateTime getDateTime(string date, string time)
        {
            string combined = date.Trim() + "," + time.Trim();
            DateTime dateTime = Convert.ToDateTime(combined);

            return dateTime;
        }

        /* Add information from the Parts tab in the report*/
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
            Dictionary<string, int> fieldToCell = organizeFields(rows[0], "Consumption");

            foreach (List<string> row in rows)
            {
                string key = row[fieldToCell["Order ID"]];
                if (newWOs.ContainsKey(key))
                {
                    string part = row[fieldToCell["Material"]];
                    WorkOrder wo = newWOs[key];
                    int qty = (int)Convert.ToDouble(row[fieldToCell["Quantity"]]);
                    string mVT = row[fieldToCell["MvT"]];
                    qty = Math.Abs(qty);
                    if (!mVT.Equals("965"))
                    {
                        qty = qty * (-1);
                    }
                    string id = row[fieldToCell["Material"]];
                    Part p = partsTable.getPart(id, wo.Vendor.Name, qty);

                    if (part == null)
                    {
                        string description = row[fieldToCell["Description"]];
                        p = partsTable.addNewPart(id, qty, description, wo.Vendor);
                    }
                    wo.addPart(p, qty);
                    newWOs[key] = wo;
                }
            }
        }

        /* Return a list of the work orders in the report
         * A null list if there isn't aren't any work 
         * orders */
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

        /* Return a list of the flagged work orders
         * A null list if there aren't any flagged work 
         * orders */
        public List<WorkOrder> getFlaggedWO()
        {
            List<WorkOrder> flagged = new List<WorkOrder>();
            if (flaggedWO != null)
            {
                foreach (WorkOrder wo in flaggedWO)
                {
                    /* ID is created so that changes can be made and
                     * the work order information can still be uploaded
                     * into MPulse */
                    wo.createMPulseID();
                    flagged.Add(wo);
                }
            }
            return flagged;
        }
    }
}
