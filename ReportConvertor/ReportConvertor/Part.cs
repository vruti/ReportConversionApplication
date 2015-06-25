using System;
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

        public Part(string partID)
        {
            supplierID=partID;
        }

        public void generateID(string newestID)
        {
            string num = newestID.Substring(10, 4);
            int n = Convert.ToInt32(num);
            n++;
            id = "GAMSA-PRT-" + n.ToString("D4");
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
    }
}
