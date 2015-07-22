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
        private string site;
        private AppInfo info;
        private Dictionary<string, List<string>> fieldNames;
        private List<List<string>> records;
        private List<List<string>> vendorData;
        private Dictionary<string, int> tableLoc;
        private Dictionary<string, List<string>> table;
        private PartsTable partsTable;
 
        public EVPConverter(string s, AppInfo i, PartsTable p)
        {
            info = i;
            site = s;
            partsTable = p;
            fieldToCell = new Dictionary<string, int>();
            vendorData = info.getVendorData("EverPower");
            getTableLoc();
            getFieldNames();
            getTableData();
        }

        public void convertReport(Report report)
        {
            wo = new WorkOrder("temp ID");
            records = report.getRecords("Main");
            organizeFields();

            wo.Site = info.getSite(site);
            wo.Vendor = wo.Site.Contractor;
            wo.Description = records[fieldToCell["Description"]][1];
            wo.WorkOrderType = records[fieldToCell["Type"]][1];
            List<string> taskInfo = table[wo.WorkOrderType];
            wo.OutageType = taskInfo[0];
            wo.Priority = taskInfo[1];
            wo.Planning = taskInfo[2];
            wo.UnplannedType = taskInfo[3];
            wo.TaskID = taskInfo[4];
            DateTime date = toDate(records);
            wo.OpenDate = date;
            wo.EndDate = date;
            wo.StartDate = date;
            wo.DownTime = Convert.ToDouble(records[fieldToCell["Down Time"]][1]);
            wo.ActualHours = Convert.ToDouble(records[fieldToCell["Actual Hours"]][1]);
            wo.Comments = records[fieldToCell["Comments"]][1];
            wo.Status = "Closed";
            string asset = records[fieldToCell["Asset"]][1];
            wo.AssetID = wo.Vendor.getAssetID(asset);
            addParts();
            Validator v = new Validator();
            if (!v.isValid(wo))
            {
                flaggedWO = wo;
                wo = null;
            }
        }

        private void addParts()
        {
            int i = fieldToCell["Parts"] + 2;
            while (!records[i][0].Equals(" "))
            {
                string id = records[i][0];
                int qty = Convert.ToInt32(records[i][2]);
                string partID = partsTable.getPartID(id, wo.Vendor.Name, qty);
                //string partID = wo.Vendor.getPartID(id, qty);
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
                //Part p = new Part(id, wo.Vendor);
                string des= rec[i][5];
                int qty = Convert.ToInt32(rec[i][6]);
                string pID = partsTable.addNewPart(id, qty, des, wo.Vendor);

                //p.generateID(wo.Vendor.newestPartID());
                wo.addPart(pID, qty);
            }
        }

        private void organizeFields()
        {
            fieldToCell = new Dictionary<string, int>();
            getFieldNames();
            string key;

            for (int i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                //for (int j = 0; j < row.Count; j++)
                {
                    key = isField(row[0]);
                    if (key != null && !fieldToCell.ContainsKey(key))
                    {
                        fieldToCell.Add(key, i);
                    }
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

        private void getFieldNames()
        {
            fieldNames = new Dictionary<string, List<string>>();
            int start = tableLoc["Fields"] - 1;
            int len = Convert.ToInt32(vendorData[start][2]);
            start++;
            int cols;
            List<string> row;

            for (int i = start; i < start + len; i++)
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

        private void getTableLoc()
        {
            tableLoc = new Dictionary<string, int>();
            int i = 1;
            int loc;
            string n;

            while (!vendorData[i][0].Equals(" "))
            {
                loc = Convert.ToInt32(vendorData[i][1]);
                n = vendorData[i][0];
                tableLoc.Add(n.Trim(), loc);
                i++;
            }
        }

        private void getTableData()
        {
            table = new Dictionary<string, List<string>>();
            int start = tableLoc["Table"];
            start -= 1;
            int len = Convert.ToInt32(vendorData[start][1]);
            start += 2;
            int cols;

            for (int i = start; i < start + len; i++)
            {
                List<string> line = vendorData[i];
                cols = line.Count;
                List<string> row = new List<string>();
                for (int j = 1; j < cols; j++)
                {
                    row.Add(line[j]);
                }
                table.Add(line[0], row);
            }
        }

        private DateTime toDate(List<List<string>> rec)
        {
            string date = rec[fieldToCell["Date"]][1];
            DateTime dateTime = Convert.ToDateTime(date);

            return dateTime;
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
