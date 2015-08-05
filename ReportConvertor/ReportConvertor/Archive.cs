using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    /* Class to put all the data in the output file in
     * an archive file in the archive folder specified
     * by the user */
    public class Archive
    {
        private string outputFile;
        private string archiveDir;

        public Archive(string oFile, string aDir)
        {
            //Location of the output file
            outputFile = oFile;
            //Location of archive directory and creates an output archive if not present
            archiveDir = aDir + "\\Output Archive";
            if (!Directory.Exists(archiveDir))
            {
                Directory.CreateDirectory(archiveDir);
            }
        }

        /* Starts the process of archiving*/
        public void startArchive()
        {
            //Open the output file
            List<List<string>> data = new List<List<string>>();
            FileInfo inNewFile = new FileInfo(outputFile);
            ExcelPackage inPck = new ExcelPackage(inNewFile);
            ExcelWorksheets inWks = inPck.Workbook.Worksheets;

            //Adding the date and a serial number to the archive file path
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

            //Open a new file to archive the output
            FileInfo aNewFile = new FileInfo(archiveFile);
            ExcelPackage aPck = new ExcelPackage(aNewFile);
            ExcelWorksheet aWk = null;

            int valid = 0;
            //Copying all the tabs in the output file
            foreach (ExcelWorksheet inWk in inWks)
            {
                string name = inWk.Name;
                aWk = aPck.Workbook.Worksheets.Add(name);
                valid += readFile(inWk, aWk);
            }
            //Saving the two files
            if (valid > -4)
            {
                /* Only create and save the archive file if 
                 * data was copied */
                aPck.Save();
            }
            inPck.Save();
        }

        /* Copies information from output file to archive file and 
         * clears the output file */
        private int readFile(ExcelWorksheet inWk, ExcelWorksheet aWk)
        {
            int totalRows = 0;
            int totalCols = 0;

            try
            {
                
                totalRows = inWk.Dimension.End.Row;
                totalCols = inWk.Dimension.End.Column;
            }
            catch
            {
                /* If the output file is empty, totalRows and 
                 * totalCols will remain zero and the rest of 
                 * the function won't run */
                return -1;
            }

            for (int i = 1; i <= totalRows; i++)
            {
                for (int j = 1; j <= totalCols; j++)
                {
                    //Copying all the values
                    aWk.Cells[i, j].Value = inWk.Cells[i, j].Value;
                    if (i > 1)
                    {
                        //Removing WO data
                        inWk.Cells[i, j].Value = null;
                    }
                }
            }
            return 1;
        }
    }
}
