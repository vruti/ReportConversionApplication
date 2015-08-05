using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;

namespace ReportConverter
{
    public class XLSFileReader : FileReader
    {
        private AppInfo info;

        public XLSFileReader(AppInfo aInfo)
        {
            info = aInfo;
        }

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
            }
            return reportsBySite;
        }

        public Tuple<string, Report> readFile(string file)
        {
            Application app = new Application();
            app.DisplayAlerts = false;
            app.AutomationSecurity = Microsoft.Office.Core.MsoAutomationSecurity.msoAutomationSecurityForceDisable;
            Workbooks wbs = app.Workbooks;
            Workbook wb = wbs.Open(file, ReadOnly: true);
            wb.DoNotPromptForConvert = true;
            Worksheet wk = wb.Worksheets[1];
            Report report = new Report();
            Tuple<string, List<List<string>>> tuple = null;
            string siteName = null;
            siteName = getNameOfSite(file);

            report.addReportTab(wk.Name);
            report.changeCurrentTab(wk.Name);
            tuple = readWorksheet(wk, siteName);
            report.addRecords(tuple.Item2);

            if (siteName == null)
            {
                siteName = tuple.Item1;
            }

            //if the vendor is senvion, there are checkboxes
            List<string> vals = getCheckedValues(wk);
            report.addCheckedVals(vals);

            wb.Close();
            return Tuple.Create(siteName, report);
        }

        private Tuple<string, List<List<string>>> readWorksheet(Worksheet worksheet, string sName)
        {
            int totalRows = worksheet.UsedRange.Rows.Count;
            int totalCols = worksheet.UsedRange.Columns.Count;
            string siteName = sName;
            List<List<string>> wk = new List<List<string>>();
            List<string> rows;

            for (int i = 1; i <= totalRows; i++)
            {
                rows = new List<string>();
                for (int j = 1; j <= totalCols; j++)
                {
                    var v = worksheet.Cells[i, j].Value;
                    if (worksheet.Cells[i, j].Value != null)
                    {
                        
                        string val = String.Format("{0}", worksheet.Cells[i, j].Text);
                        if (val.Contains("#"))
                        {
                            val = String.Format("{0}", worksheet.Cells[i, j].Value2);
                        }
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
            return Tuple.Create(siteName, wk);
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

        public List<string> getCheckedValues(Worksheet wk)
        {
            List<string> checkedVals = new List<string>();
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
                            text = String.Format("{0}", wk.Cells[row, (col+1)].Text);
                        }
                        checkedVals.Add(text);
                    }
                }
            }
            return checkedVals;
        }
    }
}
