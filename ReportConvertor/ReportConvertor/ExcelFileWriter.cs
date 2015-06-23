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
            ExcelWorksheets ws = openFile(outputFile);
            writeWO(wo, ws);
        }

        private ExcelWorksheets openFile(string file)
        {
            FileInfo newFile = new FileInfo(file);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;

            return ws;
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
            ExcelWorksheet wk = ws.Add("Work Orders");
            List<ArrayList> records = getRecords(woList);
            int totalRows = records.Count;
            int totalCols = records[1].Count;

            for (int i = 2; i <= totalRows+1; i++)
            {
                for (int j = 1; j <= totalCols; j++)
                {
                    wk.Cells[i, j].Value = records[i][j];
                }
            }

        }
    }
}
