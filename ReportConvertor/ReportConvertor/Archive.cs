using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class Archive
    {
        private string outputFile;
        private string archiveDir;

        public Archive(string oFile, string aDir)
        {
            outputFile = oFile;
            archiveDir = aDir;
        }

        public void startArchive()
        {
            List<List<string>> data = new List<List<string>>();
            FileInfo inNewFile = new FileInfo(outputFile);
            ExcelPackage inPck = new ExcelPackage(inNewFile);
            ExcelWorksheets inWks = inPck.Workbook.Worksheets;

            DateTime today = DateTime.Today;
            string date = "";
            date += today.Month.ToString()+"-";
            date += today.Day.ToString()+"-";
            date += today.Year.ToString();
            int end = 1;
            string archiveFile = archiveDir + "\\\\Archived-" + date + "-" + end.ToString("D2") + ".xlsx";
            while (File.Exists(archiveFile))
            {
                end++;
                archiveFile = archiveDir + "\\\\Archived-" + date + "-" + end.ToString("D2") + ".xlsx";
            }
            FileInfo aNewFile = new FileInfo(archiveFile);
            
            ExcelPackage aPck = new ExcelPackage(aNewFile);
            ExcelWorksheet aWk = null;

            foreach (ExcelWorksheet inWk in inWks)
            {
                string name = inWk.Name;
                aWk = aPck.Workbook.Worksheets.Add(name);
                readFile(inWk, aWk);
            }
            aPck.Save();
            inPck.Save();
        }

        private void readFile(ExcelWorksheet inWk, ExcelWorksheet aWk)
        {
            int totalRows = inWk.Dimension.End.Row;
            int totalCols = inWk.Dimension.End.Column;

            for (int i = 1; i <= totalRows; i++)
            {
                for (int j = 1; j <= totalCols; j++)
                {
                    aWk.Cells[i, j].Value = inWk.Cells[i, j].Value;
                    if (i > 1)
                    {
                        inWk.Cells[i, j].Value = null;
                    }
                }
            }
        }
    }
}
