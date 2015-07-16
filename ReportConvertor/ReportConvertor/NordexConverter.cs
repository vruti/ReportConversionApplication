using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class NordexConverter : Converter
    {
        private WorkOrder newWO;
        private WorkOrder flaggedWO;
        private string site;
        private AppInfo info;
        private List<List<string>> records;

        public NordexConverter(string s, AppInfo i)
        {
            site = s;
            info = i;
        }

        public void convertReport(Report report)
        { 
            records = report.getRecords("main");
            string id = getOrderNo();
            newWO = new WorkOrder(id);
            newWO.Site = info.getSite(site);
            newWO.Vendor = info.getVendor("Nordex");
            newWO.Status = "Closed";
            int countS = 1;
            int countC = 1;

            for (int i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                if (row[0].Equals("Date"))
                {
                    newWO.OpenDate = Convert.ToDateTime(row[1].Trim());
                } 
                else if (row[0].Contains("Start Date") && countS == 1)
                {
                    i = timeUsed((i + 1), id);
                    countS++;
                }
                else if (row[0].Contains("Oper.") && countC == 1)
                {
                    i = componentsUsed((i + 1), id);
                    countC++;
                }
            }
        }

        private DateTime toDate(string d)
        {
            int day = Convert.ToInt32(d.Substring(0, 2));
            int month = Convert.ToInt32(d.Substring(3, 2));
            int year = Convert.ToInt32(d.Substring(6, 4));
            DateTime date;
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

        private string getOrderNo()
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

        //DEAL WITH THIS SOOOOONNNNNN
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
        
        private int timeUsed(int i, string id)
        {
            Dictionary<string, int> fieldToCell = organizeFields(records[i-1]);
            DateTime def = new DateTime();
            while (!records[i][0].Contains("Oper. Ref."))
            {
                List<string> row = records[i];

                if(newWO.StartDate == def)
                {
                    DateTime s = toDate(row[fieldToCell["Start Date"]]);
                    newWO.StartDate = s;
                } 
                else 
                {
                    DateTime d = toDate(row[fieldToCell["Start Date"]]);
                    if (d < newWO.StartDate)
                    {
                        newWO.StartDate = d;
                    }
                }

                if (newWO.EndDate == def)
                {
                    newWO.EndDate = toDate(row[fieldToCell["End Date"]]);
                }
                else
                {
                    DateTime d = toDate(row[fieldToCell["End Date"]]);
                    if (d > newWO.EndDate)
                    {
                        newWO.StartDate = d;
                    }
                }
                if (newWO.Description == null)
                {
                    newWO.Description = row[fieldToCell["Description of"]];
                }
                else
                {
                    string d = row[fieldToCell["Description of"]];
                    if (d.Length > newWO.Description.Length)
                    {
                        newWO.Description = d;
                    }
                }
                if (newWO.Comments == null)
                {
                    newWO.Comments = row[fieldToCell["Longtext of"]];
                }
                else
                {
                    string c = row[fieldToCell["Longtext of"]];
                    if (c.Length > newWO.Comments.Length)
                    {
                        newWO.Comments = c;
                    }
                }
                string startTime = row[fieldToCell["Start Time"]];
                string endTime = row[fieldToCell["End Time"]];
                if (row[fieldToCell["Description of"]].Contains("Start/Stop"))
                {
                    newWO.DownTime += getHours(startTime, endTime);
                    newWO.ActualHours += getHours(startTime, endTime);
                }
                else
                {
                    newWO.ActualHours += getHours(startTime, endTime);
                }
                i++;
            }
            return (i-1);
        }

        private double getHours(string start, string end)
        {
            DateTime s = Convert.ToDateTime(start);
            DateTime e = Convert.ToDateTime(end);
            TimeSpan time = e - s;
            double hours = time.TotalHours;
            return hours;
        }

        private int componentsUsed(int i, string id)
        {
            Dictionary<string, int> fieldToCell = organizeFields(records[i-1]);
            while (!records[i][0].Contains("Measurement"))
            {
                List<string> record = records[i];
                string partNo = record[fieldToCell["Product Ref."]];
                double dQty = Convert.ToDouble(record[fieldToCell["Quantity"]]);
                int qty = Convert.ToInt32(dQty);
                string partID = newWO.Vendor.getPartID(partNo, qty);
                if (partID != null)
                {
                    newWO.addPart(partID, qty);
                }
                else
                {
                    string description = record[fieldToCell["Description"]];
                    partID = newWO.Vendor.addNewPart(partNo, qty, description);
                    newWO.addPart(partID, qty);
                }
                i++;
            }
            return (i - 1);
        }

        private int counterReadings(int i)
        {
            
            while (records[i].Count > 1)
            {
                i++;
            }
            return (i-1);
        }

        private void comments(int i)
        {
            for (int j = i; j < records.Count; j++)
            {
                
            }
        }

        public List<WorkOrder> getWorkOrders()
        {
            List<WorkOrder> newWOs = new List<WorkOrder>();
            newWO.createMPulseID();
            newWOs.Add(newWO);
            return newWOs;
        }

    }
}
