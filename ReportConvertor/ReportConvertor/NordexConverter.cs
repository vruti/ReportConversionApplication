using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class NordexConverter : Converter
    {
        private WorkOrder wo;
        private WorkOrder flaggedWO;
        private string site;
        private AppInfo info;
        private List<List<string>> records;
        private PartsTable partsTable;
        private AssetTable aTable;
        private Vendor ven;

        public NordexConverter(string s, AppInfo i, PartsTable p, AssetTable a)
        {
            site = s;
            info = i;
            partsTable = p;
            aTable = a;
            ven = info.getVendor("Nordex");
        }

        /* Start report conversion */
        public void convertReport(Report report)
        { 
            records = report.getRecords("Main");
            //Get report Id and start a new WorkOrder
            string id = getReportID();
            wo = new WorkOrder(id);
            wo.Site = info.getSite(site);
            wo.Vendor = ven;
            wo.Status = "Closed";
            //counters for date and parts
            int countS = 1;
            int countC = 1;

            for (int i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                if (row[0].Equals("Date"))
                {
                    wo.OpenDate = Convert.ToDateTime(row[1].Trim());
                } 
                else if (row[0].Contains("Start Date") && countS == 1)
                {
                    i = addDatesInfo((i + 1), id);
                    countS++;
                }
                else if (row[0].Contains("Oper.") && countC == 1)
                {
                    i = componentsUsed((i + 1));
                    countC++;
                }
                else if (row[0].Contains("Equipment Ref"))
                {
                    string asset = row[1].Trim();
                    wo.AssetID = aTable.getAssetID(asset, site);
                }
            }

            //Fill in any values that weren't present in report
            wo.fillValues();
            Validator v = new Validator();
            if (!v.isValid(wo))
            {
                flaggedWO = wo;
                wo = null;
            }
            
        }

        /* Get the Report ID number */
        private string getReportID()
        {
            foreach (List<string> row in records)
            {
                if (row[0].Contains("Order No."))
                {
                    return row[1];
                }
            }
            return null;
        }

        /* Since the date format is in ddmmyyy we need to
         * convert it manually instead of using the 
         * ToDate function in Convert*/
        private DateTime toDate(string d)
        {
            int day = Convert.ToInt32(d.Substring(0, 2));
            int month = Convert.ToInt32(d.Substring(3, 2));
            int year = Convert.ToInt32(d.Substring(6, 4));
            DateTime date;
            //In case the date is in mmddyyy format
            if (month > 12)
            {
                date = new DateTime(year, day, month);
            }
            else
            {
                date = new DateTime(year, month, day);
            }
            return date;
        }

        /* Fill in a dictionary with the exact cell location 
         * of each of the field headers.
         * NOTE: Not global due to only being used to calculate time*/
        private Dictionary<string, int> organizeFields(List<string> record)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            for (int i = 0; i < record.Count; i++)
            {
                string[] words = record[i].Split(' ');
                if (words.Length > 2)
                {
                    string name = words[0] +" "+ words[1];
                    result.Add(name, i);
                }
                else
                {
                    result.Add(record[i], i);
                }
            }
            return result;
        }
        
        /* Calculate and add all date and time related information */
        private int addDatesInfo(int i, string id)
        {
            Dictionary<string, int> fieldToCell = organizeFields(records[i-1]);
            DateTime def = new DateTime();
            while (!records[i][0].Contains("Oper. Ref."))
            {
                List<string> row = records[i];
                //Add start date
                if(wo.StartDate == def)
                {
                    DateTime s = toDate(row[fieldToCell["Start Date"]]);
                    wo.StartDate = s;
                } 
                else 
                {
                    //Ensures that the earliest date is set as start date
                    DateTime d = toDate(row[fieldToCell["Start Date"]]);
                    if (d < wo.StartDate)
                    {
                        wo.StartDate = d;
                    }
                }

                //Add End date
                if (wo.EndDate == def)
                {
                    wo.EndDate = toDate(row[fieldToCell["End Date"]]);
                }
                else
                {
                    //Ensures that the latest date is set as end date
                    DateTime d = toDate(row[fieldToCell["End Date"]]);
                    if (d > wo.EndDate)
                    {
                        wo.EndDate = d;
                    }
                }
                //Adding Description
                if (wo.Description == null)
                {
                    wo.Description = row[fieldToCell["Description of"]];
                }
                else
                {
                    //Ensures that the longest description is used
                    string d = row[fieldToCell["Description of"]];
                    if (d.Length > wo.Description.Length)
                    {
                        wo.Description = d;
                    }
                }
                //Adding comments
                if (wo.Comments == null)
                {
                    wo.Comments = row[fieldToCell["Longtext of"]];
                }
                else
                {
                    //Ensures that the longest comment is used
                    string c = row[fieldToCell["Longtext of"]];
                    if (c.Length > wo.Comments.Length)
                    {
                        wo.Comments = c;
                    }
                }

                //Calculate down time and labor/actual hours
                string startTime = row[fieldToCell["Start Time"]];
                string endTime = row[fieldToCell["End Time"]];
                if (row[fieldToCell["Description of"]].Contains("Start/Stop"))
                {
                    wo.DownTime += calculateHours(startTime, endTime);
                    wo.ActualHours += calculateHours(startTime, endTime);
                }
                else
                {
                    wo.ActualHours += calculateHours(startTime, endTime);
                }
                i++;
            }
            return (i-1);
        }

        /* Calculates the time span in hours between a start and stop time*/
        private double calculateHours(string start, string end)
        {
            DateTime s = Convert.ToDateTime(start);
            DateTime e = Convert.ToDateTime(end);
            TimeSpan time = e - s;
            double hours = time.TotalHours;
            return hours;
        }

        /* Find and adds all components(parts) to the work order */
        private int componentsUsed(int i)
        {
            Dictionary<string, int> fieldToCell = organizeFields(records[i-1]);
            while (!records[i][0].Contains("Measurement"))
            {
                List<string> record = records[i];
                string id = record[fieldToCell["Product Ref."]];
                //Quantity is in decimal format, but we want it as an int
                double dQty = Convert.ToDouble(record[fieldToCell["Quantity"]]);
                int qty = Convert.ToInt32(dQty);
                string partID = partsTable.getPartID(id, wo.Vendor.Name, qty);
                if (partID == null)
                {
                    string description = record[fieldToCell["Description"]];
                    partID = partsTable.addNewPart(id, qty, description, wo.Vendor);
                }
                wo.addPart(partID, qty);
                i++;
            }
            return (i - 1);
        }

        /* Return a list of the work orders
         * in this case, only one in the list
         * since the conveter only handles one at
         * a time */
        public List<WorkOrder> getWorkOrders()
        {
            List<WorkOrder> wos = new List<WorkOrder>();
            if (wo != null)
            {
                wo.createMPulseID();
                wos.Add(wo);
            }
            return wos;
        }

        /* Return a list of the flagged work orders
         * A null list if there isn't a flagged work 
         * order */
        public List<WorkOrder> getFlaggedWO()
        {
            List<WorkOrder> flagged = new List<WorkOrder>();
            if (flaggedWO != null)
            {
                /*ID is created so that changes can be made and
                 * the work order information can still be uploaded
                 * into MPulse */
                flaggedWO.createMPulseID();
                flagged.Add(flaggedWO);
            }
            return flagged;
        }
    }
}
