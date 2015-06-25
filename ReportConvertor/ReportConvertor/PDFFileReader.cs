using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections;


namespace ReportConverter
{
    public class PDFFileReader : FileReader
    {
        private AppInfo info;
        public PDFFileReader(AppInfo i)
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
            string siteName="";

            //PDF READER IMPLEMENTATION
            var doc1 = new Document();
            PdfReader reader = new PdfReader(file);

            ITextExtractionStrategy s = new SimpleTextExtractionStrategy();

            //only one page
            var currentText = PdfTextExtractor.GetTextFromPage(reader, 1, s);
            currentText = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
            report.addReportTab("main");
            report.changeCurrentTab("main");

            int lengthOfFile = currentText.Length;
            List<List<string>> wholeFile = new List<List<string>>();

            siteName = getNameOfSite(currentText);
            List<string> eachLine = new List<string>();
            for (int i = 0; i < lengthOfFile; i++)
            {
                //check if it is the end of a line
                if (currentText[i] == '\n')
                {
                    wholeFile.Add(eachLine);
                    eachLine = new List<string>();
                }
                else
                {
                    string temp = currentText[i].ToString();
                    eachLine.Add(temp);
                }
            }
            report.addRecords(wholeFile);

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
