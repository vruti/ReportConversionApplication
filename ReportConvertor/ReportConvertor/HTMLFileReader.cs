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
using System.Windows.Forms;

namespace ReportConverter
{
    public class HTMLFileReader : FileReader
    {
        private AppInfo info;
        private ProgressBar pBar;

        public HTMLFileReader(AppInfo aInfo, ProgressBar pB)
        {
            info = aInfo;
            pBar = pB;
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
                pBar.PerformStep();
            }
            return reportsBySite;
        }

        public Tuple<string, Report> readFile(string file)
        {
            Report report = new Report();
            string siteName = null;

            //PDF READER IMPLEMENTATION
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            string data = File.ReadAllText(file);
            doc.LoadHtml(data);
            string x;

            List<List<string>> wholeFile = new List<List<string>>();

            List<String> eachRow;
            List<List<string>> table = doc.DocumentNode.SelectNodes("//table").Descendants("tr").Skip(1).Where(tr => tr.Elements("td").Count() > 1).Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList()).ToList();
            foreach (List<string> row in table)
            {
                eachRow = new List<string>();
                foreach (string val in row)
                {
                    x = val.Replace("&nbsp;", "");
                    x = x.Replace("\n", "");
                    x = x.Trim();
                    if (siteName == null)
                    {
                        siteName = getNameOfSite(x);
                      
                    }
                    eachRow.Add(x);
                }
                wholeFile.Add(eachRow);
            }
            report.addReportTab("main");
            report.changeCurrentTab("main");
            report.addRecords(wholeFile);

            report.Filepath = file;
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
