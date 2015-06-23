﻿using System;
using System.Collections;
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
        private DateTime openDate;
        private DateTime startDate;
        private DateTime dateDone;
        private double downTime;
        private Site site;
        private string planning;
        private string unplannedType = " ";
        private string taskID;
        private string assetID;
        private Vendor vendor;
        private double actualHours;
        private Dictionary<string, int> partsList;
        private string originalID;
        private string comments;


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
                return openDate;
            }
            set
            {
                openDate = value;
            }
        }

        public string Comments
        {
            get
            {
                return comments;
            }
            set
            {
                comments = value;
            }
        }

        public double DownTime
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

        public void addPart(string id, int qty)
        {
            if (partsList.ContainsKey(id))
            {
                partsList[id]+=qty;
            }
            else
            {
                partsList.Add(id, qty);
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
            name += startDate.Year.ToString();
            //add month
            name += startDate.Month.ToString();
            //add day
            name += startDate.Day.ToString();
            //get serial number
            int srNo = vendor.getWOIDNumber(name);
            //add WOID to vendor's WO list
            vendor.addWO(name, srNo);
            //add serial number
            name = name + "-" + srNo.ToString();

            mPulseID = name;
        }

        public List<ArrayList> getWORecord()
        {
            List<ArrayList> records = new List<ArrayList>();
            List<string> keys = partsList.Keys.ToList();

            ArrayList record;
            foreach(string key in keys)
            {
                record = new ArrayList();
                record.Add(mPulseID);
                record.Add(description);
                record.Add(status);
                record.Add(woType);
                record.Add(outageType);
                record.Add(priority);
                record.Add(openDate);
                record.Add(startDate);
                record.Add(dateDone);
                record.Add(downTime);
                record.Add(site.Name);
                record.Add(planning);
                record.Add(unplannedType);
                record.Add(taskID);
                record.Add(assetID);
                record.Add(vendor.ID);
                record.Add(actualHours);
                record.Add(key);
                record.Add(partsList[key]);
                record.Add(comments);
                record.Add(originalID);

                records.Add(record);
            }
            return records;
        }
    }
}
