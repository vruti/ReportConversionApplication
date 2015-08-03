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
        //private List<List<string>> vendorData;
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
            //vendorData = info.getVendorData("EverPower");
            //addFieldNames();
            fieldNames = ven.getFieldNames("Main");
        }

        public void convertReport(Report report)
        {
            wo = new WorkOrder("temp ID");
            records = report.getRecords("Main");
            organizeFields();

            wo.Site = site;
            wo.Vendor = ven;
            wo.Description = records[fieldToCell["Description"]][1];
            string workOrderType = records[fieldToCell["Type"]][1];
            List<string> taskInfo = info.getTypeInfo(workOrderType);
            wo.WorkOrderType = taskInfo[0];
            wo.TaskID = taskInfo[1];
            wo.OutageType = taskInfo[2];
            wo.Planning = taskInfo[3];
            wo.UnplannedType = taskInfo[4];
            wo.Priority = taskInfo[5];
            DateTime date = toDate(records);
            wo.OpenDate = date;
            wo.EndDate = date;
            wo.StartDate = date;
            wo.DownTime = convertToDouble(fieldToCell["Down Time"]);
            wo.ActualHours = convertToDouble(fieldToCell["Actual Hours"]);
            wo.Comments = records[fieldToCell["Comments"]][1];
            wo.Status = "Closed";
            string asset = records[fieldToCell["Asset"]][1];
            wo.AssetID = aTable.getAssetID(asset, site.Name);
            addParts();
            Validator v = new Validator();
            if (!v.isValid(wo))
            {
                flaggedWO = wo;
                wo = null;
            }
        }

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

        private void addParts()
        {
            int i = fieldToCell["Parts"] + 2;
            while (!records[i][0].Equals(" "))
            {
                string id = records[i][0];
                int qty = Convert.ToInt32(records[i][3]);
                string partID = partsTable.getPartID(id, wo.Vendor.Name, qty);
                if (partID != null)
                {
                    wo.addPart(partID, qty);
                }
                else
                {
                    flaggedWO = wo;
                    wo = null;
                }
                i++;
            }
        }

        private void addNewParts(List<List<string>> rec)
        {
            int i = fieldToCell["Parts"] + 2;
            while (!rec[i][4].Equals(""))
            {
                string id = rec[i][4];
                string des= rec[i][5];
                int qty = Convert.ToInt32(rec[i][6]);
                string pID = partsTable.addNewPart(id, qty, des, wo.Vendor);
                wo.addPart(pID, qty);
            }
        }

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

        private string isField(string s)
        {
            if (s.Equals(" ")) return null;
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
        private void addFieldNames()
        {
            fieldNames = new Dictionary<string, List<string>>();
            double l = Convert.ToDouble(vendorData[0][2]);
            int len = Convert.ToInt32(l) + 1;
            int cols;
            List<string> row;

            for (int i = 0; i < len; i++)
            {
                cols = vendorData[i].Count;
                row = new List<string>();
                for (int j = 0; j < cols; j++)
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
        private DateTime toDate(List<List<string>> rec)
        {
            string date = rec[fieldToCell["Date"]][1];
            DateTime dateTime = Convert.ToDateTime(date);

            return dateTime;
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
