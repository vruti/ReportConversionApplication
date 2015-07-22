using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class PartsTable
    {
        private Dictionary<string, Dictionary<string, string>> parts;
        private Dictionary<string, Dictionary<string, Part>> newParts;
        private List<Vendor> vendors;

        public PartsTable(List<Vendor> v, string file)
        {
            vendors = v;
            parts = new Dictionary<string, Dictionary<string, string>>();
            newParts = new Dictionary<string, Dictionary<string, Part>>();
            foreach (Vendor ven in vendors)
            {
                Dictionary<string, string> partTable = new Dictionary<string, string>();
                parts.Add(ven.Name, partTable);
                Dictionary<string, Part> newPartTable = new Dictionary<string, Part>();
                newParts.Add(ven.Name, newPartTable);
            }
            generateTable(file);
        }

        private void generateTable(string partsFile)
        {
            FileInfo newFile = new FileInfo(partsFile);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;

            /*chosing the tab number in the file based
             * on the contractor name
             */
            ExcelWorksheet wk = ws[1];

            int totalRows = wk.Dimension.End.Row;

            for (int i = 1; i <= totalRows; i++)
            {
                string id = wk.Cells[i, 1].Value.ToString();
                string ownedBy = wk.Cells[i, 11].Text;
                foreach (Vendor v in vendors)
                {
                    if (isVendor(id, ownedBy, v))
                    {
                        var partID = wk.Cells[i, 8].Value;
                        string ven = v.Name;
                        if (partID != null && !parts[ven].ContainsKey(partID.ToString()))
                        {
                            parts[ven].Add(partID.ToString(), id);
                        }
                    }
                }
            }
        }

        private bool isVendor(string id, string s, Vendor v)
        {
            string ownedBy = s.ToLower();
            string name = v.Name;
            string threeL = v.ThreeLetterCode;
            string fiveL = v.FiveLetterCode;
            List<string> altNames = v.getAltNames();
            if (ownedBy.Contains(name)) return true;
            if (id.Contains(fiveL) || id.Contains(threeL)) return true;
            foreach (string alt in altNames)
            {
                if (ownedBy.Contains(alt.ToLower())) return true;
            }
            return false;
        }

        public string getPartID(string partID, string vendor, int qty)
        {
            //return the mpulse part number if it exists
            if (parts[vendor].ContainsKey(partID))
            {
                return parts[vendor][partID];
            }
            else if (newParts[vendor].ContainsKey(partID))
            {
                newParts[vendor][partID].Qty += qty;
                return newParts[vendor][partID].ID;
            }
            /*return null if it doesn't convertor should 
             * create a new part to be uploaded*/
            return null;
        }

        private string newestPartID(string vendor)
        {
            int len;
            if (newParts[vendor].Count > 0)
            {
                List<string> k = newParts[vendor].Keys.ToList();
                len = k.Count;
                return newParts[vendor][k[len - 1]].ID;
            }
            List<string> keys = parts[vendor].Keys.ToList();
            len = keys.Count;
            return parts[vendor][keys[len - 1]];
        }

        public string addNewPart(string id, int qty, string des, Vendor vendor)
        {
            Part newPart = new Part(id, vendor);
            newPart.generateID(newestPartID(vendor.Name));
            newPart.Qty += qty;
            newPart.Description = des;
            newParts[vendor.Name].Add(id, newPart);
            return newPart.ID;
        }

        public List<Part> getNewParts()
        {
            List<Part> newPartsList = new List<Part>();
            List<string> venKeys = newParts.Keys.ToList();
            foreach (string venKey in venKeys)
            {
                List<string> pKeys = newParts[venKey].Keys.ToList();
                foreach (string key in pKeys)
                {
                    newPartsList.Add(newParts[venKey][key]);
                }
            }
            return newPartsList;
        }
    }
}
