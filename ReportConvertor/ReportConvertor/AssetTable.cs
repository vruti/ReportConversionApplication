using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    /*
     * This implementation of the asset table involves
     * a Dictionary with the site being the key, linking
     * to another dictionary with the contractor Asset ID as
     * the key linking to the MPulse Asset ID.
     */
    public class AssetTable
    {
        private Dictionary<string, Dictionary<string, string>> assets;
        private List<List<string>> unlinkedAssets;
        List<Site> sites;

        /*
         * Initialize all global values
         * Start the function that reads the 
         * Excel file for asset information
         */
        public AssetTable(List<Site> s, string file)
        {
            sites = s;
            assets = new Dictionary<string, Dictionary<string, string>>();
            unlinkedAssets = new List<List<string>>();
            /* Initialize a dictionary of contractor asset id 
             * to Mpulse asset id for each site
             */
            foreach (Site site in sites)
            {
                Dictionary<string, string> assetDict = new Dictionary<string, string>();
                assets.Add(site.Name, assetDict);
            }
            generateTable(file);
        }

        /*
         * Read the excel file containing all the asset information
         * and put the data into a dictionary with the contractor
         * asset id as the key, linking to the MPulse asset id
         */ 
        private void generateTable(string file)
        {
            FileInfo newFile = new FileInfo(file);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;

            //Each site has a asset dictionary
            foreach (Site s in sites)
            {
                ExcelWorksheet wk = ws[s.Name];
                int totalRows = wk.Dimension.End.Row;
                for (int i = 1; i <= totalRows; i++)
                {
                    //Value in the first column is the contractor asset ID
                    string vendorID = wk.Cells[i, 1].Value.ToString();
                    //Value in the second column is MPulse asset ID
                    string mpulseID = wk.Cells[i, 2].Value.ToString();
                    assets[s.Name].Add(vendorID, mpulseID);
                }
            }
        }

        /* Return the asset dictionary for a specific site
         */
        public Dictionary<string, string> getAssets(string site)
        {
            return assets[site];
        }

        /* 
         * Return the MPulse Asset ID from a given site with the
         * given input contractor asset ID
         */
        public string getAssetID(string aID, string site)
        {
            if (assets[site].ContainsKey(aID))
            {
                return assets[site][aID];
            }
            else
            {
                /* If the contractor asset ID doesn't exist in
                 * the dictionary, add it and the site to the 
                 * unlinked asset ID list to be written into the
                 * output excel file for user to handle
                 */
                List<string> unlinked = new List<string>();
                unlinked.Add(aID);
                unlinked.Add(site);
                unlinkedAssets.Add(unlinked);
                return "";
            }
        }

        /*
         * Return the list of the unlinked assets and 
         * their corresponding site names
         */
        public List<List<string>> getUnlinkedAssets()
        {
            return unlinkedAssets;
        }
    }
}
