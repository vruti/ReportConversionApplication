using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public class PartsTable
    {
        private Dictionary<string, string> partsTable;

        /*
         * creating a parts table from an excel file based on 
         * the name of the contractor.
         * NOTE: no new part names are added to the excel file 
         * if found, they will be added directly to MPulse
         * and the excel file will be updated via datalink.
         */
        public PartsTable(Vendor v, string file)
        {
            /* initializing the dictionary to store the 
             * contractor part number to mpulse part number 
             */
            this.partsTable = new Dictionary<string, string>();
            FileInfo newFile = new FileInfo(file);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;

            /*chosing the tab number in the file based
             * on the contractor name
             */
            ExcelWorksheet wk = ws[v.getTabNo()];

            int totalRows = wk.Dimension.End.Row;

            for (int i = 2; i < totalRows; i++)
            {
                //1st column has contractor part name
                //2nd column has mpulse part name
                if (wk.Cells[i, 1].Value != null && wk.Cells[i,2].Value != null)
                {
                    partsTable.Add(wk.Cells[i, 1].Text, wk.Cells[i, 2].Text);
                } 
            }
        }

        public string getPartName(string cPart)
        {
            //return the mpulse part number if it exists
            if (partsTable.ContainsKey(cPart))
            {
                return partsTable[cPart];
            }
            //return null if it doesn't
            //convertor should create a new part to be uploaded
            return null;
        }
    }
}
