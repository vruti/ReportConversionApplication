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
        private Dictionary<string, List<List<string>>> allRecords;
        private ArrayList checklistVals;
        private string currentTab;

        public Report()
        {
            allRecords = new Dictionary<string, List<List<string>>>();

        }

        public bool addReportTab(string tabName)
        {
            if (allRecords.ContainsKey(tabName))
            {
                //already contains this tab
                return false;
            }
            List<List<string>> records = new List<List<string>>();
            allRecords.Add(tabName, records);
            return true;
        }

        public void changeCurrentTab(string tabName)
        {
            currentTab = tabName;
        }

        public void addRecords(List<string> record)
        {
            allRecords[currentTab].Add(record);
        }

        public List<List<string>> getRecords(string tab)
        {
            return allRecords[tab];
        }

        public void addCheckedVals(ArrayList v)
        {
            checklistVals = v;
        }

        public List<string> getKeys()
        {
            return allRecords.Keys.ToList();
        }
    }
}
