using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
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
        private string dir;
        private string prtFile;
        private string wOArchive;
        private List<Vendor> vendors;
        private List<Site> sites;
        private Dictionary<string, string> fileLocs;

        public AppInfo()
        {
            vendors = new List<Vendor>();
            sites = new List<Site>();
            fileLocs = new Dictionary<string, string>();

            /* The AppinfoSheet file should always be located
             * in the specified location. File can be modified 
             * but never moved
             */
            string curDir = Environment.CurrentDirectory;
            string[] filePathsXlsx = Directory.GetFiles(@curDir, "*.xlsx");
            string path = filePathsXlsx[0];
            FileInfo newFile = new FileInfo(path);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;

            Console.WriteLine(ws.Count);

            getFileLoc(ws[1]);
            getVendors(ws[3]);
            getSites(ws[2]);
        }

        // Reads all the file locations from the excel worksheet
        private void getFileLoc(ExcelWorksheet worksheet)
        {
            int rows = worksheet.Dimension.End.Row;

            for (int i = 2; i <= rows; i++)
            {
                if (worksheet.Cells[i, 2].Value != null)
                {
                    fileLocs.Add(worksheet.Cells[i, 1].Value.ToString(), worksheet.Cells[i, 2].Value.ToString());
                }
                else
                {
                    fileLocs.Add(worksheet.Cells[i, 1].Value.ToString(), "  ");
                }
                Console.WriteLine(worksheet.Cells[i, 1].Value.ToString());
            }

        }

        /*Reads all the site information and creates
         * a site object to contain it, then puts that
         * object into a list for easy access
         */
        private void getSites(ExcelWorksheet wk)
        {
            int rows = wk.Dimension.End.Row;
            int cols = wk.Dimension.End.Column;
            Site site;
            string vendor;

            for (int i = 2; i <= rows; i++)
            {
                vendor = wk.Cells[i, 2].Value.ToString();

                //create a site object
                site = new Site();
                //enter all necessart information
                site.Name = wk.Cells[i,1].Value.ToString();
                site.ThreeLetterCode = wk.Cells[i,3].Value.ToString();
                site.FiveLetterCode = wk.Cells[i,4].Value.ToString();
                site.Contractor = (findVendor(vendor));

                for (int j = 5; j <= cols; j++)
                {
                    if (wk.Cells[i, j].Value != null)
                    {
                        site.addAltName(wk.Cells[i, j].Value.ToString());
                    }
                }
                //adding the new site to the list
                sites.Add(site);
            }
        }

        /* finds the vendor object based on the input
         * string given */
        private Vendor findVendor(string n)
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

        private void getVendors(ExcelWorksheet wk)
        {
            int rows = wk.Dimension.End.Row;

            Vendor v;

            for (int i = 2; i <= rows; i++)
            {
                

                v = new Vendor();

                //adding all the necessary fields
                v.IDNo = wk.Cells[i, 1].Value.ToString();
                v.Name = wk.Cells[i, 2].Value.ToString();
                v.ThreeLetterCode = wk.Cells[i, 4].Value.ToString();
                v.FiveLetterCode = wk.Cells[i, 5].Value.ToString();
                v.PartsTabNo = Convert.ToInt32(wk.Cells[i, 6].Value.ToString());
                v.WOArchiveTabNo = Convert.ToInt32(wk.Cells[i, 7].Value.ToString());
                v.PartsFile=(fileLocs["Parts"]);
                v.WOArchiveFile=(fileLocs["WOHistory"]);

                //if there is an alternate name that we have to look for
                if (wk.Cells[i, 2].Value != null)
                {
                    v.addAltNames(wk.Cells[i, 2].Value.ToString());
                }
                vendors.Add(v);
            }
        }

        public List<Site> getSites()
        {
            return sites;
        }

        public List<Vendor> getVendors()
        {
            return vendors;
        }

        public string getFileLoc(string s)
        {
            return fileLocs[s];
        }

    }
}
