using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public class Report
    {
        private Dictionary<string, ArrayList> allRecords;
        private ArrayList checklistVals;
        private string currentTab;

        public Report()
        {
            allRecords = new Dictionary<string, ArrayList>();
        }

        public bool addReportTab(string tabName)
        {
            if (allRecords.ContainsKey(tabName))
            {
                //already contains this tab
                return false;
            }
            ArrayList records = new ArrayList();
            allRecords.Add(tabName, records);
            return true;
        }

        public void changeCurrentTab(string tabName)
        {
            currentTab = tabName;
        }

        public void addRecords(ArrayList record)
        {
            allRecords[currentTab].Add(record);
        }

        public void addCheckedVals(ArrayList v)
        {
            checklistVals = v;
        }
    }
}
