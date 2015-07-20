using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class Site
    {
        private string name;
        private string code3;
        private string code5;
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
                return code3;
            }
            set
            {
                code3 = value;
            }
        }

        public string FiveLetterCode
        {
            get
            {
                return code5;
            }
            set
            {
                code5 = value;
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
            string noSpaces = name.Replace(" ", "");
            string lowerCase = n.ToLower();
            if (n.Contains(name) || n.Contains(noSpaces))
            {
                return true;
            }
            foreach (string s in altNames)
            {
                if (n.Contains(s) || lowerCase.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
