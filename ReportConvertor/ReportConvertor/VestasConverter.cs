using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class VestasConverter : Converter
    {
        private Dictionary<string, Part> newParts;
        private List<WorkOrder> newWOs;
        private Dictionary<string, WorkOrder> flaggedWO;
        private string site;
        AppInfo info;

        public VestasConverter(string s, AppInfo i)
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

        public List<WorkOrder> getWorkOrders()
        {
            return newWOs;
        }
    }
}
