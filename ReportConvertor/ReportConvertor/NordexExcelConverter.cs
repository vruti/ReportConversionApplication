using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class NordexExcelConverter : Converter
    {
        private string site;
        private AppInfo info;
        private WorkOrder wo;
        //private WorkOrder flaggedWO;
        private Dictionary<string, Part> newParts;
        private List<List<string>> records;
        private Dictionary<string, List<string>> fieldNames;
        private Dictionary<string, int[]> fieldToCell;

        public NordexExcelConverter(string s, AppInfo i)
        {
            newParts = new Dictionary<string, Part>();
            site = s;
            info = i;
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("main");
            organizeFields();
            string id = getID();
            wo = new WorkOrder(id);
            wo.WorkOrderType = report.checkedVals()[0];
            wo.Site = info.getSite(site);
            wo.Vendor = info.getVendor("Nordex");
            wo.Status = "Closed";
            
        }

        private string getID()
        {
            string id = null;
            List<string> idFields = fieldNames["ID"];
            bool isID = false;
            for (int i = 0; i < records.Count; i++)
            {
                if (!isID)
                {
                    List<string> row = records[i];
                    for (int j = 0; j < row.Count; j++)
                    {
                        if (isID)
                        {
                            if (!row[j].Equals(" "))
                            {
                                id = row[j];
                                break;
                            }
                        }
                        else
                        {
                            foreach (string field in idFields)
                            {
                                if (row[j].ToLower().Contains(field.ToLower()))
                                {
                                    isID = true;
                                }
                            }
                        }
                    }
                }
            }
            return id;
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
                    if (key != null && !fieldToCell.ContainsKey(key))
                    {
                        fieldToCell.Add(key, new int[] { i, j });
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

        private void getFieldNames()
        {
            List<List<string>> data = info.getVendorData("Senvion");
            fieldNames = new Dictionary<string, List<string>>();
            int i, j;
            List<string> row;
            bool isFieldTable = false;

            for (i = 0; i < data.Count; i++)
            {
                row = data[i];
                if (row[0].Contains("Field Name"))
                {
                    isFieldTable = true;
                    i++;
                }
                if (row[0].Equals(" "))
                {
                    isFieldTable = false;
                    break;
                }
                if (isFieldTable)
                {
                    row = data[i];
                    List<string> fields = new List<string>();
                    for (j = 0; j < row.Count; j++)
                    {
                        if (!row[j].Equals(" "))
                        {
                            fields.Add(row[j].ToLower());
                        }
                    }
                    fieldNames.Add(row[0], fields);
                }
            }
        }

        private void getDownTime()
        {
            bool isStopTime = false;
            int i = fieldToCell["Stop Time"][0];
            List<string> row = records[i+1];
            while (!row[0].Equals(" "))
            {
                int last = row.Count - 1;
                double time = Convert.ToDouble(row[last]);
                wo.DownTime += time;
            }/*
            foreach (List<string> rowd in records)
            {
                if (isStopTime && row[1].Equals(" "))
                {
                    break;
                }
                if (isStopTime)
                {
                    
                }
                if (row[1].Contains("Turbine Stop"))
                {
                    isStopTime = true;
                }
            }*/
        }

        /*Find and add all the parts that are
         * used in the work order*/
        private void addParts(int start)
        {

        }

        /*Returns a list of the new parts in the
         * work order so that they can be put into 
         * the output file for upload*/
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
            List<WorkOrder> newWOs = new List<WorkOrder>();
            wo.createMPulseID();
            newWOs.Add(wo);
            return newWOs;
        }
    }
}
