using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public class Site
    {
        private string siteName;
        private string siteCode3;
        private string siteCode5;
        private ArrayList altNames;
        private Vendor vendor;

        public Site()
        {
            altNames = new ArrayList();
        }

        public Vendor Contractor
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

        public string Name
        {
            get
            {
                return siteName;
            }
            set
            {
                siteName = value;
            }
        }

        public string ThreeLetterCode
        {
            get
            {
                return siteCode3;
            }
            set
            {
                siteCode3 = value;
            }
        }

        public string FiveLetterCode
        {
            get
            {
                return siteCode5;
            }
            set
            {
                siteCode5 = value;
            }
        }

        public void addAltName(string n)
        {
            altNames.Add(n);
        }

        public ArrayList getAltNames()
        {
            return altNames;
        }

        public bool isSite(string n)
        {
            if (n.Contains(siteName))
            {
                return true;
            }
            foreach (string s in altNames)
            {
                if (n.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
