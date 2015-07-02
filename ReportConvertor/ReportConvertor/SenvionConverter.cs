using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class SenvionConverter : Converter
    {
        private Dictionary<string, Part> newParts;
        private Dictionary<string, WorkOrder> newWOs;
        private Dictionary<string, WorkOrder> flaggedWO;
        private string site;
        AppInfo info;

        public SenvionConverter(AppInfo i, string s)
        {
            info = i;
            site = s;
        }

        public void convertReport(Report report)
        {

        }

        public List<Part> getNewParts()
        {
            List<string> keys = newParts.Keys.ToList();
            List<Part> parts = new List<Part>();
            foreach (string key in keys)
            {
                parts.Add(newParts[key]);
            }
            return parts;
        }

        public Dictionary<string, WorkOrder> getWorkOrders()
        {
            return newWOs;
        }
    }
}
