using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class Part
    {
        private string id;
        private string supplierID;
        private string description;
        private int qty;
        private Vendor vendor;
        private string stockArea;

        public Part(string partID, Vendor v)
        {
            supplierID=partID;
            vendor = v;
            stockArea = v.Name;
        }

        public void generateID(string newestID)
        {
            int len = newestID.Length;
            int start = len - 4;
            string num = newestID.Substring(start, 4);
            int n = Convert.ToInt32(num);
            n++;
            id = vendor.FiveLetterCode+"-PRT-" + n.ToString("D4");
        }

        public Vendor Vendor
        {
            get
            {
                return vendor;
            }
        }

        public string ID
        {
            set
            {
                id = value;
            }
            get
            {
                return id;
            }
        }

        public string SupplierID
        {
            get
            {
                return supplierID;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        public int Qty
        {
            get
            {
                return qty;
            }
            set
            {
                qty = value;
            }
        }

        public ArrayList getRecord()
        {
            ArrayList parts = new ArrayList();

            parts.Add(id);
            parts.Add(description);
            parts.Add(qty);
            parts.Add(Vendor.Name);
            parts.Add(stockArea);
            parts.Add(supplierID);

            return parts;
        }
    }
}
