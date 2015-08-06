using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class SuzlonConverter : Converter
    {
        private WorkOrder wo;
        private WorkOrder flaggedWO;
        private AppInfo info;
        private Site site;
        private Dictionary<string, int[]> fieldToCell;
        private Dictionary<string, List<string>> fieldNames;
        private List<List<string>> records;
        private PartsTable partsTable;
        private AssetTable aTable;
        private Vendor ven;
        private WOTable woTable;

        public SuzlonConverter(String s, AppInfo i, PartsTable p, AssetTable a, WOTable woT)
        {
            info = i;
            site = info.getSite(s);
            partsTable = p;
            aTable = a;
            ven = info.getVendor("Suzlon");
            fieldNames = ven.getFieldNames("Main");
            woTable = woT;
        }

        /* Start report conversion */
        public void convertReport(Report report)
        {
            records = report.getRecords("Main");
            organizeFields();

            //No report ID is given so filler value is used
            wo = new WorkOrder("None", woTable, report.Filepath);
            wo.Site = site;
            wo.Vendor = ven;
            wo.Status = "Closed";
            addDateTime();
            addWorkOrderInfo();
            addAsset();

            //Add Description and Comments to WO
            int[] loc = fieldToCell["Description"];
            wo.Description = records[loc[0]][loc[1] + 1];
            wo.Comments = wo.Description;

            addParts();
            //Fill in any empty values
            wo.fillValues();
            //Check if the WO is valid
            Validator v = new Validator();
            if (!v.isValid(wo))
            {
                flaggedWO = wo;
                wo = null;
            }
        }

        /* Adds information based on work order type from
         * the table present in the AppInfo file */
        private void addWorkOrderInfo()
        {
            int[] loc = fieldToCell["Outage Type"];
            string woType = records[loc[0]][loc[1] + 1];
            if (!woType.Equals(" "))
            {
                List<string> taskInfo = info.getTypeInfo(woType);
                wo.WorkOrderType = taskInfo[0];
                wo.TaskID = taskInfo[1];
                wo.TaskDescription = taskInfo[2];
                wo.OutageType = taskInfo[3];
                wo.Planning = taskInfo[4];
                wo.UnplannedType = taskInfo[5];
                wo.Priority = taskInfo[6];
            }
        }
        /* Gets the MPulse asset ID based on the contractor asset ID
         * and adds it to the work order*/
        private void addAsset()
        {
            int[] loc = fieldToCell["Asset"];
            string asset = records[loc[0]][loc[1] + 1];
            List<string> a = aTable.getAssetID(asset, site.Name);
            wo.AssetID = a[0];
            wo.AssetDescription = a[1];
        }

        /* This function maps the fields to locations so
         * that when parsing data, the locations are already
         * pre-determined*/
        private void organizeFields()
        {
            fieldToCell = new Dictionary<string, int[]>();
            string key;

            for (int i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                key = isField(row[0]);
                if (key != null && !fieldToCell.ContainsKey(key))
                {
                    fieldToCell.Add(key, new int[] { i, 0 });
                }
            }
        }

        /* Checks if the string is a valid field header name*/
        private string isField(string s)
        {
            List<string> fieldKeys = fieldNames.Keys.ToList();
            string name = s.ToLower();
            if (name.Equals(" ")) return null;
            foreach (string key in fieldKeys)
            {
                foreach (string field in fieldNames[key])
                {
                    if (name.Contains(field.ToLower()))
                    {
                        return key;
                    }
                }
            }
            return null;
        }

        /* Find and add all the parts that are used in the work order*/
        private void addParts()
        {
            int[] loc = fieldToCell["Parts"];
            int i = loc[0]+1;
            int j = loc[1];
            int idLoc = j+1;
            int qLoc = idLoc+1;
            int desLoc = qLoc+2;

            while (!records[i][idLoc].Equals(" "))
            {
                string id = records[i][idLoc];
                double dQty = Convert.ToDouble(records[i][qLoc]);
                int qty = Convert.ToInt32(dQty);
                Part part = partsTable.getPart(id, wo.Vendor.Name, qty);
                if (part == null)
                {
                    /* If the part isn't in the parts list, create a new part*/
                    string description = records[i][desLoc];
                    part = partsTable.addNewPart(id, qty, description, wo.Vendor);                    
                }
                wo.addPart(part, qty);
                i++;
            }
        }

        /* Adds start, end, open dates and downtime of 
         * the report to the work order */
        private void addDateTime()
        {
            int[] loc = fieldToCell["Start Date"];
            wo.StartDate = Convert.ToDateTime(records[loc[0]][loc[1] + 1]);
            loc = fieldToCell["End Date"];
            wo.EndDate = Convert.ToDateTime(records[loc[0]][loc[1] + 1]);
            wo.OpenDate = wo.StartDate;
            loc = fieldToCell["Actual Hours"];
            wo.ActualHours = Convert.ToDouble(records[loc[0]][loc[1] + 1]);
            loc = fieldToCell["Down Time"];
            string downtime = records[loc[0]][loc[1] + 1];
            //If downtime isn't specified, it should be 0
            if (downtime.Equals(" "))
            {
                wo.DownTime = 0;
            }
            else
            {
                wo.DownTime = convertToDouble(records[loc[0]][loc[1] + 1]);
            }
        }

        /* Converts a string time to double */
        private double convertToDouble(string time)
        {
            double result=0;
            try
            {
                result = Convert.ToDouble(time);
            }
            catch
            {
                //Get the hours and minutes in double format
                int count = time.IndexOf(":");
                double hour = Convert.ToDouble(time.Substring(0, count));
                double min = Convert.ToDouble(time.Substring(count + 1, 2));
                result = hour + (min / 60);
            }
            return result;
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
