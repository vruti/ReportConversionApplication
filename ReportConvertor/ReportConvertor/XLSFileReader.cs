using Excel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private IExcelDataReader getExcelDataReader(string file)
        {
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read);

            IExcelDataReader reader = null;
            try
            {
                if (file.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                return reader;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Tuple<string, Report> readFile(string file)
        {
            Application app = new Application();
            Workbooks wbs = app.Workbooks;
            Workbook wb = wbs.Open(file);

            //Data Reader methods
            foreach (DataTable table in result.Tables)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    for (int j = 0; j < table.Columns.Count; j++)
                        Console.Write("\"" + table.Rows[i].ItemArray[j] + "\";");
                    Console.WriteLine();
                }
            }
            
            //////////////////////////
            FileInfo newFile = new FileInfo(file);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;
            Report report = new Report();
            Tuple<string, List<List<string>>> tuple = null;
            string siteName = null;

            foreach (ExcelWorksheet wk in ws)
            {
                report.addReportTab(wk.Name);
                report.changeCurrentTab(wk.Name);
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
            }
            return Tuple.Create(siteName, report);
        }
    }
}
