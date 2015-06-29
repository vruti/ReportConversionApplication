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

        public Part(string partID, Vendor v)
        {
            supplierID=partID;
            vendor = v;
        }

        public void generateID(string newestID)
        {
            string num = newestID.Substring(10, 4);
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
            parts.Add(supplierID);

            return parts;
        }
    }
}
