using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Diagnostics;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Data;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;


namespace ReportConvertor
{
    public class ExcelFileReader : FileReader
    {
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

        //NOTE: Figure out pass by reference
        public Tuple<string, Report> readFile(string file)
        {
            FileInfo newFile = new FileInfo(file);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;
            Report report = new Report();
            Tuple<string, ArrayList> tuple = null;
            
            int n = 1;

            foreach (ExcelWorksheet wk in ws){
                report.addWorksheet(wk.Name);
                report.changeCurrentTab(wk.Name);
                tuple = readWorksheet(wk);
                //add the records to the report
                report.addRecords(tuple.Item2);

                //if the vendor is senvion, there are checkboxes
                if (tuple.Item1.Equals("Twin Ridges") || tuple.Item1.Equals("Howard"))
                {
                    ArrayList vals = getCheckedValues(file, n);
                    report.addCheckedVals(vals);
                }
                n++;
            }

            return Tuple.Create(tuple.Item1, report);
        }

        private Tuple<string, ArrayList> readWorksheet(ExcelWorksheet worksheet)
        {
            int totalRows = worksheet.Dimension.End.Row;
            int totalCols = worksheet.Dimension.End.Column;
            string siteName = null;
            ArrayList wk = new ArrayList();
            ArrayList rows;

            for (int i = 1; i < totalRows; i++)
            {
                rows = new ArrayList();
                for (int j = 1; j < totalCols; j++)
                {
                    if (worksheet.Cells[i, j].Value != null)
                    {
                        if (siteName == null)
                        {
                            Tuple<bool, string> tuple = isNameOfSite(worksheet.Cells.Text);
                            if (tuple.Item1)
                            {
                                siteName = tuple.Item2;
                            }
                        }
                        rows.Add(worksheet.Cells.Text);
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
        public ArrayList getCheckedValues(string file, int n)
        {
            ArrayList checkedVals = new ArrayList();
            Application app = new Application();
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
                        //adding the text value of the checked checke
                        checkedVals.Add(shape.AlternativeText);
                    }
                }
            }

            //Important to close file as not done automatically
            wb.Close();

            return checkedVals;
        }

        private Tuple<bool, string> isNameOfSite(string n)
        {
            string name = n.ToLower();
            if (name.Contains("howard"))
            {
                return Tuple.Create(true, "Howard");
            }
            else if (name.Contains("highland 1"))
            {
                return Tuple.Create(true, "Highland 1");
            }
            else if (name.Contains("highland north"))
            {
                return Tuple.Create(true, "Highland North");
            }
            else if (name.Contains("patton"))
            {
                return Tuple.Create(true, "Patton");
            }
            else if (name.Contains("twin ridges"))
            {
                return Tuple.Create(true, "Twin Ridges");
            }
            else if (name.Contains("big sky"))
            {
                return Tuple.Create(true, "Big Sky"); ;
            }
            else if (name.Contains("mustang hills"))
            {
                return Tuple.Create(true, "Mustang Hills"); ;
            }
            return Tuple.Create(false, "");
        }
    }
}
