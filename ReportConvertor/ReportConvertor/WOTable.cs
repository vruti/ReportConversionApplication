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
        private Dictionary<string, int> pastWOIDs;
        private Dictionary<string, List<string>> pastWOAsset;
        private Dictionary<string, int> newWOs;
        private Dictionary<string, int> fieldToCell;

        public WOTable(string path)
        {
            newWOs = new Dictionary<string, int>();
            startTable(path);
        }

        public void startTable(string path)
        {
            pastWOIDs = new Dictionary<string, int>();
            pastWOAsset = new Dictionary<string, List<string>>();
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

            int assetI = getAssetI(wk);

            /*starting from 2 because the first row will be
             * the header */
            for (int i = 2; i <= totalRows; i++)
            {
                string woID = wk.Cells[i, 1].Value.ToString();

                //Find the last instance of the '-' symbol
                int end = lastInstance(woID);
                //Get the first portion of the ID
                string id = woID.Substring(0, end - 1);
                int len = woID.Length - end;
                //Get the serial number in the ID;
                string num = woID.Substring((end), len);
                num = Regex.Replace(num, "[^0-9.]", "");
                int n = Convert.ToInt32(num);
                string asset;
                //Get Asset ID
                try
                {
                    asset = wk.Cells[i, assetI].Value.ToString();
                }
                catch
                {
                    asset = "";
                }

                if (pastWOIDs.ContainsKey(id))
                {
                    pastWOIDs[id] = n;
                    pastWOAsset[id].Add(asset);
                }
                else
                {
                    pastWOIDs.Add(id, n);
                    List<string> assets = new List<string>();
                    assets.Add(asset);
                    pastWOAsset.Add(id, assets);
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

        public Boolean isValidWO(WorkOrder wo)
        {
            string woID = wo.ID;
            int end = woID.Length - 3;
            string id = woID.Substring(0, end);
            if (pastWOAsset.ContainsKey(id))
            {
                string asset = wo.AssetID;
                if (pastWOAsset[id].Contains(asset))
                {
                    return false;
                }
            }
            return true;
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
                if (pastWOIDs.ContainsKey(id))
                {
                    pastWOIDs[id]++;
                    result = pastWOIDs[id];
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

        private int getAssetI(ExcelWorksheet wk)
        {
            string s = "Asset";
            int cols = wk.Dimension.End.Column;

            for (int i = 1; i <= cols; i++)
            {
                string val = wk.Cells[1, i].Value.ToString();
                if (val.Contains(s))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
