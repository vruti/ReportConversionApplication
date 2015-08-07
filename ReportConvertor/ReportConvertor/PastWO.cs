using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportConverter
{
    public class PastWO
    {
        private string asset;
        private DateTime startDate;
        private DateTime endDate;
        private DateTime openDate;
        private string id;

        public PastWO(string i)
        {
            id = i;
        }

        public string ID
        {
            get
            {
                return id;
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
                return endDate;
            }
            set
            {
                endDate = value;
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

        public string Asset
        {
            get
            {
                return asset;
            }
            set
            {
                asset = value;
            }
        }

    }
}
