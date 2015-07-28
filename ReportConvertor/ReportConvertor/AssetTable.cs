using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class AssetTable
    {
        private Dictionary<string, Dictionary<string, string>> assets;
        List<Site> sites;

        public AssetTable(List<Site> s, string file)
        {
            sites = s;
            assets = new Dictionary<string, Dictionary<string, string>>();
            foreach (Site site in sites)
            {
                Dictionary<string, string> assetDict = new Dictionary<string, string>();
                assets.Add(site.Name, assetDict);
            }
            generateTable(file);
        }

        private void generateTable(string file)
        {
            FileInfo newFile = new FileInfo(file);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;

            foreach (Site s in sites)
            {
                ExcelWorksheet wk = ws[s.Name];
                int totalRows = wk.Dimension.End.Row;
                for (int i = 1; i <= totalRows; i++)
                {
                    //string loc = wk.Cells[i, 3].Value.ToString();
                    //if (loc.Equals(s))
                    {
                        string vendorID = wk.Cells[i, 1].Value.ToString();
                        string mpulseID = wk.Cells[i, 2].Value.ToString();
                        assets[s.Name].Add(vendorID, mpulseID);
                    }
                }
            }
        }

        public Dictionary<string, string> getAssets(string site)
        {
            return assets[site];
        }

        public string getAssetID(string vendorID, string site)
        {
            if (assets[site].ContainsKey(vendorID))
            {
                return assets[site][vendorID];
            }
            else return "";
        }
    }
}
