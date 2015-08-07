using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class WorkOrder
    {
        private string mPulseID;
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
        private string taskDescription;
        private string assetID;
        private string assetDescription;
        private Vendor vendor;
        private double actualHours;
        private Dictionary<Part, int> partsList;
        private string originalID;
        private string comments;
        private WOTable woTable;
        private string filepath;

        /* Initialize a work order with the report ID*/
        public WorkOrder(string id, WOTable woT, string f)
        {
            originalID = id;
            woTable = woT;
            filepath = f;
            partsList = new Dictionary<Part, int>();
            actualHours = 0;
            downTime = 0;
            openDate = new DateTime();
            startDate = new DateTime();
            dateDone = new DateTime();
        }

        public string ID
        {
            get
            {
                return mPulseID;
            }
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

        public string TaskDescription
        {
            get
            {
                return taskDescription;
            }
            set
            {
                taskDescription = value;
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

        public string AssetDescription
        {
            get
            {
                return assetDescription;
            }
            set
            {
                assetDescription = value;
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

        public string Filepath
        {
            get
            {
                return filepath;
            }
        }

        public void addPart(Part p, int qty)
        {
            if (partsList.ContainsKey(p))
            {
                partsList[p]+=qty;
            }
            else
            {
                partsList.Add(p, qty);
            }
        }

        public Dictionary<Part, int> getPartsList()
        {
            return partsList;
        }

        /* Creates an MPulse ID for the work order
         * with all the necessary information */
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
            name += startDate.Year.ToString("D4");
            //add month
            name += startDate.Month.ToString("D2");
            //add day
            name += startDate.Day.ToString("D2");
            //get serial number
            int srNo = woTable.getWOIDNumber(name);
            //add serial number
            name = name + "-" + srNo.ToString("D2");

            mPulseID = name;
        }

        /* If there are certain values missing in a WO,
         * fill them in with default values */
        public void fillValues()
        {
            if (comments == null || comments.Equals("") || comments.Equals(" "))
            {
                comments = description;
            }
            if (outageType == null || outageType.Equals(" ") || outageType.Equals(" "))
            {
                outageType = "Planned";
            }
            if (woType == null || woType.Equals(" ") || woType.Equals(" "))
            {
                woType = "General Maintenance";
            }
            if (planning == null || planning.Equals(" ") || planning.Equals(" "))
            {
                planning = "Planned";
            }
            if (taskID == null || taskID.Equals(" ") || taskID.Equals(" "))
            {
                taskID = "MT-052";
                taskDescription = "General Maintenance";
            }
            if (priority == null || priority.Equals(" ") || priority.Equals(" "))
            {
                priority = "03-Medium";
            }
        }

        /* Puts all the work order information into a list
         * of ArrayLists so that it can be writte into 
         * the output file*/
        public List<ArrayList> getWORecord()
        {
            List<ArrayList> records = new List<ArrayList>();
            List<Part> keys = partsList.Keys.ToList();

            ArrayList record;
            if (comments == null || comments.Equals("") || comments.Equals(" "))
            {
                comments = description;
            }

            /* If there are no parts*/
            if (keys.Count == 0 || (keys.Count == 1 && partsList[keys[0]] < 1))
            {
                record = addValues();
                record.Add(" ");
                record.Add(" ");
                record.Add(" ");
                record.Add(comments);
                record.Add(originalID);

                records.Add(record);
                return records;
            }

            /* Each part is its own ArrayList*/
            foreach(Part key in keys)
            {
                if (partsList[key] > 0)
                {
                    record = addValues();
                    record.Add(key.ID);
                    record.Add(key.Description);
                    record.Add(partsList[key]);
                    record.Add(comments);
                    record.Add(originalID);

                    records.Add(record);
                }
            }
            return records;
        }

        /* Adding all the necessary values to the ArrayList*/
        private ArrayList addValues()
        {
            ArrayList record = new ArrayList();
            record.Add(mPulseID);
            record.Add(description);
            record.Add(status);
            record.Add(woType);
            record.Add(outageType);
            record.Add(priority);
            record.Add(openDate.ToShortDateString());
            record.Add(startDate.ToShortDateString());
            record.Add(dateDone.ToShortDateString());
            record.Add(downTime);
            record.Add(site.Name);
            record.Add(planning);
            record.Add(unplannedType);
            record.Add(taskID);
            record.Add(taskDescription);
            record.Add(assetID);
            record.Add(assetDescription);
            record.Add(vendor.ID);
            record.Add(vendor.Name);
            record.Add(actualHours);

            return record;
        }
    }
}
