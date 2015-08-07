using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Diagnostics;
using OfficeOpenXml;
using System.Data;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

namespace ReportConverter
{
    public class ExcelFileReader : FileReader
    {
        private AppInfo info;
        private ProgressBar pBar;

        public ExcelFileReader(AppInfo aInfo, ProgressBar pB)
        {
            info = aInfo;
            pBar = pB;
        }

        /* Reads all the files given */
        public Dictionary<string, List<Report>> readFiles(string[] files)
        {
            //Creating a dictionary to store all the records found in the files
            //being parsed
            Dictionary<string, List<Report>> reportsBySite = new Dictionary<string, List<Report>>();
            Tuple<string, Report> tuple;

            //Looping through all the files given
            foreach (string file in files)
            {
                tuple = readFile(file);
                //Adding a report by the site name
                if (reportsBySite.ContainsKey(tuple.Item1))
                {
                    reportsBySite[tuple.Item1].Add(tuple.Item2);
                }
                else
                {
                    List<Report> reportsList = new List<Report>();
                    reportsList.Add(tuple.Item2);
                    reportsBySite.Add(tuple.Item1, reportsList);
                }
                pBar.PerformStep();
            }
            return reportsBySite;
        }

        /* Reading a given excel file*/
        public Tuple<string, Report> readFile(string file)
        {
            FileInfo newFile = new FileInfo(file);
            XLSFileReader fr = new XLSFileReader(info, pBar);
            ExcelPackage pck;
            //If the file cannot be opened, try the .xls file reader
            try
            {
                pck = new ExcelPackage(newFile);
            }
            catch
            {
                return fr.readFile(file);
            }
            ExcelWorksheets ws = pck.Workbook.Worksheets;
            Report report = new Report();
            Tuple<string, List<List<string>>> tuple = null;
            string siteName = null;
            siteName = getNameOfSite(file);
            //Each worksheet is read
            foreach (ExcelWorksheet wk in ws)
            {
                report.addReportTab(wk.Name);
                report.changeCurrentTab(wk.Name);
                getNameOfSite(wk.Name);
                tuple = readWorksheet(wk);
                //add the records to the report
                report.addRecords(tuple.Item2);

                if (siteName == null)
                {
                    siteName = tuple.Item1;
                }

                //if the vendor is senvion, there are checkboxes
                if (siteName.Equals("Twin Ridges") || siteName.Equals("Howard"))
                {
                    List<string> vals = getCheckedValues(file, wk.Index);
                    report.addCheckedVals(vals);
                }
                if (!siteName.Equals("Patton") && siteName != null)
                {
                    break;
                }
            }
            report.Filepath = file;
            return Tuple.Create(siteName, report);
        }

        public Tuple<string, List<List<string>>> readWorksheet(ExcelWorksheet worksheet)
        {
            int totalRows = worksheet.Dimension.End.Row;
            int totalCols = worksheet.Dimension.End.Column;
            string siteName = null;
            List<List<string>> wk = new List<List<string>>();
            List<string> rows;

            //Read all the values in the worksheet
            for (int i = 1; i <= totalRows; i++)
            {
                rows = new List<string>();
                for (int j = 1; j <= totalCols; j++)
                {
                    if (worksheet.Cells[i, j].Value != null)
                    {
                        string val = String.Format("{0}", worksheet.Cells[i, j].Text);
                        if (siteName == null)
                        {
                            siteName = getNameOfSite(val);
                        }
                        rows.Add(val);
                    }
                    else
                    {
                        rows.Add(" ");
                    }
                }
                wk.Add(rows);
            }
            if (siteName == null)
            {
                //RAISE AN ERROR!!!!!!!!!!!
                //tell user
                //open file
                //ask for user input
            }

            return Tuple.Create(siteName, wk);
        }

        /**
         * A function that scans each worksheet for checkboxes and obtains their values. 
         * Cannot be done with same library used in the read functions so the file is 
         * opened again with functions from the Microsoft.Interop.Excel library and
         * the worksheet in question is scanned for any checkbox values
         **/
        public List<string> getCheckedValues(string file, int n)
        {
            List<string> checkedVals = new List<string>();
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbooks wbs = app.Workbooks;
            Workbook wb = wbs.Open(file);
            Worksheet wk = wb.Worksheets[n];
            //shapes involve all vba objects and pictures in the worksheet
            Shapes shapes = wk.Shapes;
            foreach (Shape shape in shapes)
            {
                //checking if the object is a checkbox
                if (shape.Name.Contains("Check"))
                {
                    //if the checkbox value is > 0 it is true
                    if (shape.OLEFormat.Object.Value > 0)
                    {
                        //adding the text value of the checked checked
                        string text = shape.AlternativeText;
                        if (text.Equals("") || text.Equals(" "))
                        {
                            Range c = shape.TopLeftCell;
                            int col = c.Column;
                            int row = c.Row;
                            text = String.Format("{0}", wk.Cells[row, (col + 1)].Text);
                        }
                        checkedVals.Add(text);
                    }
                }
            }

            //Important to close file as not done automatically
            wb.Close();

            return checkedVals;
        }

        public string getNameOfSite(string n)
        {
            List<Site> sites = info.getSites();

            foreach (Site s in sites)
            {
                if (s.isSite(n))
                {
                    return s.Name;
                }
            }
            return null;
        }
    }
}
