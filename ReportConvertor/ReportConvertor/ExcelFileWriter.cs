using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class ExcelFileWriter
    {
        private AppInfo info;
       
        public ExcelFileWriter(AppInfo i)
        {
            info = i;
        }

        /* Starts the output write process */
        public void writeFiles(List<WorkOrder> wo, List<Part> p, List<WorkOrder> flaggedWO, List<List<string>> uAssets)
        {
            string outputFile = info.getFileLoc("Output");
            FileInfo newFile = new FileInfo(outputFile);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;
       
            //Each type (WO, parts, flagged WO, unlinked assets) gets its own tab
            writeWO(wo, ws);
            //If there are parts to be written
            if (p != null)
            {
                writeParts(p, ws);
            }
            writeFlagged(flaggedWO, ws);
            writeUnlinkedAssets(uAssets, ws);
            List<string> filesToMove = getFilesToMove(wo);
            moveFiles(filesToMove);
            pck.Save();
        }

        private void addHeaders(ExcelWorksheet wk, string name)
        {
            List<string> headers = info.getHeaders(name);

            for (int i = 0; i < headers.Count; i++)
            {
                wk.Cells[1, (i + 1)].Value = headers[i];
            }
        }

        /* Get the record version of all the work orders */
        private List<ArrayList> getRecords(List<WorkOrder> woList)
        {
            List<ArrayList> records= new List<ArrayList>();
            int rows = woList.Count;

            foreach (WorkOrder wo in woList)
            {
                List<ArrayList> rList = wo.getWORecord();
                foreach (ArrayList r in rList)
                {
                    records.Add(r);
                }
            }
            return records;
        }

        /* Write the work orders into the output file */
        private void writeWO(List<WorkOrder> woList, ExcelWorksheets ws)
        {
            ExcelWorksheet wk = ws["Work Orders"];
            if (wk == null)
            {
                wk = ws.Add("Work Orders");
                addHeaders(wk, "Work Orders");
            }
            List<ArrayList> records = getRecords(woList);
            int totalRows = records.Count;
            if (totalRows > 0)
            {
                int totalCols = records[0].Count;

                for (int i = 0; i < totalRows; i++)
                {
                    for (int j = 0; j < totalCols; j++)
                    {
                        wk.Cells[(i + 2), (j + 1)].Value = records[i][j];
                    }
                }
            }
        }

        /* Write the parts into the output file*/
        private void writeParts(List<Part> parts, ExcelWorksheets ws)
        {
            ExcelWorksheet wk = ws["Parts"];
            if (wk == null)
            {
                wk = ws.Add("Parts");
                addHeaders(wk, "Parts");
            }
            int totalRows = parts.Count;

            for (int i = 0; i < totalRows; i++)
            {
                ArrayList record = parts[i].getRecord();
                int totalCols = record.Count;
                for (int j = 0; j < totalCols; j++)
                {
                    wk.Cells[(i + 2), (j + 1)].Value = record[j];
                }
            }
        }

        /* Write the flagged WO into the output file*/
        private void writeFlagged(List<WorkOrder> flagged, ExcelWorksheets ws)
        {
            ExcelWorksheet wk = ws["Flagged Work Orders"];
            if (wk == null)
            {
                wk = ws.Add("Flagged Work Orders");
                addHeaders(wk, "Work Orders");
            }
            List<ArrayList> records = getRecords(flagged);
            int totalRows = records.Count;

            if (totalRows > 0)
            {
                int totalCols = records[0].Count;

                for (int i = 0; i < totalRows; i++)
                {
                    for (int j = 0; j < totalCols; j++)
                    {
                        wk.Cells[(i + 2), (j + 1)].Value = records[i][j];
                    }
                }
            }
        }

        /* Write the unlinked assets into the output file*/
        private void writeUnlinkedAssets(List<List<string>> assets, ExcelWorksheets ws)
        {
            ExcelWorksheet wk = ws["Unlinked Assets"];
            if (wk == null)
            {
                wk = ws.Add("Unlinked Assets");
                addHeaders(wk, "Unlinked Assets");
            }
            int totalRows = assets.Count;

            for (int i = 0; i < totalRows; i++)
            {
                wk.Cells[(i + 2), 1].Value = assets[i][0];
                wk.Cells[(i + 2), 2].Value = assets[i][1];
            }
        }

        /* Write the file locations into the appInfo file if any
         * were changed by the users in the settings menu*/
        public void writeFileLocs()
        {
            Dictionary<string, string> fileLocs = info.getFileLocs();
            string appInfoFile = fileLocs["AppInfo"];
            FileInfo newFile = new FileInfo(appInfoFile);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;
            ExcelWorksheet wk = ws["FileLocations"];
            fileLocs.Remove("AppInfo");
            List<string> keys = fileLocs.Keys.ToList();

            for (int i = 0; i < keys.Count; i++)
            {
                wk.Cells[(i + 2), 1].Value = keys[i];
                wk.Cells[(i + 2), 2].Value = fileLocs[keys[i]];
            }
            pck.Save();
        }

        private List<string> getFilesToMove(List<WorkOrder> wos)
        {
            List<string> files = new List<string>();

            foreach (WorkOrder wo in wos)
            {
                string path = wo.Filepath;
                if (!files.Contains(path))
                {
                    files.Add(path);
                }
            }

            return files;
        }

        /* Moving read files into archive folder*/
        public void moveFiles(List<string> files)
        {
            //Getting the Archive folder, if it doesn't exist create one
            string archiveDir = info.getFileLoc("Archive");
            if (!Directory.Exists(archiveDir))
            {
                Directory.CreateDirectory(archiveDir);
            }
            //Creates a folder based on the date
            DateTime today = DateTime.Today;
            string date = "";
            date += today.Month.ToString() + "-";
            date += today.Day.ToString() + "-";
            date += today.Year.ToString();
            string dir = archiveDir + "\\\\Archived-" + date;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            //Moves all the files to the archive folder
            foreach (string filePath in files)
            {
                string file = Path.GetFileName(filePath);
                string archiveFile = Path.Combine(dir, file);
                File.Move(filePath, archiveFile);
            }
        }
    }
}
