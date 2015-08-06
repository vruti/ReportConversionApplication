using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class Vendor
    {
        private string id;
        private string name;
        private string v3Code;
        private string v5Code;
        private List<string> altNames;
        private int partsTab;
        private int woTab;
        private string woFile;
        private string partsFile;
        private Dictionary<string, Part> newParts;
        private Dictionary<string, Dictionary<string, List<string>>> fieldNames;

        public Vendor()
        {
            altNames = new List<string>();
            newParts = new Dictionary<string, Part>();
        }

        //Getters and Setter for all the attributes

        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
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
                return v3Code;
            }
            set
            {
                v3Code = value;
            }
        }

        public string FiveLetterCode
        {
            get
            {
                return v5Code;
            }
            set
            {
                v5Code = value;
            }
        }

        public int PartsTabNo
        {
            get
            {
                return partsTab;
            }
            set
            {
                partsTab = value;
            }
        }

        public int WOArchiveTabNo
        {
            get
            {
                return woTab;
            }
            set
            {
                woTab = value;
            }
        }

        //Adding alternate names for the contractor
        public void addAltNames(string n)
        {
            altNames.Add(n);
        }

        public List<string> getAltNames()
        {
            return altNames;
        }

        public string PartsFile
        {
            set
            {
                partsFile = value;
            }
        }

        public string WOArchiveFile
        {
            set
            {
                woFile = value;
            }
        }

        /* Field header names specific to each tab in the report*/
        public void addFieldNames(Dictionary<string, Dictionary<string, List<string>>> fields)
        {
            fieldNames = fields;
        }

        public Dictionary<string, List<string>> getFieldNames(string tab)
        {
            return fieldNames[tab];
        }
    }
}
