using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public class WorkOrder
    {
        public string mPulseID;
        private string description;
        private string status;
        private string woType;
        private string outageType;
        private string priority;
        private DateTime reportDate;
        private DateTime startDate;
        private DateTime dateDone;
        private int downTime;
        private Site site;
        private string planning;
        private string unplannedType;
        private string taskID;
        private string assetID;
        private Vendor vendor;
        private double actualHours;
        private Dictionary<string, int> partsList;
        private string originalID;


        public WorkOrder(string id)
        {
            originalID = id;
            partsList = new Dictionary<string, int>();
        }

        public string Description {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }

        }

        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        public string WorkOrderType
        {
            get
            {
                return woType;
            }
            set
            {
                woType = value;
            }
        }

        public string OutageType
        {
            get
            {
                return outageType;
            }
            set
            {
                outageType = value;
            }
        }

        public string Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return dateDone;
            }
            set
            {
                dateDone = value;
            }
        }

        public DateTime OpenDate
        {
            get
            {
                return reportDate;
            }
            set
            {
                reportDate = value;
            }
        }

        public int DownTime
        {
            get
            {
                return downTime;
            }
            set
            {
                downTime = value;
            }
        }

        public Site Site
        {
            get
            {
                return site;
            }
            set
            {
                site = value;
            }
        }

        public string Planning
        {
            get
            {
                return planning;
            }
            set
            {
                planning = value;
            }
        }

        public string UnplannedType
        {
            get
            {
                return unplannedType;
            }
            set
            {
                unplannedType = value;
            }
        }

        public string TaskID
        {
            get
            {
                return taskID;
            }
            set
            {
                taskID = value;
            }
        }

        public string AssetID
        {
            get
            {
                return assetID;
            }
            set
            {
                assetID = value;
            }
        }

        public Vendor Vendor
        {
            get
            {
                return vendor;
            }
            set
            {
                vendor = value;
            }
        }

        public double ActualHours
        {
            get
            {
                return actualHours;
            }
            set
            {
                actualHours = value;
            }
        }

        public string OriginalID
        {
            get
            {
                return originalID;
            }
        }

        public void addPart(string id)
        {
            if (partsList.ContainsKey(id))
            {
                partsList[id]++;
            }
            else
            {
                partsList.Add(id, 1);
            }
        }

        public Dictionary<string, int> getPartsList()
        {
            return partsList;
        }

        public void createMPulseID()
        {
            string name = "";
            
            //add site name
            name += site.FiveLetterCode;
            //add -WKO
            name+="-WKO";
            //add vendor name
            name = name+"-"+vendor.ThreeLetterCode+ "-";
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
        }
    }
}
