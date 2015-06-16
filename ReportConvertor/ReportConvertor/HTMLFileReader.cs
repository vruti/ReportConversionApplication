using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    class HTMLFileReader
    {
        public Dictionary<string, List<Report>> readFiles(string[] files)
        {
            Dictionary<string, List<Report>> reportsBySite = new Dictionary<string, List<Report>>();
            Tuple<string, Report> tuple;

            foreach (string file in files)
            {
                tuple = readFile(file);
                if (reportsBySite.ContainsKey(tuple.Item1))
                {
                    reportsBySite[tuple.Item1].Add(tuple.Item2);
                }
                else
                {
                    List<Report> reportsList = new List<Report>();
                    reportsList.Add(tuple.Item2);
                    reportsBySite.Add(tuple.Item1, reportsList);
                }
            }
            return reportsBySite;
        }

        public Tuple<string, Report> readFile(string file)
        {
            Report report = new Report();
            string siteName = "";

            //PDF READER IMPLEMENTATION
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(file);
            DataTable table = new DataTable();
            string r;
            string x;

            ArrayList wholeFile = new ArrayList();

            ArrayList eachRow = new ArrayList();
            foreach (HtmlNode row in doc.DocumentNode.SelectNodes("//table//tr"))
            {
                foreach (HtmlNode col in row.SelectNodes("//td"))
                {
                    r = col.InnerText;
                    x = r.Replace("&nbsp;", "");
                    if (siteName == null)
                    {
                        if (isNameOfSite(x).Item1)
                        {
                            siteName = isNameOfSite(x).Item2;
                        }
                    }
                    eachRow.Add(x);
                }
                wholeFile.Add(eachRow);
                eachRow = new ArrayList();
            }
            report.addReportTab("main");
            report.changeCurrentTab("main");
            report.addRecords(wholeFile);

            return Tuple.Create(siteName, report);
        }

        public Tuple<bool, string> isNameOfSite(string n)
        {
            string name = n.ToLower();
            if (name.Contains("howard"))
            {
                return Tuple.Create(true, "Howard");
            }
            else if (name.Contains("highland 1"))
            {
                return Tuple.Create(true, "Highland 1");
            }
            else if (name.Contains("highland north"))
            {
                return Tuple.Create(true, "Highland North");
            }
            else if (name.Contains("patton"))
            {
                return Tuple.Create(true, "Patton");
            }
            else if (name.Contains("twin ridges"))
            {
                return Tuple.Create(true, "Twin Ridges");
            }
            else if (name.Contains("big sky"))
            {
                return Tuple.Create(true, "Big Sky"); ;
            }
            else if (name.Contains("mustang hills"))
            {
                return Tuple.Create(true, "Mustang Hills"); ;
            }
            return Tuple.Create(false, "");
        }
    }
}
