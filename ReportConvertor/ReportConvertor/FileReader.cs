using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public interface FileReader
    {
        Dictionary<string, List<Report>> readFiles(string[] files);

        Tuple<string, Report> readFile(string file);

        Tuple<bool, string> isNameOfSite(string n);
    }
}
