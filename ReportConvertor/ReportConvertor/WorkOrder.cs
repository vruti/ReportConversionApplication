using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    class WorkOrder
    {
        private string mPulseID;
        private string description;
        private string status;
        private string type;
        private string outageType;
        private string priority;
        private DateTime reportDate;
        private DateTime startDate;
        private DateTime dateDone;
        private int downTime;
        private string regulatoryDocument;
        private string regulator;
        private Site site;
        private string planning;
        private string unplannedType;
        private string taskID;
        private string assetID;
        private Vendor vendor;
        private string ActualHours;
        private string partID;
        private string partQuantity;
        private string originalID;


        public WorkOrder(string id)
        {
            originalID = id;
        }

        public string createMPulseID()
        {
            string name = "";
            
            //add site name
            name += site.getSiteCode();
            //add -WKO
            name+="-WKO";
            //add vendor name
            name = name+"-"+vendor.get3Form() + "-";
            //add year
            //CHECK IF DATE DONE
            name += reportDate.Year.ToString();
            //add month
            name += reportDate.Month.ToString();
            //add day
            name += reportDate.Day.ToString();
            //get serial number
            int srNo = vendor.getWOIDNumber(name);
            //add WOID to vendor's WO list
            vendor.addWO(name, srNo);
            //add serial number
            name = name + "-" + srNo.ToString();

            mPulseID = name;
            return mPulseID;
        }

        
    }
}
