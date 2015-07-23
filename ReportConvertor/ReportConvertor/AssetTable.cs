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
        List<string> sites;

        public AssetTable(List<string> s, string file)
        {
            sites = s;
            assets = new Dictionary<string, Dictionary<string, string>>();
            foreach (string site in sites)
            {
                Dictionary<string, string> assetDict = new Dictionary<string, string>();
                assets.Add(site, assetDict);
            }
            generateTable(file);
        }

        private void generateTable(string file)
        {
            FileInfo newFile = new FileInfo(file);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;

            foreach (string s in sites)
            {
                ExcelWorksheet wk = ws[s];
                int totalRows = wk.Dimension.End.Row;
                for (int i = 1; i <= totalRows; i++)
                {
                    string loc = wk.Cells[i, 3].Value.ToString();
                    if (loc.Equals(s))
                    {
                        string vendorID = wk.Cells[i, 1].Value.ToString().ToLower();
                        string mpulseID = wk.Cells[i, 2].Value.ToString();
                        assets[s].Add(vendorID, mpulseID);
                    }
                }
            }
        }

        public Dictionary<string, string> getAssets(string site)
        {
            return assets[site];
        }
    }
}
