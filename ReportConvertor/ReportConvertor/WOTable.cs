using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class WOTable
    {
        private Dictionary<string, int> pastWOs;
        private Dictionary<string, int> newWOs;
        private Dictionary<string, int> fieldToCell;

        public WOTable(string path)
        {
            newWOs = new Dictionary<string, int>();
            startTable(path);
        }

        public void startTable(string path)
        {
            pastWOs = new Dictionary<string, int>();
            generateTable(path);
        }

        /* Generating a dictionary with all the past
         * work order IDs as the key, linking to the
         * serial number */
        private void generateTable(string path)
        {
            FileInfo newFile = new FileInfo(path);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;
            ExcelWorksheet wk = ws[1];

            /* Working under the assumption that all work
             * orders will have consequtive serial numbers 
             */
            int totalRows = wk.Dimension.End.Row;

            /*starting from 2 because the first row will be
             * the header */
            for (int i = 2; i <= totalRows; i++)
            {
                string woID = wk.Cells[i, 1].Value.ToString();
                int end = lastInstance(woID);
                string id = woID.Substring(0, end - 1);
                int len = woID.Length - end;
                string num = woID.Substring((end), len);
                num = Regex.Replace(num, "[^0-9.]", "");
                int n = Convert.ToInt32(num);
                if (pastWOs.ContainsKey(id))
                {
                    pastWOs[id] = n;
                }
                else
                {
                    pastWOs.Add(id, 1);
                }
            }
        }

        private int lastInstance(string id)
        {
            int loc = 0;
            int i = 0;
            string temp = id.ToLower();
            int len = temp.Length;
            while (temp.IndexOf("-") > 0)
            {
                i = temp.IndexOf("-") + 1;
                loc += i;
                len = len-i;
                temp = temp.Substring(i, len);
            }

            return loc;
        }

                
         /* Check if a work order with the ID 
          * number already exists and generate
          * the last two numbers for the WO ID
          * to be created */
        public int getWOIDNumber(string id)
        {
            int result;
            /* If a work order with same date, location and
             * vendor was recently created, the most recent 
             * serial number will be in the newWorkOrders
             * dictionary, so we check that first*/
            if (newWOs.ContainsKey(id))
            {
                newWOs[id]++;
                result = newWOs[id];
            }
            else
            {
                /* if it isn't in the newWorkOrder dictionary, we check
                 * the archive of work order IDs*/
                if (pastWOs.ContainsKey(id))
                {
                    pastWOs[id]++;
                    result = pastWOs[id];
                }
                else
                {
                    /* if the id isn't present in either 
                     * dictionary,it is a brand new id 
                     * and should get serial number 1*/
                    newWOs.Add(id, 1);
                    result = 1;
                }
            }
            return result;
        }
    }
}
