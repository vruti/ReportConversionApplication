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
        public Dictionary<int, Record> readFiles(string[] files)
        {
            //Creating a dictionary to store all the records found in the files
            //being parsed
            Dictionary<int, Record> fileData = new Dictionary<int,Record>();

            //Looping through all the files given
            foreach (string file in files)
            {
                readFile(file, fileData);
            }
            return fileData;
        }

        //NOTE: Figure out pass by reference
        public Dictionary<int, Record> readFile(string file, Dictionary<int, Record> dict)
        {
            FileInfo newFile = new FileInfo(file);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;

            int n = 1;
            ArrayList vals;
            foreach (ExcelWorksheet wk in ws){
                dict = readWorksheet(file, wk, dict);
                vals = getCheckedValues(file, n);
                n++;
            }

            return dict;
        }

        private Dictionary<int, Record> readWorksheet(string file, ExcelWorksheet worksheet, Dictionary<int, Record> dict) {
            int totalRows = worksheet.Dimension.End.Row;
            int totalCols = worksheet.Dimension.End.Column;

            for (int i = 1; i < totalRows; i++)
            {
                for (int j = 1; j < totalCols; j++)
                {
                    if (worksheet.Cells[i, j].Value != null)
                    {
                        // DO THISSSSSSSSS
                        Console.Write("Hi");
                    }
                }
            }

                return dict;
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
    }
}
