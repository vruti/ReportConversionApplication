using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class EVPConverter : Converter
    {
        private WorkOrder wo;
        private WorkOrder flaggedWO;
        private Dictionary<string, int> fieldToCell;
        private Site site;
        private AppInfo info;
        private Dictionary<string, List<string>> fieldNames;
        private List<List<string>> records;
        private PartsTable partsTable;
        private AssetTable aTable;
        private Vendor ven;
 
        public EVPConverter(string s, AppInfo i, PartsTable p, AssetTable a)
        {
            info = i;
            site = i.getSite(s);
            partsTable = p;
            aTable = a;
            fieldToCell = new Dictionary<string, int>();
            ven = site.Contractor;
            /*In this case, the field Names are stored in the 
             * EverPower Vendor object */
            Vendor tVen = info.getVendor("EverPower");
            fieldNames = tVen.getFieldNames("Main");
        }

        public void convertReport(Report report)
        {
            //No report ID is given so filler value is used
            wo = new WorkOrder("None");
            records = report.getRecords("Main");
            organizeFields();

            wo.Site = site;
            wo.Vendor = ven;
            wo.Description = records[fieldToCell["Description"]][1];
            addWorkOrderInfo();

            DateTime date = toDate();

            //Add dates to WO
            wo.OpenDate = date;
            wo.EndDate = date;
            wo.StartDate = date;

            //Add Downtime and Labor(Actual) hours
            wo.DownTime = convertToDouble(fieldToCell["Down Time"]);
            wo.ActualHours = convertToDouble(fieldToCell["Actual Hours"]);

            wo.Comments = records[fieldToCell["Comments"]][1];
            wo.Status = "Closed";

            //Add the asset attached to the work order
            string asset = records[fieldToCell["Asset"]][1];
            wo.AssetID = aTable.getAssetID(asset, site.Name);
            //Add parts
            addParts();
            addNewParts();

            //Check if the work order is valid
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
            string workOrderType = records[fieldToCell["Type"]][1];
            List<string> taskInfo = info.getTypeInfo(workOrderType);
            wo.WorkOrderType = taskInfo[0];
            wo.TaskID = taskInfo[1];
            wo.OutageType = taskInfo[2];
            wo.Planning = taskInfo[3];
            wo.UnplannedType = taskInfo[4];
            wo.Priority = taskInfo[5];
        }

        /* Converts a string time to double */
        private double convertToDouble(int x)
        {
            string downTime = records[x][1];
            int i = downTime.IndexOf(":");
            /* If the down time is in the hours format,
             * we replace the ':' with '.' so that it
             * can be converted to a double*/
            if (i > -1)
            {
                downTime = downTime.Replace(":", ".");
            }
            return Convert.ToDouble(downTime);
        }

        /* Find and add all the parts that are used in the work order*/
        private void addParts()
        {
            int i = fieldToCell["Parts"] + 2;
            while (!records[i][0].Equals("") && !records[i][0].Equals(" "))
            {
                string id = records[i][0];
                int qty = Convert.ToInt32(records[i][3]);
                string partID = partsTable.getPartID(id, wo.Vendor.Name, qty);
                //If the part is not found in the parts table, create a new part
                if (partID == null)
                {
                    string des = records[i][1];
                    partID = partsTable.addNewPart(id, qty, des, wo.Vendor);
                }
                wo.addPart(partID, qty);
                i++;
            }
        }

        /* Find and add all the new parts that are used in the work order*/
        private void addNewParts()
        {
            int i = fieldToCell["Parts"] + 2;
            while (!records[i][5].Equals("") && !records[i][5].Equals(" "))
            {
                string id = records[i][5];
                string des= records[i][6];
                int qty = Convert.ToInt32(records[i][7]);
                string pID = partsTable.addNewPart(id, qty, des, wo.Vendor);
                wo.addPart(pID, qty);
            }
        }

        /* This function maps the fields to locations so
         * that when parsing data, the locations are already
         * pre-determined*/
        private void organizeFields()
        {
            fieldToCell = new Dictionary<string, int>();
            string key;

            for (int i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                key = isField(row[0]);
                if (key != null && !fieldToCell.ContainsKey(key))
                {
                    fieldToCell.Add(key, i);
                }
            }
        }

        /* Checks if the string is a valid field header name*/
        private string isField(string s)
        {
            if (s.Equals(" ")) return null;
            List<string> fieldKeys = fieldNames.Keys.ToList();
            string name = s.ToLower();
            foreach (string key in fieldKeys)
            {
                foreach (string field in fieldNames[key])
                {
                    if (name.Contains(field.ToLower()) && !fieldToCell.ContainsKey(key))
                    {
                        return key;
                    }
                }
            }
            return null;
        }

        
        private DateTime toDate()
        {
            string date = records[fieldToCell["Date"]][1];
            DateTime dateTime = Convert.ToDateTime(date);

            return dateTime;
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
