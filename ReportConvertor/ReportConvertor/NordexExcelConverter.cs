using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class NordexExcelConverter : Converter
    {
        private Site site;
        private AppInfo info;
        private WorkOrder wo;
        private WorkOrder flaggedWO;
        private List<List<string>> records;
        private Dictionary<string, List<string>> fieldNames;
        private Dictionary<string, int[]> fieldToCell;
        private PartsTable partsTable;
        private AssetTable aTable;
        private Vendor ven;
        private WOTable woTable;

        public NordexExcelConverter(string s, AppInfo i, PartsTable p, AssetTable a, WOTable woT)
        {
            site = i.getSite(s);
            info = i;
            partsTable = p;
            aTable = a;
            ven = info.getVendor("Nordex");
            fieldNames = ven.getFieldNames("Main");
            woTable = woT;
        }

        /* Start report conversion */
        public void convertReport(Report report)
        {
            records = report.getRecords("Main");
            organizeFields();

            //Get the ID from the report to start a new WorkOrder
            string id = getID();
            wo = new WorkOrder(id, woTable);
            addWorkOrderInfo(report.checkedVals()[0]);
            

            //adding the asset to the work order
            int[] loc = fieldToCell["Asset"];
            string asset = records[loc[0]][loc[1] + 2];
            wo.AssetID = aTable.getAssetID(asset, site.Name);
            addDescription();
            addActualHours();
            addDownTime();
            addMaterials();
            addParts();
            addDates();

            Validator v = new Validator();
            if (!v.isValid(wo))
            {
                flaggedWO = wo;
                wo = null;
            }
        }

        /* Get the report ID */
        private string getID()
        {
            int[] loc = fieldToCell["ID"];
            int x = loc[0];
            int y = loc[1]+1;
            string id = " ";
            /* loop until the ID number is found. The cell
             * containing the ID number is not fixed in relation 
             * to the cell containing the header.*/
            while (id == " ")
            {
                id = records[x][y];
                y++;
            }
            return id;
        }

        /* Adds information based on work order type from
         * the table present in the AppInfo file */
        private void addWorkOrderInfo(string workOrderType)
        {
            List<string> taskInfo = info.getTypeInfo(workOrderType);
            wo.WorkOrderType = taskInfo[0];
            wo.TaskID = taskInfo[1];
            wo.OutageType = taskInfo[2];
            wo.Planning = taskInfo[3];
            wo.UnplannedType = taskInfo[4];
            wo.Priority = taskInfo[5];
            wo.Site = site;
            wo.Vendor = ven;
            wo.Status = "Closed";
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
                    if (!row[j].Equals(" ") && !row[j].Equals(""))
                    {
                        key = isField(row[j]);
                        if (key != null && !fieldToCell.ContainsKey(key))
                        {
                            fieldToCell.Add(key, new int[] { i, j });
                        }
                    }
                }
            }
        }

        /*
         * This function checks if a value in the 
         * report is a field header
         */
        private string isField(string s)
        {
            List<string> fieldKeys = fieldNames.Keys.ToList();
            string name = s.ToLower();
            //Checking for each field header
            foreach (string key in fieldKeys)
            {
                //Checking all the possible alternate header names
                foreach (string field in fieldNames[key])
                {
                    //making sure that the field hasn't been read already
                    if (name.Contains(field.ToLower()) && !fieldToCell.ContainsKey(key))
                    {
                        return key;
                    }
                }
            }
            return null;
        }

        /* Adds the down time to the work order*/
        private void addDownTime()
        {
            int i = fieldToCell["Down Time"][0]+2;
            List<string> row = records[i];
            while (!row[1].Equals(" "))
            {
                int last = fieldToCell["Stoppage"][1];
                //ensure that there are no empty spaces on either side of the string
                string time = row[last].Trim();
                wo.DownTime += convertToHours(time);
                i++;
                row = records[i];
            }
        }

        /* Adds the start, end and open dates of the work order */
        private void addDates()
        {
            int[] loc = fieldToCell["Open Date"];
            wo.OpenDate = Convert.ToDateTime(records[loc[0]][loc[1] + 1]);
            loc = fieldToCell["Start Date"];
            wo.StartDate = Convert.ToDateTime(records[loc[0]+1][loc[1]]);
            loc = fieldToCell["End Date"];
            wo.EndDate = Convert.ToDateTime(records[loc[0] + 1][loc[1]]);
        }

        /* Adds the description of the work order. The first sentence is 
         * put in the description field and the entire description is put
         * in the comments field */
        private void addDescription()
        {
            int x = fieldToCell["Description"][0]+1;
            int y = fieldToCell["Description"][1];
            string d = records[x][y];
            int i = d.IndexOf(".");
            string first;

            if (i > 0)
            {
                first = d.Substring(0, i+1);
            }
            else
            {
                first = d;
            }
            wo.Description = first;
            wo.Comments = d;
        }

        /* Calculating the labour/actual hours of the work order */
        private void addActualHours()
        {
            int start = fieldToCell["Actual Hours"][0]+2;
            // The exact location of the hours column
            int y = fieldToCell["Hrs"][1];
            /*The records for labor/actual hours only go up
             * till the description header in the reports */
            int end = fieldToCell["Description"][0];
            double total = 0;
            for (int i = start; i < end; i++)
            {
                string time = records[i][y].Trim();
                if (!time.Equals(""))
                {
                    total += convertToHours(time);
                }
            }

            wo.ActualHours = total;
        }

        /* Converts a string time to hours */
        private double convertToHours(string time)
        {
            //Get the hours and minutes in double format
            int count = time.IndexOf(":");
            double hour = Convert.ToDouble(time.Substring(0, count));
            double min = Convert.ToDouble(time.Substring(count + 1, 2));
            double total = hour + (min / 60);
            return total;
        }

        /* Adds materials to the work order */
        private void addMaterials()
        {
            int start = fieldToCell["Materials"][0]+3;
            int y = fieldToCell["Materials"][1];
            int end = fieldToCell["Parts"][0];

            for (int i = start; i < end; i++)
            {
                string id = records[i][y+2];
                if (!id.Equals(" "))
                {
                    int qty = Convert.ToInt32(records[i][y + 1]);
                    string partID = partsTable.getPartID(id, wo.Vendor.Name, qty);
                    if (partID == null)
                    {
                        /* If the part isn't in the parts list, create a new part*/
                        string description = records[i][y + 3];
                        partID = partsTable.addNewPart(id, qty, description, wo.Vendor);
                        wo.addPart(partID, qty);
                    }
                    wo.addPart(partID, qty);
                }
            }
        }

        /* Find and add all the parts that are
         * used in the work order*/
        private void addParts()
        {
            int start = fieldToCell["Parts"][0] + 2;
            int y = fieldToCell["Materials"][1];
            int end = fieldToCell["Tools"][0];

            for (int i = start; i < end; i++)
            {
                string id = records[i][y + 2];
                if (!id.Equals(" "))
                {
                    int qty = Convert.ToInt32(records[i][y + 1]);
                    string partID = partsTable.getPartID(id, wo.Vendor.Name, qty);
                    if (partID == null)
                    {
                        /* If the part isn't in the parts list, create a new part*/
                        string description = records[i][y + 3];
                        partID = partsTable.addNewPart(id, qty, description, wo.Vendor);
                        wo.addPart(partID, qty);
                    }
                    wo.addPart(partID, qty);
                }
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
