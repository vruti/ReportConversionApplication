using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    class DirectoryReader
    {
        private string inputDir;
        private string archiveDir;

        public DirectoryReader(string dir, string aDir){
            inputDir = dir;
            archiveDir = aDir;
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
            List<string> filePaths = filePathsXlsx.ToList();
            if (filePathsXlsx.Length > 0)
            {
                dict.Add("xlsx", filePathsXlsx);
            }
            
            List<string> filePathsXls = Directory.GetFiles(@inputDir, "*.xls").ToList();
            List<string> finalFilePathXls = new List<string>();
            for (int i = 0; i < filePathsXls.Count; i++)
            {
                string file = filePathsXls[i];
                if (!file.Contains("xlsx") && !file.Contains("xlsm"))
                {
                    finalFilePathXls.Add(file);
                }
            }
            if (finalFilePathXls.Count > 0)
            {
                dict.Add("xls", finalFilePathXls.ToArray());
            }

            //adding the evp format reports
            string[] filePathsXlsm = Directory.GetFiles(@inputDir, "*.xlsm");
            if (filePathsXlsm.Length > 0)
            {
                dict.Add("xlsm", filePathsXlsm);
            }

            //pdf files
            string[] filePathsPDF = Directory.GetFiles(@inputDir, "*.pdf");
            if (filePathsPDF.Length > 0)
            {
                dict.Add("pdf", filePathsPDF);
            }

            //html files
            string[] filePathsHTML = Directory.GetFiles(@inputDir, "*.htm");
            if (filePathsHTML.Length > 0)
            {
                dict.Add("html", filePathsHTML);
            }            

            return dict;
        }

        public void moveFiles(string[] files)
        {
            foreach (string filePath in files)
            {
                string file = Path.GetFileName(filePath);
                string archiveFile = Path.Combine(archiveDir, file);
                File.Move(filePath, archiveFile);
            }
        }
    }
}
