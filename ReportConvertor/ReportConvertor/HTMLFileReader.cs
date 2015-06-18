using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text.RegularExpressions;
using System.IO;

namespace ReportConvertor
{
    public class HTMLFileReader : FileReader
    {
        private AppInfo info;

        public HTMLFileReader(AppInfo i)
        {
            info = i;
        }

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
            string siteName = null;

            //PDF READER IMPLEMENTATION
            HtmlDocument doc = new HtmlDocument();
            string data = File.ReadAllText(file);
            doc.LoadHtml(data);
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
                        siteName = getNameOfSite(x);
                      
                    }
                    eachRow.Add(x);
                }
                wholeFile.Add(eachRow);
                eachRow = new ArrayList();
            }
            report.addReportTab("main");
            report.changeCurrentTab("main");
            report.addRecords(wholeFile);
            Console.WriteLine(siteName);

            return Tuple.Create(siteName, report);
        }

        public string getNameOfSite(string n)
        {
            string name = n.ToLower();
            List<Site> sites = info.getSites();

            foreach (Site s in sites)
            {
                if (s.isSite(name))
                {
                    return s.Name;
                }
            }

            return null;
        }
    }
}
