using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    /* Contains all the necessary information for the 
     * application to run, like file locations, vendor
     * and site information. All the information is 
     * obtained from an Excel file, AppInfoSheet. 
     * 
     * This allows easy modification of the file to 
     * add or remove sites, vendors and file locations
     */
    public class AppInfo
    {
        private List<Vendor> vendors;
        private Dictionary<string, Site> sites;
        private Dictionary<string, string> fileLocs;
        private Dictionary<string, List<List<string>>> vendorData;
        private Dictionary<string, List<string>> assets;
        //private Dictionary<string, List<string>> fieldNames;
        private Dictionary<string, List<string>> woTypeTable;

        /* Initialize the object by opening the file with 
         * all the information. The file should probably be
         * located in the project folder, most likely debug
         * while I'm working on it and release for normal
         * functioning*/
        public AppInfo()
        {
            vendors = new List<Vendor>();
            sites = new Dictionary<string, Site>();
            fileLocs = new Dictionary<string, string>();
            vendorData = new Dictionary<string, List<List<string>>>();
            assets = new Dictionary<string, List<string>>();
            //fieldNames = new Dictionary<string, List<string>>();

            /* The AppinfoSheet file should always be located
             * in the specified location. File can be modified 
             * but never moved
             */
            string curDir = Environment.CurrentDirectory;
            string[] filePathsXlsx = Directory.GetFiles(@curDir, "*.xlsx");
            string path = filePathsXlsx[0];
            fileLocs.Add("AppInfo", path);
            FileInfo newFile = new FileInfo(path);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;

            Console.WriteLine(ws.Count);
            
            //read the individual tabs of the file
            addFileLoc(ws["FileLocations"]);
            addVendors(ws["Vendor General"]);
            addSites(ws["Site"]);
            addWOTypeTable(ws["WOType"]);
            addVendorData(ws);
        }

        // Reads all the file locations from the excel worksheet
        private void addFileLoc(ExcelWorksheet worksheet)
        {
            int rows = worksheet.Dimension.End.Row;

            for (int i = 2; i <= rows; i++)
            {
                //The file path fields should never be empty
                if (worksheet.Cells[i, 2].Value != null)
                {
                    fileLocs.Add(worksheet.Cells[i, 1].Value.ToString(), worksheet.Cells[i, 2].Value.ToString());
                }
            }
        }

        public Dictionary<string, string> getFileLocs()
        {
            return fileLocs;
        }

        // Get the location of file/directory specified
        public string getFileLoc(string n)
        {
            if (fileLocs.ContainsKey(n))
            {
                return fileLocs[n];
            }
            return null;
        }

        public void changeFileLoc(string f, string tab)
        {
            fileLocs[tab] = f;
        }

        /*Reads all the site information and creates
         * a site object to contain it, then puts that
         * object into a list for easy access
         */
        private void addSites(ExcelWorksheet wk)
        {
            int rows = wk.Dimension.End.Row;
            int cols = wk.Dimension.End.Column;
            Site site;
            string vendor;

            for (int i = 2; i <= rows; i++)
            {
                vendor = String.Format("{0}", wk.Cells[i, 2].Text);

                //create a site object
                site = new Site();
                //enter all necessary information
                site.Name = String.Format("{0}", wk.Cells[i,1].Text);
                site.ThreeLetterCode = String.Format("{0}", wk.Cells[i, 3].Text);
                site.FiveLetterCode = String.Format("{0}", wk.Cells[i, 4].Text);
                site.Contractor = (getVendor(vendor));
                //Adding any alternate names for site
                for (int j = 5; j <= cols; j++)
                {
                    if (wk.Cells[i, j].Value != null)
                    {
                        site.addAltName(String.Format("{0}", wk.Cells[i, j].Text));
                    }
                }
                //adding the new site to the list
                sites.Add(site.Name, site);
            }
        }

        //Get list of all the sites
        public List<Site> getSites()
        {
            List<Site> siteList = new List<Site>();
            List<string> keys = sites.Keys.ToList();
            foreach (string key in keys)
            {
                siteList.Add(sites[key]);
            }
            return siteList;
        }

        /* Getter class to retrieve a specific site
         * by name */
        public Site getSite(string name)
        {
            return sites[name];
        }

        /* Create a Vendor object for each of the vendors
         * listed out in the file with all the information
         * in the file for each vendor*/
        private void addVendors(ExcelWorksheet wk)
        {
            int rows = wk.Dimension.End.Row;

            Vendor v;

            for (int i = 2; i <= rows; i++)
            {
                v = new Vendor();

                //adding all the necessary fields
                v.ID = String.Format("{0}", wk.Cells[i, 1].Text);
                v.Name = String.Format("{0}", wk.Cells[i, 2].Text);
                v.ThreeLetterCode = String.Format("{0}", wk.Cells[i, 4].Text);
                v.FiveLetterCode = String.Format("{0}", wk.Cells[i, 5].Text);
                v.PartsTabNo = Convert.ToInt32(wk.Cells[i, 6].Value.ToString());
                v.WOArchiveTabNo = Convert.ToInt32(wk.Cells[i, 7].Value.ToString());
                v.PartsFile=(fileLocs["Parts"]);
                v.WOArchiveFile=(fileLocs["WOHistory"]);
                //v.generatePartsTable();
                v.generateWOHistory();
                //if there is an alternate name that we have to look for
                if (wk.Cells[i, 2].Value != null)
                {
                    v.addAltNames(String.Format("{0}", wk.Cells[i, 2].Text));
                }
                vendors.Add(v);
            }
        }

        /* finds the vendor object based on the input
         * string given */
        public Vendor getVendor(string n)
        {
            foreach (Vendor v in vendors)
            {
                if (v.Name.Equals(n))
                {
                    return v;
                }
            }
            //do some sort of error catch for this?
            return null;
        }

        public List<Vendor> getAllVendors()
        {
            return vendors;
        }

        /* reading the specific information about
         * a vendor by the tab */
        private void addVendorData(ExcelWorksheets ws)
        {
            foreach (Vendor v in vendors)
            {
                for (int i = 1; i <= ws.Count; i++)
                {
                    if (ws[i].Name.Equals(v.Name))
                    {
                        List<List<string>> wksht = new List<List<string>>();
                        ExcelWorksheet wk = ws[i];

                        int rows = wk.Dimension.End.Row;
                        int cols = wk.Dimension.End.Column;

                        List<string> row;
                        for (int j = 1; j <= rows; j++)
                        {
                            row = new List<string>();
                            for(int k = 1; k <= cols; k++)
                            {                       
                                if (wk.Cells[j, k].Value != null)
                                {
                                    row.Add(String.Format("{0}", wk.Cells[j, k].Text));
                                }
                                else
                                {
                                    row.Add(" ");
                                }
                            }
                            wksht.Add(row);
                        }
                        vendorData.Add(v.Name, wksht);
                    }
                }
            }
        }

        /* get vedor tab data by vendor*/
        public List<List<string>> getVendorData(string n)
        {
            if (vendorData.ContainsKey(n))
            {
                return vendorData[n];
            }
            return null;
        }

        private void addAssets()
        {
            if (fileLocs.ContainsKey("Assets"))
            {
                string dir = fileLocs["Assets"];

                FileInfo newFile = new FileInfo(dir);
                ExcelPackage pck = new ExcelPackage(newFile);
                ExcelWorksheet wk = pck.Workbook.Worksheets[1];
                List<string> names;

                int rows = wk.Dimension.End.Row;
                int cols = wk.Dimension.End.Column;

                for (int i = 1; i <= rows; i++)
                {
                    names = new List<string>();
                    for (int j = 2; j <= cols; j++)
                    {
                        if (wk.Cells[i, j].Value != null)
                        {
                            names.Add(wk.Cells[i, j].Value.ToString());
                        }
                    }
                    assets.Add(wk.Cells[i, 1].Value.ToString(), names);
                }

            }
        }

        public string getAssetID(string n)
        {
            List<string> keys = assets.Keys.ToList();
            foreach(string key in keys)
            {
                List<string> assetNames = assets[key];
                foreach (string asset in assetNames)
                {
                    if (n.ToLower().Contains(asset.ToLower()))
                    {
                        return key;
                    }
                }
            }
            return null;
        }

        private void addWOTypeTable(ExcelWorksheet wk)
        {
            woTypeTable = new Dictionary<string, List<string>>();
            List<string> values;

            int rows = wk.Dimension.End.Row;
            int cols = wk.Dimension.End.Column;

            for (int i = 2; i <= rows; i++)
            {
                values = new List<string>();
                for (int j = 2; j <= cols; j++)
                {
                    if (wk.Cells[i, j].Value != null)
                    {
                        values.Add(wk.Cells[i, j].Value.ToString());
                    }
                    else
                    {
                        values.Add(" ");
                    }
                }
                woTypeTable.Add(wk.Cells[i, 1].Value.ToString(), values);
            }
        }

        public List<string> getTypeInfo(string type)
        {
            return woTypeTable[type];
        }
    }
}
