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

        public Dictionary<string, string[]> readDirectory()
        {
            Dictionary <string, string[]> dict = new Dictionary<string, string[]>();

            //xlsx
            string[] filePathsXlsx = Directory.GetFiles(@inputDir, "*.xlsx");
            dict.Add("xlsx", filePathsXlsx);

            //pdf
            string[] filePathsPDF = Directory.GetFiles(@inputDir, "*.pdf");
            dict.Add("pdf", filePathsPDF);

            return dict;
        }
    }
}
