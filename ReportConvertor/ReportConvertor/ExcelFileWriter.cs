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

        public void writeFiles(List<WorkOrder> wo, List<Part> p, List<WorkOrder> flaggedWO, List<List<string>> uAssets)
        {
            string outputFile = info.getFileLoc("Output");
            FileInfo newFile = new FileInfo(outputFile);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;
       
            writeWO(wo, ws);
            if (p != null)
            {
                writeParts(p, ws);
            }
            writeFlagged(flaggedWO, ws);
            writeUnlinkedAssets(uAssets, ws);
            pck.Save();
        }

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

        private void writeWO(List<WorkOrder> woList, ExcelWorksheets ws)
        {
            ExcelWorksheet wk = ws["Work Orders"];
            if (wk == null)
            {
                wk = ws.Add("Work Orders");
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

        private void writeParts(List<Part> parts, ExcelWorksheets ws)
        {
            ExcelWorksheet wk = ws["Parts"];
            if (wk == null)
            {
                wk = ws.Add("Parts");
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

        private void writeFlagged(List<WorkOrder> flagged, ExcelWorksheets ws)
        {
            ExcelWorksheet wk = ws["Flagged Work Orders"];
            if (wk == null)
            {
                wk = ws.Add("Flagged Work Orders");
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

        private void writeUnlinkedAssets(List<List<string>> assets, ExcelWorksheets ws)
        {
            ExcelWorksheet wk = ws["Unlinked Assets"];
            if (wk == null)
            {
                wk = ws.Add("Unlinked Assets");
            }
            int totalRows = assets.Count;

            for (int i = 0; i < totalRows; i++)
            {
                wk.Cells[(i + 2), 1].Value = assets[i][0];
                wk.Cells[(i + 2), 2].Value = assets[i][1];
            }
        }

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
    }
}
