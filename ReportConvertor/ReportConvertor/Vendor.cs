﻿using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public class Vendor
    {
        private string id;
        private string name;
        private string v3Code;
        private string v5Code;
        private ArrayList altNames;
        private Dictionary<string, int> newWO;
        private Dictionary<string, int> oldWO;
        private Dictionary<string, string> partsTable;
        private int partsTab;
        private int woTab;
        string woFile;
        string partsFile;

        public Vendor()
        {
            newWO = new Dictionary<string, int>();
            altNames = new ArrayList();
        }

        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string ThreeLetterCode
        {
            get
            {
                return v3Code;
            }
            set
            {
                v3Code = value;
            }
        }

        public string FiveLetterCode
        {
            get
            {
                return v5Code;
            }
            set
            {
                v5Code = value;
            }
        }

        public int PartsTabNo
        {
            get
            {
                return partsTab;
            }
            set
            {
                partsTab = value;
            }
        }

        public int WOArchiveTabNo
        {
            get
            {
                return woTab;
            }
            set
            {
                woTab = value;
            }
        }

        public void addAltNames(string n)
        {
            altNames.Add(n);
        }

        public ArrayList getAltNames()
        {
            return altNames;
        }

        public string PartsFile
        {
            set
            {
                partsFile = value;
            }
        }

        public string WOArchiveFile
        {
            set
            {
                woFile = value;
            }
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
            if (newWO.ContainsKey(id))
            {
                newWO[id]++;
            }
            else
            {
                /* If the workorder isn't present,
                 * add it to the dictionary with its serial
                 * number to start */
                newWO.Add(id, n);
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
            if(newWO.ContainsKey(id)){
                result=newWO[id]+1;
            } else {
                /* if it isn't in the newWorkOrder dictionary, we check
                 * the archive of work order IDs*/
                if (oldWO.ContainsKey(id))
                {
                    result = oldWO[id]+1;
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

        private void generateWOHistory()
        {
            oldWO = new Dictionary<string,int>();
            FileInfo newFile = new FileInfo(woFile);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;
            ExcelWorksheet wk = ws[woTab];

            /*working under the assumption that all work
             * orders will have consequtive serial numbers 
             */
            int totalRows = wk.Dimension.End.Row;
            string woName;
            string woID;

            /*starting from 2 because the first row will be
             * the header */
            for (int i = 2; i <= totalRows; i++)
            {
                if(wk.Cells[i,1].Value != null){
                    woName=wk.Cells[i,1].Text;
                    woID = woName.Substring(0,(woName.Length - 2));
                    if (oldWO.ContainsKey(woID))
                    {
                        oldWO[woID]++;
                    }
                    else
                    {
                        oldWO.Add(woID, 1);
                    }

                }
            }
        }

        private void generatePartsTable()
        {
            /* initializing the dictionary to store the 
             * contractor part number to mpulse part number 
             */
            partsTable = new Dictionary<string, string>();
            FileInfo newFile = new FileInfo(partsFile);
            ExcelPackage pck = new ExcelPackage(newFile);
            ExcelWorksheets ws = pck.Workbook.Worksheets;

            /*chosing the tab number in the file based
             * on the contractor name
             */
            ExcelWorksheet wk = ws[partsTab];

            int totalRows = wk.Dimension.End.Row;

            for (int i = 2; i <= totalRows; i++)
            {
                //1st column has contractor part name
                //2nd column has mpulse part name
                if (wk.Cells[i, 1].Value != null && wk.Cells[i, 2].Value != null)
                {
                    partsTable.Add(wk.Cells[i, 1].Text, wk.Cells[i, 2].Text);
                }
            }
        }

        public string getPartID(string cPart)
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

        public string newestPartID()
        {
            List<string> keys = partsTable.Keys.ToList();
            int len = keys.Count;
            return partsTable[keys[len]];
        }
    }
}
