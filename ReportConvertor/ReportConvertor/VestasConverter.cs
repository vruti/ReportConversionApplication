using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class VestasConverter : Converter
    {
        private Dictionary<string, Part> newParts;
        private WorkOrder wo;
        //private WorkOrder flaggedWO;
        private Site site;
        private Vendor vendor;
        private AppInfo info;
        private List<List<string>> records;
        private PartsTable partsTable;

        public VestasConverter(string s, AppInfo i, PartsTable p)
        {
            info = i;
            site = i.getSite(s);
            vendor = i.getVendor("Vestas");
            partsTable = p;
            newParts = new Dictionary<string, Part>();
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("Main");
            string id = getID();
            wo = new WorkOrder(id);
            wo.Status = "Closed";
            wo.Site = site;
            wo.Vendor = vendor;

            for (int i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                if (row.Contains("Offline") && row.Contains("Date:"))
                {
                    addDownTime(i);
                }
                else if (row.Contains("Total") && row.Contains("Consumption:"))
                {
                    wo.ActualHours = Convert.ToDouble(row[3]);
                }
                else if (row.Contains("Start") && row.Contains("Date:"))
                {
                    addDates();
                }
            }
        }

        private string getID()
        {
            int len = records.Count;
            string id = null;
            for (int i = 0; i < len; i++)
            {
                List<string> row = records[i];
                if (row.Contains("Service") && row.Contains("Order:"))
                {
                    int loc = row.IndexOf("Service")/2;
                    row = records[i + 1];
                    id = row[loc];
                }
            }
            Console.WriteLine(id);
            return id;
        }
        /*
        private void rearrangeHeaders(List<string> headers)
        {
            int i;
            for (i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                if (isHeader(row[0], headers))
                {
                    List<string> newR = new List<string>();          
                    while (row.Count != 0)
                    {
                        string word = "";
                        foreach (string v in row)
                        {
                            if (v.Contains(":"))
                            {
                                word += v;
                                newR.Add(word);
                                word = "";
                            }
                            else
                            {
                                word += v + " ";
                            }
                        }
                    }
                    records[i] = newR;
                }
            }
        }

        private bool isHeader(string firstVal, List<string> indicators)
        {
            foreach (string val in indicators)
            {
                if (val.ToLower().Contains(firstVal))
                {
                    return true;
                }
            }
            return false;
        }
        */

        private void addDownTime(int i)
        {
            List<string> headers = records[i];
            List<string> values = records[i + 1];

            string offline = values[0] + "," + values[1];
            string online = values[2] + "," + values[3];
            DateTime offlineD = Convert.ToDateTime(offline);
            DateTime onlineD = Convert.ToDateTime(online);
            double downTime = (onlineD - offlineD).TotalHours;
            wo.DownTime = downTime;
        }

        private void addDates(int i)
        {
            List<string> values = records[i + 1];
            DateTime s = Convert.ToDateTime(values[1]);
            wo.StartDate = s;
            wo.OpenDate = s;
            if (values.Count < 3)
            {
                wo.EndDate = s;
            }
            else
            {
                wo.EndDate = Convert.ToDateTime(values[2]);
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

        public List<WorkOrder> getWorkOrders()
        {
            List<WorkOrder> wos = new List<WorkOrder>();
            wo.createMPulseID();
            wos.Add(wo);
            return wos;
        }
    }
}
