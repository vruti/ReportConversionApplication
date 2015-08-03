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
        //private List<List<string>> vendorData;
        private PartsTable partsTable;
        private AssetTable aTable;
        private Vendor ven;

        public SenvionConverter(string s, AppInfo i, PartsTable p, AssetTable a)
        {
            info = i;
            site = i.getSite(s);
            partsTable = p;
            aTable = a;
            //vendorData = info.getVendorData("Senvion");
            //getFieldNames();
            ven = info.getVendor("Senvion");
            fieldNames = ven.getFieldNames("Main");
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("Main");
            organizeFields();
            int[] loc = fieldToCell["ID#"];
            wo = new WorkOrder(records[loc[0]+1][loc[1]]);
            wo.Site = site;
            wo.Vendor = info.getVendor("Senvion");
            calcLaborHours(fieldToCell["Time"]);
            addDates();
            getDownTime();
            List<string> checkedVals = report.checkedVals();
            int len = checkedVals.Count;
            string workOrderType = checkedVals[len - 1];
            List<string> taskInfo = info.getTypeInfo(workOrderType);
            wo.WorkOrderType = taskInfo[0];
            wo.TaskID = taskInfo[1];
            wo.OutageType = taskInfo[2];
            wo.Planning = taskInfo[3];
            wo.UnplannedType = taskInfo[4];
            wo.Priority = taskInfo[5];
            loc = fieldToCell["Description"];
            wo.Description = records[loc[0]+2][loc[1]];
            loc = fieldToCell["Comments"];
            wo.Comments = records[loc[0]][loc[1]+1];
            wo.Status = "Closed";
            addParts();
            addAsset();
            Validator v = new Validator();
            if (!v.isValid(wo))
            {
                flaggedWO = wo;
                wo = null;
            }
        }

        private void addAsset()
        {
            int[] loc = fieldToCell["Asset"];
            string asset = records[loc[0] + 1][loc[1] + 2];
            if (asset.Equals(" "))
            {
                asset = records[loc[0] + 1][loc[1]];
            }
            wo.AssetID = aTable.getAssetID(asset, site.Name);
        }

        private void getDownTime()
        {
            TimeSpan t = wo.EndDate - wo.StartDate;
            wo.DownTime = t.TotalHours;
        }

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

        private DateTime convertToDate(string s)
        {
            try
            {
                string val = s.Trim();
                DateTime result = Convert.ToDateTime(val);
                return result;
            }
            catch
            {
                return wo.StartDate;
            }
        }

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

        private DateTime convertToTime(string s)
        {
            DateTime result;
            try
            {
                result = Convert.ToDateTime(s);
            }
            catch
            {
                double d = Convert.ToDouble(s);
                result = DateTime.FromOADate(d);
            }
            return result;
        }

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
                        return key;
                    }
                }
            }
            return null;
        }
        /*
        private void getFieldNames()
        {
            fieldNames = new Dictionary<string, List<string>>();
            double l = Convert.ToDouble(vendorData[0][2]);
            int len = Convert.ToInt32(l) + 1;
            int cols;
            List<string> row;

            for (int i = 1; i < len; i++)
            {
                cols = vendorData[i].Count;
                row = new List<string>();
                for (int j = 1; j < cols; j++)
                {
                    if (!vendorData[i][j].Equals(" "))
                    {
                        row.Add(vendorData[i][j]);
                    }
                }
                fieldNames.Add(vendorData[i][0], row);
            }
        }
        */
        private void addParts()
        {
            int x = fieldToCell["Parts"][0] + 3;
            int y = fieldToCell["Parts"][1] + 1;

            while (!records[x][y].Equals(""))
            {
                string id = records[x][y + 5];
                int qty = Convert.ToInt32(records[x][y + 6]);
                string partID = partsTable.getPartID(id, wo.Vendor.Name, qty);
                if (partID == null)
                {
                    string description = records[x][y];
                    partID = partsTable.addNewPart(id, qty, description, wo.Vendor);
                }
                wo.addPart(partID, qty);
                x++;
            }
        }

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

        public List<WorkOrder> getFlaggedWO()
        {
            List<WorkOrder> flagged = new List<WorkOrder>();
            if (flaggedWO != null)
            {
                flaggedWO.createMPulseID();
                flagged.Add(flaggedWO);
            }
            return flagged;
        }
    }
}
