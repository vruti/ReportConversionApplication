using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class NordexConverter : Converter
    {
        private Dictionary<string, Part> newParts;
        private Dictionary<string, WorkOrder> newWOs;
        private Dictionary<string, WorkOrder> flaggedWO;
        private string site;
        AppInfo info;

        public NordexConverter(string s, AppInfo i)
        {
            newParts = new Dictionary<string,Part>();
            newWOs = new Dictionary<string, WorkOrder>();
            flaggedWO = new Dictionary<string, WorkOrder>();
            site = s;
            info = i;
        }

        public void convertReport(Report report)
        {
            List<List<string>> rec = report.getRecords("main");
            string id = getOrderNo(rec);
            WorkOrder wo = new WorkOrder(id);
            wo.Site=info.getSite(site);
            wo.Vendor = info.getVendor("Nordex");
            wo.Status = "Closed";
            newWOs.Add(id, wo);
            int countS = 1;
            int countC = 1;

            for (int i = 0; i < rec.Count; i++)
            {
                List<string> row = rec[i];
                if (row[0].Equals("Date"))
                {
                    wo.OpenDate = toDate(row[1].Trim());
                } 
                else if (row[0].Contains("Start Date") && countS == 1)
                {
                    i = timeUsed(rec, (i + 1), id);
                    countS++;
                }
                else if (row[0].Contains("Oper.") && countC == 1)
                {
                    i = componentsUsed(rec, (i + 1), id);
                    countC++;
                }
            }


        }

        private DateTime toDate(string date)
        {
            int day = Convert.ToInt32(date.Substring(0, 2));
            int month = Convert.ToInt32(date.Substring(3, 2));
            int year = Convert.ToInt32(date.Substring(6, 4));
            DateTime dateTime = new DateTime(year, month, day);

            return dateTime;
        }

        private string getOrderNo(List<List<string>> rec)
        {
            foreach (List<string> row in rec)
            {
                if (row[0].Contains("Order No."))
                {
                    return row[1];
                }
            }
            return null;
        }

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

        private int timeUsed(List<List<string>> records, int i, string id)
        {
            Dictionary<string, int> fieldToCell = organizeFields(records[i-1]);
            WorkOrder wo = newWOs[id];
            DateTime def = new DateTime();
            while (!records[i][0].Contains("Oper. Ref."))
            {
                List<string> row = records[i];

                if(wo.StartDate == def)
                {
                    wo.StartDate = toDate(row[fieldToCell["Start Date"]]);
                } else 
                {
                    DateTime d = toDate(row[fieldToCell["Start Date"]]);
                    if (d < wo.StartDate)
                    {
                        wo.StartDate = d;
                    }
                }

                if (wo.EndDate == def)
                {
                    wo.EndDate = toDate(row[fieldToCell["End Date"]]);
                }
                else
                {
                    DateTime d = toDate(row[fieldToCell["End Date"]]);
                    if (d > wo.EndDate)
                    {
                        wo.StartDate = d;
                    }
                }
                if (wo.Description == null)
                {
                    wo.Description = row[fieldToCell["Description of"]];
                }
                else
                {
                    string d = row[fieldToCell["Description of"]];
                    if (d.Length > wo.Description.Length)
                    {
                        wo.Description = d;
                    }
                }
                if (wo.Comments == null)
                {
                    wo.Comments = row[fieldToCell["Longtext of"]];
                }
                else
                {
                    string c = row[fieldToCell["Longtext of"]];
                    if (c.Length > wo.Comments.Length)
                    {
                        wo.Comments = c;
                    }
                }
                string startTime = row[fieldToCell["Start Time"]];
                string endTime = row[fieldToCell["End Time"]];
                if (row[fieldToCell["Description of"]].Contains("Start/Stop"))
                {
                    wo.DownTime += getHours(startTime, endTime);
                }
                else
                {
                    wo.ActualHours += getHours(startTime, endTime);
                }
                i++;
            }
            newWOs[id] = wo;
            return (i-1);
        }

        private double getHours(string start, string end)
        {
            double sHour = Convert.ToDouble(start.Substring(0, 2));
            double sMin = Convert.ToDouble(start.Substring(3, 2));

            double eHour = Convert.ToDouble(end.Substring(0, 2));
            double eMin = Convert.ToDouble(end.Substring(3, 2));

            double time = (eHour - sHour) + ((eMin - sMin) / 60);
            return time;
        }

        private int componentsUsed(List<List<string>> records, int i, string id)
        {
            WorkOrder wo = newWOs[id];
            Dictionary<string, int> fieldToCell = organizeFields(records[i-1]);
            while (!records[i][0].Contains("Measurement"))
            {
                List<string> record = records[i];
                string partNo = record[fieldToCell["Product Ref."]];
                string partID = wo.Vendor.getPartID(partNo);
                double dQty = Convert.ToDouble(record[fieldToCell["Quantity"]]);
                int qty = Convert.ToInt32(dQty);
                if (partID != null)
                {
                    wo.addPart(partID, qty);
                }
                else
                {
                    if (newParts.ContainsKey(partNo))
                    {
                        partID = newParts[partNo].ID;
                        wo.addPart(partID, qty);
                    }
                    else
                    {
                        Part newPart = new Part(partNo, wo.Vendor);
                        if (newParts.Count > 1)
                        {
                            List<string> k = newParts.Keys.ToList();
                            int len = k.Count;
                            newPart.generateID(newParts[k[len - 1]].ID);
                        }
                        else
                        {
                            newPart.generateID(newWOs[id].Vendor.newestPartID());
                        }
                        newPart.Qty = qty;
                        newPart.Description = record[fieldToCell["Description"]];
                        newParts.Add(partNo, newPart);
                        newWOs[id].addPart(newPart.ID, qty);
                    }
                }
                i++;
            }
            return (i - 1);
        }

        private int counterReadings(List<List<string>> records, int i)
        {
            
            while (records[i].Count > 1)
            {
                i++;
            }
            return (i-1);
        }

        private void comments(List<List<string>> records, int i)
        {
            for (int j = i; j < records.Count; j++)
            {
                
            }
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

        public Dictionary<string, WorkOrder> getWorkOrders()
        {
            return newWOs;
        }

    }
}
