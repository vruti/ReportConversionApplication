using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public class Vendor
    {
        private string vendorID;
        private string vendorName;
        private string vendor3LetterName;
        private string vendor5LetterName;
        private Dictionary<string, int> newWorkOrders;
        private int tabNo;
        string wOFile;

        public Vendor(string id, string name, string threeName, string fiveName, int n, string file)
        {
            vendorID = id;
            vendorName = name;
            vendor3LetterName = threeName;
            vendor5LetterName = fiveName;
            newWorkOrders = new Dictionary<string, int>();
            tabNo = n;
            wOFile = file;
        }

        public string getVendorName()
        {
            return vendorName;
        }

        public string get5Form()
        {
            return vendor5LetterName;
        }

        public string get3Form()
        {
            return vendor3LetterName;
        }

        public int getTabNo()
        {
            return tabNo;
        }

        /*
         * add newly created WO IDs to vendor
         * maintained list so that we can know if a
         * WO already exists. Newly created WOs are 
         * only added to external excel file after the
         * entire conversion process is completed
         */
        public void addWO(string id, int n)
        {
            if (newWorkOrders.ContainsKey(id))
            {
                newWorkOrders[id]++;
            }
            else
            {
                /* If the workorder isn't present,
                 * add it to the dictionary with its serial
                 * number to start */
                newWorkOrders.Add(id, n);
            }
        }

        /*
         * check if a work order with the ID 
         * number already exists and generate
         * the last two numbers for the WO ID
         * to be created
         */
        public int getWOIDNumber(string id)
        {
            int result;
            /* if a work order with same date, location and
             * vendor was recently created, the most recent 
             * serial number will be in the newWorkOrders
             * dictionary, so we check that first*/
            if(newWorkOrders.ContainsKey(id)){
                result=newWorkOrders[id]+1;
            } else {
                /* if it isn't in the newWorkOrder dictionary, we check
                 * the archive of work order IDs*/
                Dictionary<string, int> workOrders = getWOHistory();
                if (workOrders.ContainsKey(id))
                {
                    result = workOrders[id]+1;
                }
                else
                {
                    /* if the id isn't present in either 
                     * dictionary,it is a brand new id 
                     * and should get serial number 1*/
                    result = 1;
                }
            }
            return result;
        }

        private Dictionary<string, int> getWOHistory()
        {
            Dictionary<string, int> WorkOrders = new Dictionary<string,int>();
            FileInfo newFile = new FileInfo(wOFile);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;
            ExcelWorksheet wk = ws[tabNo];

            /*working under the assumption that all work
             * orders will have consequtive serial numbers 
             */
            int totalRows = wk.Dimension.End.Row;
            string woName;
            string woID;

            /*starting from 2 because the first row will be
             * the header */
            for (int i = 2; i < totalRows; i++)
            {
                if(wk.Cells[i,1].Value != null){
                    woName=wk.Cells[i,1].Text;
                    woID = woName.Substring(0,(woName.Length - 2));
                    if (WorkOrders.ContainsKey(woID))
                    {
                        WorkOrders[woID]++;
                    }
                    else
                    {
                        WorkOrders.Add(woID, 1);
                    }

                }
            }

            return WorkOrders;
        }
    }
}
