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
        private Dictionary<string, Dictionary<string, Part>> parts;
        private Dictionary<string, Dictionary<string, Part>> newParts;
        private List<Vendor> vendors;

        public PartsTable(List<Vendor> v, string file)
        {
            vendors = v;
            newParts = new Dictionary<string, Dictionary<string, Part>>();
            foreach (Vendor ven in vendors)
            {
                Dictionary<string, Part> newPartTable = new Dictionary<string, Part>();
                newParts.Add(ven.Name, newPartTable);
            }
            startTable(file);
        }

        public void startTable(string file)
        {
            parts = new Dictionary<string, Dictionary<string, Part>>();
            foreach (Vendor ven in vendors)
            {
                Dictionary<string, Part> partTable = new Dictionary<string, Part>();
                parts.Add(ven.Name, partTable);
            }
            generateTable(file);
        }

        private void generateTable(string partsFile)
        {
            FileInfo newFile = new FileInfo(partsFile);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;
            ExcelWorksheet wk = ws[1];
            int oLoc = getLocation("Owned", wk);
            int sLoc = getLocation("Supplier_Part", wk);
            int dLoc = getLocation("Description", wk);
            Part p;

            int totalRows = wk.Dimension.End.Row;

            for (int i = 2; i <= totalRows; i++)
            {
                string id = wk.Cells[i, 1].Value.ToString();
                string ownedBy = wk.Cells[i, oLoc].Text;
                foreach (Vendor v in vendors)
                {
                    if (isVendor(id, ownedBy, v))
                    {
                        var partID = wk.Cells[i, sLoc].Value;
                        string ven = v.Name;
                        if (partID != null && !parts[ven].ContainsKey(partID.ToString()))
                        {
                            p = new Part(partID.ToString(), v);
                            p.ID = id;
                            p.Description = wk.Cells[i, dLoc].Value.ToString();
                            parts[ven].Add(partID.ToString(), p);
                        }
                    }
                }
            }
        }

        private int getLocation(string s, ExcelWorksheet wk)
        {
            int cols = wk.Dimension.End.Column;

            for (int i = 1; i <= cols; i++)
            {
                string val = wk.Cells[1, i].Value.ToString();
                if (val.Contains(s))
                {
                    return i;
                }
            }
            return -1;
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

        public Part getPart(string partID, string vendor, int qty)
        {
            //return the mpulse part number if it exists
            if (parts[vendor].ContainsKey(partID))
            {
                return parts[vendor][partID];
            }
            else if (newParts[vendor].ContainsKey(partID))
            {
                newParts[vendor][partID].Qty += qty;
                return newParts[vendor][partID];
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
            return parts[vendor][keys[len - 1]].ID;
        }

        public Part addNewPart(string id, int qty, string des, Vendor vendor)
        {
            Part newPart = new Part(id, vendor);
            newPart.generateID(newestPartID(vendor.Name));
            newPart.Qty += qty;
            newPart.Description = des;
            newParts[vendor.Name].Add(id, newPart);
            return newPart;
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
