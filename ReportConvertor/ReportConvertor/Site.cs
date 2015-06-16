using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public class Site
    {
        private string siteName;
        private string siteCode;

        public Site(string name, string code)
        {
            siteName = name;
            siteCode = code;
        }

        public string getSiteName()
        {
            return siteName;
        }

        public string getSiteCode()
        {
            return siteCode;
        }
    }
}
