﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class VestasConverter : Converter
    {
        private Dictionary<string, Part> newParts;
        private WorkOrder newWO;
        private Dictionary<string, WorkOrder> flaggedWO;
        private string site;
        AppInfo info;
        List<List<string>> records;

        public VestasConverter(string s, AppInfo i)
        {
            info = i;
            site = s;
        }

        public void convertReport(Report report)
        {
            records = report.getRecords("main");
        }

        private string getID()
        {
            int len = records.Count;
            string id = null;
            for (int i = 0; i < len; i++)
            {
                List<string> row = records[i];
                if (row.Contains("Service") && row.Contains("Order"))
                {
                    row = records[i + 1];

                }
            }
            return id;
        }

        private void rearrangeHeaders(List<string> headers)
        {

            int i;
            for (i = 0; i < records.Count; i++)
            {
                List<string> row = records[i];
                if (isHeader(row[0], headers))
                {
                    List<string> newR = new List<string>();          
                    while (row.Count != 0)
                    {
                        string word = "";
                        foreach (string v in row)
                        {
                            if (v.Contains(":"))
                            {
                                word += v;
                                newR.Add(word);
                                word = "";
                            }
                            else
                            {
                                word += v + " ";
                            }
                        }
                    }
                    records[i] = newR;
                }
            }
        }

        private bool isHeader(string firstVal, List<string> indicators)
        {
            foreach (string val in indicators)
            {
                if (val.ToLower().Contains(firstVal))
                {
                    return true;
                }
            }
            return false;
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
