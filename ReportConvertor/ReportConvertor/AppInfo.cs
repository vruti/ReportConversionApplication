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
        private Dictionary<string, List<string>> woTypeTable;

        /* Initialize the object by opening the file with 
         * all the information. The file should probably be
         * located in the project folder, most likely debug
         * while I'm working on it and release for normal
         * functioning
         */
        public AppInfo()
        {
            vendors = new List<Vendor>();
            sites = new Dictionary<string, Site>();
            fileLocs = new Dictionary<string, string>();
            vendorData = new Dictionary<string, List<List<string>>>();
            assets = new Dictionary<string, List<string>>();

            /* The AppinfoSheet file should always be located
             * in the specified location. File can be modified 
             * but never moved
             */
            string curDir = Environment.CurrentDirectory;
            string[] filePathsXlsx = Directory.GetFiles(@curDir, "*.xlsx");
            string path = filePathsXlsx[0];
            fileLocs.Add("AppInfo", path);

            //Open the file with the information
            FileInfo newFile = new FileInfo(path);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;
            
            //Read the individual tabs of the file
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
                var filepath = worksheet.Cells[i, 2].Value;
                //The file path fields should never be empty
                if (filepath != null)
                {
                    //Adding the location of file to dictionary
                    fileLocs.Add(worksheet.Cells[i, 1].Value.ToString(), filepath.ToString());
                }
            }
        }

        /*
         * Returns the dictionary containing the locations
         * of all the files needed for the program to run
         * smoothly
         */
        public Dictionary<string, string> getFileLocs()
        {
            return fileLocs;
        }

        /*
         * Get the location of file/directory specified
         */
        public string getFileLoc(string n)
        {
            if (fileLocs.ContainsKey(n))
            {
                return fileLocs[n];
            }
            return null;
        }

        /*
         * Change a file location if new file location is
         * given by the user on the settings page
         */
        public void changeFileLoc(string f, string tab)
        {
            fileLocs[tab] = f;
        }

        /* 
         * Reads all the site information and creates
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
                v.generateWOHistory();
                //if there is an alternate name that we have to look for
                if (wk.Cells[i, 2].Value != null)
                {
                    v.addAltNames(String.Format("{0}", wk.Cells[i, 2].Text));
                }
                vendors.Add(v);
            }
        }

        /* 
         * Finds the vendor object based on the input
         * string given 
         */
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

        /*
         * Return the list of all the vendors
         */
        public List<Vendor> getAllVendors()
        {
            return vendors;
        }

        /* 
         * Reading information about field header names
         * and adding to each vendor
         */
        private void addVendorData(ExcelWorksheets ws)
        {
            int len, start;
            foreach (Vendor v in vendors)
            {
                Dictionary<string, Dictionary<string, List<string>>> fields;
                ExcelWorksheet wk = ws[v.Name];
                if (v.Name.Equals("Gamesa"))
                {
                    fields = new Dictionary<string,Dictionary<string,List<string>>>();
                    int l = Convert.ToInt32(wk.Cells[1, 3].Value.ToString()) + 2;
                    for (int i = 2; i < l; i++)
                    {
                        start = Convert.ToInt32(wk.Cells[i, 2].Value.ToString());
                        string tab = wk.Cells[start, 3].Value.ToString();
                        len = Convert.ToInt32(wk.Cells[start, 2].Value.ToString());
                        start++;
                        fields.Add(tab, addFields(wk, start, len));
                    }
                    v.addFieldNames(fields);
                }
                else
                {
                    fields = new Dictionary<string, Dictionary<string, List<string>>>();
                    len = Convert.ToInt32(wk.Cells[1, 3].Value.ToString());
                    start = 2;
                    fields.Add("Main", addFields(wk, start, len));
                }
            }
        }

        /*
         * Reading the field header name and alternate names from the excel worksheet
         * and adding it to a dictionary by main field header name
         */
        private Dictionary<string, List<string>> addFields(ExcelWorksheet wk, int start, int len)
        {
            Dictionary<string, List<string>> fieldNames = new Dictionary<string, List<string>>();
            int totalCols = wk.Dimension.End.Column;
            List<string> fieldRow;

            for (int i = start; i < start + len; i++)
            {
                fieldRow = new List<string>();
                for (int j = 2; j < totalCols; j++)
                {
                    var cell = wk.Cells[i, j].Value;
                    if (cell != null)
                    {
                        fieldRow.Add(cell.ToString());
                    }
                }
                fieldNames.Add(wk.Cells[i, 1].Value.ToString(), fieldRow);
            }
            return fieldNames;
        }

        /*
         * Creating the table of information that is linked
         * to the work order type
         */
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

        /* 
         * Retrieve the information that is linked to
         * a specific work order type
         */
        public List<string> getTypeInfo(string type)
        {
            return woTypeTable[type];
        }
    }
}
