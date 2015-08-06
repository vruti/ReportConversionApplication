using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class Report
    {
        private Dictionary<string, List<List<string>>> allRecords;
        private List<string> checklistVals;
        private string currentTab;
        private string filepath;

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
            if (allRecords.Count == 0)
            {
                tabName = "Main";
            }
            List<List<string>> records = new List<List<string>>();
            allRecords.Add(tabName, records);
            return true;
        }

        public void changeCurrentTab(string tabName)
        {
            if (allRecords.Count == 1)
            {
                tabName = "Main";
            }
            currentTab = tabName;
        }

        public void addRecords(List<List<string>> records)
        {
            foreach (List<string> row in records)
            {
                allRecords[currentTab].Add(row);
            }
        }

        public List<List<string>> getRecords(string tab)
        {
            return allRecords[tab];
        }

        public void addCheckedVals(List<string> v)
        {
            if (checklistVals != null)
            {
                foreach (string c in v)
                {
                    checklistVals.Add(c);
                }
            }
            else
            {
                checklistVals = v;
            }
        }

        public List<string> getKeys()
        {
            return allRecords.Keys.ToList();
        }

        public List<string> checkedVals()
        {
            if (checklistVals.Contains(""))
            {
                checklistVals.Remove("");
            }
            return checklistVals;
        }

        public string Filepath
        {
            set
            {
                filepath = value;
            }
            get
            {
                return filepath;
            }
        }
    }
}
