using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class SenvionConverter : Converter
    {
        private WorkOrder wo;
        private WorkOrder flaggedWO;
        private Site site;
        private AppInfo info;
        private Dictionary<string, int[]> fieldToCell;
        private Dictionary<string, List<string>> fieldNames;
        private List<List<string>> records;
        private PartsTable partsTable;
        private AssetTable aTable;
        private Vendor ven;
        private WOTable woTable;

        public SenvionConverter(string s, AppInfo i, PartsTable p, AssetTable a, WOTable woT)
        {
            info = i;
            site = i.getSite(s);
            partsTable = p;
            aTable = a;
            ven = info.getVendor("Senvion");
            fieldNames = ven.getFieldNames("Main");
            woTable = woT;
        }

        /* Start report conversion */
        public void convertReport(Report report)
        {
            records = report.getRecords("Main");
            organizeFields();
            int[] loc = fieldToCell["ID#"];
            //start a new work order
            wo = new WorkOrder(records[loc[0]+1][loc[1]], woTable, report.Filepath);
            wo.Site = site;
            wo.Vendor = ven;
            calcLaborHours(fieldToCell["Time"]);
            addDates();
            getDownTime();
            List<string> checkedVals = report.checkedVals();
            addWorkOrderInfo(checkedVals);
            loc = fieldToCell["Description"];
            wo.Description = records[loc[0]+2][loc[1]];
            loc = fieldToCell["Comments"];
            wo.Comments = records[loc[0]][loc[1]+1];
            wo.Status = "Closed";
            addParts();
            addAsset();
            //check if WO is valid, if not set as flagged WO
            Validator v = new Validator();
            if (!v.isValid(wo))
            {
                flaggedWO = wo;
                wo = null;
            }
        }

        /* Adds information based on work order type from
         * the table present in the AppInfo file*/
        private void addWorkOrderInfo(List<string> checkedVals)
        {
            //The work order type is the last checked value in the report
            int len = checkedVals.Count;
            string workOrderType = checkedVals[len - 1];
            //get the information linked to the workorder type
            List<string> taskInfo = info.getTypeInfo(workOrderType);
            wo.WorkOrderType = taskInfo[0];
            wo.TaskID = taskInfo[1];
            wo.TaskDescription = taskInfo[2];
            wo.OutageType = taskInfo[3];
            wo.Planning = taskInfo[4];
            wo.UnplannedType = taskInfo[5];
            wo.Priority = taskInfo[6];
        }

        /*Add asset to the work order*/
        private void addAsset()
        {
            int[] loc = fieldToCell["Asset"];
            string asset = records[loc[0] + 1][loc[1] + 2];
            /* If asset number isn't in a cell of its own, asset ID
             * and location must be in one cell*/
            if (asset.Equals(" "))
            {
                asset = records[loc[0] + 1][loc[1]];
            }
            List<string> a = aTable.getAssetID(asset, site.Name);
            wo.AssetID = a[0];
            wo.AssetDescription = a[1];
        }

        /* Calculate downtime*/
        private void getDownTime()
        {
            TimeSpan t = wo.EndDate - wo.StartDate;
            wo.DownTime = t.TotalHours;
        }

        /* Add the start, end and open dates to the work order */
        private void addDates()
        {
            int x = fieldToCell["Offline"][0];
            int y = fieldToCell["Offline"][1]+3;
            wo.StartDate = convertToDate(records[x][y]+","+records[x][y+1]);
            x = fieldToCell["Online"][0];
            y = fieldToCell["Online"][1]+3;
            wo.EndDate = convertToDate(records[x][y]+","+records[x][y+1]);
            x = fieldToCell["Date"][0];
            y = fieldToCell["Date"][1]+2;
            wo.OpenDate = convertToDate(records[x][y]);
        }
        
        /* Converts a string to a DateTime value*/
        private DateTime convertToDate(string s)
        {
            try
            {
                /* If the string is in date format it will be 
                 * converted into a DateTime value*/
                string val = s.Trim();
                DateTime result = Convert.ToDateTime(val);
                return result;
            }
            catch
            {
                /* If the string is not in a date format, the start
                 * date is returned*/
                return wo.StartDate;
            }
        }
        
        /* Calculate the total labor hours (actual hours) of 
         * a work order */
        private void calcLaborHours(int[] loc)
        {
            double result = 0;
            int i = loc[0] + 2, j = loc[1];
            while (!records[i][j].Equals(" "))
            {
                DateTime from = convertToTime(records[i][j]);
                DateTime until = convertToTime(records[i][j + 1]);
                double timeTaken = (until - from).TotalHours;
                result += timeTaken;
                i++;
            }
            wo.ActualHours = result;
        }

        /*Convert a string to DateTime*/
        private DateTime convertToTime(string s)
        {
            DateTime result;
            try
            {
                /*If the string is in a date format this will work*/
                result = Convert.ToDateTime(s);
            }
            catch
            {
                /*If the string is in a decimal format, use the
                 * fromOADate function instead*/
                double d = Convert.ToDouble(s);
                result = DateTime.FromOADate(d);
            }
            return result;
        }

        /* Fill in the fieldToCell dictionary with the 
         * exact cell location of each of the field headers */
        private void organizeFields()
        {
            fieldToCell = new Dictionary<string, int[]>();
            string key;
            
            for (int i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                for (int j = 0; j < row.Count; j++)
                {
                    key = isField(row[j]);
                    if (key!= null && !fieldToCell.ContainsKey(key))
                    {
                        fieldToCell.Add(key, new int[] {i, j});
                    }
                }
            }
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
                    if (name.Contains(field.ToLower()) && !fieldToCell.ContainsKey(key))
                    {
                        return key;
                    }
                }
            }
            return null;
        }

        /*Adding parts to the work order*/
        private void addParts()
        {
            int x = fieldToCell["Parts"][0] + 3;
            int y = fieldToCell["Parts"][1] + 1;

            /*While there are parts in that section of the report*/
            while (!records[x][y].Equals("") && !records[x][y].Equals(" "))
            {
                string id = records[x][y + 5];
                int qty = Convert.ToInt32(records[x][y + 6]);
                Part part = partsTable.getPart(id, wo.Vendor.Name, qty);
                if (part == null)
                {
                    /* If the part isn't in the parts list, create a new part*/
                    string description = records[x][y];
                    part = partsTable.addNewPart(id, qty, description, wo.Vendor);
                }
                wo.addPart(part, qty);
                x++;
            }
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
