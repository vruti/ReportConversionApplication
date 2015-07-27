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
            report.addReportTab("main");
            report.changeCurrentTab("main");

            //PDF READER IMPLEMENTATION
            var doc1 = new Document();
            PdfReader reader = new PdfReader(file);

            int nPages = reader.NumberOfPages;
            string currentText = null;
            ITextExtractionStrategy s = new SimpleTextExtractionStrategy();
            List<List<string>> wholeFile = new List<List<string>>();

            for (int i = 1; i <= nPages; i++)
            {
                currentText = PdfTextExtractor.GetTextFromPage(reader, i, s);
                currentText = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));               
            }

            int lengthOfFile = currentText.Length;
            if (siteName.Equals(""))
            {
                siteName = getNameOfSite(currentText);
            }
            List<string> eachLine = new List<string>();
            string word = "";
            for (int j = 0; j < lengthOfFile; j++)
            {
                //check if it is the end of a line
                if (currentText[j] == '\n')
                {
                    if (!word.Equals(""))
                    {
                        eachLine.Add(word);
                        word = "";
                    }
                    wholeFile.Add(eachLine);
                    eachLine = new List<string>();
                }
                else if (currentText[j] == ' ')
                {
                    if (!word.Equals(""))
                    {
                        eachLine.Add(word);
                        word = "";
                    }
                }
                else
                {
                    word += currentText[j];
                }
            }

            //only one page
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
