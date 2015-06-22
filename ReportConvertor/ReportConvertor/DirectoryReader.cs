using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    class DirectoryReader
    {
        private string inputDir;

        public DirectoryReader(string dir){
            inputDir= dir;
        }

        /* with a given directory, the function reads
         * all the excel, pdf and html files in it and 
         * stores them in a dictionary, organized by the
         * type of file */
        public Dictionary<string, string[]> readDirectory()
        {
            Dictionary <string, string[]> dict = new Dictionary<string, string[]>();

            //excel files. All the report files are .xlsx
            string[] filePathsXlsx = Directory.GetFiles(@inputDir, "*.xlsx");
            dict.Add("xlsx", filePathsXlsx);

            //pdf files
            string[] filePathsPDF = Directory.GetFiles(@inputDir, "*.pdf");
            dict.Add("pdf", filePathsPDF);

            //html files
            string[] filePathsHTML = Directory.GetFiles(@inputDir, "*.htm");
            dict.Add("html", filePathsHTML);

            return dict;
        }
    }
}
