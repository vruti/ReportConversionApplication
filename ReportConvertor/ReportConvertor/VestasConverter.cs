﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class VestasConverter : Converter
    {
        private WorkOrder wo;
        private WorkOrder flaggedWO;
        private Site site;
        private Vendor vendor;
        private AppInfo info;
        private List<List<string>> records;
        private PartsTable partsTable;
        private AssetTable aTable;
        private WOTable woTable;

        public VestasConverter(string s, AppInfo i, PartsTable p, AssetTable a, WOTable woT)
        {
            info = i;
            site = i.getSite(s);
            aTable = a;
            vendor = i.getVendor("Vestas");
            partsTable = p;
            woTable = woT;
        }

        /* Start report conversion */
        public void convertReport(Report report)
        {
            records = report.getRecords("Main");
            int idLoc = getID();
            string id = records[idLoc][1];
            wo = new WorkOrder(id, woTable);
            string asset = records[idLoc][0];
            wo.AssetID = aTable.getAssetID(asset, site.Name);
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
                    addDates(i);
                }
                else if (row[0].Contains("Item"))
                {
                    addParts(i+1);
                }
                else if (row.Contains("Work") && row.Contains("Performed"))
                {
                    addDescription(i + 2);
                }
            }
            //Fill in any missing information
            wo.fillValues();

            //check if WO is valid, if not set as flagged WO
            Validator v = new Validator();
            if (!v.isValid(wo))
            {
                flaggedWO = wo;
                wo = null;
            }
        }

        /* Get the Report ID number */
        private int getID()
        {
            int len = records.Count;
            string id = null;
            int i;
            for (i = 0; i < len; i++)
            {
                List<string> row = records[i];
                if (row.Contains("Service") && row.Contains("Order:"))
                {
                    int loc = row.IndexOf("Service")/2;
                    row = records[i + 1];
                    id = row[loc];
                    break;
                }
            }
            Console.WriteLine(id);
            return i + 1;
        }

        /* Calculate and add downtime information */
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

        /* Add the start, end and open dates */
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

        /* Find and adds all parts to the work order */
        private void addParts(int i)
        {
            while (!records[i][0].Contains("_"))
            {
                string id = records[i][0];
                int len = records[i].Count;
                int qty = Convert.ToInt32(Convert.ToDouble(records[i][len-2]));
                string pID = partsTable.getPartID(id, vendor.Name, qty);
                if (pID == null)
                {
                    string description = records[i][1];
                    pID = partsTable.addNewPart(id, qty, description, wo.Vendor);
                }
                wo.addPart(pID, qty);
                i++;
            }
        }

        /* Adds the description of the work order. The first sentence is 
         * put in the description field and the entire description is put
         * in the comments field */
        private void addDescription(int i)
        {
            string d = "";
            for (int y = i; y < i + 2; y++)
            {
                List<string> row = records[y];
                for (int x = 1; x < row.Count; x++)
                {
                    d += row[x] + " ";
                }
                d += " ";
            }
            d = d.Trim();
            int loc = d.IndexOf(".");
            string first;

            if (loc > 0)
            {
                first = d.Substring(0, loc + 1);
            }
            else
            {
                first = d;
            }
            wo.Description = first;
            wo.Comments = d;
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
