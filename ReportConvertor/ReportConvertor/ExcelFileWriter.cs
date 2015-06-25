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

        public void writeFiles(List<WorkOrder> wo, List<Part> p, List<WorkOrder> flaggedWO)
        {
            string outputFile = info.getFileLoc("Output");
            FileInfo newFile = new FileInfo(outputFile);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;
       
            writeWO(wo, ws);
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
            List<ArrayList> records = getRecords(woList);
            int totalRows = records.Count;
            int totalCols = records[0].Count;

            for (int i = 0; i < totalRows; i++)
            {
                for (int j = 0; j < totalCols; j++)
                {
                    wk.Cells[(i+2), (j+1)].Value = records[i][j];
                }
            }

        }

        private void writeParts(List<WorkOrder> woList, ExcelWorksheets ws)
        {
            ExcelWorksheet wk = ws["Work Orders"];
            List<ArrayList> records = getRecords(woList);
            int totalRows = records.Count;
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
}
