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
        ArrayList getCheckedValues(string file, int n);

        Dictionary<int, Record> readFiles(string[] files);

        Dictionary<int, Record> readFile(string file, Dictionary<int, Record> dict);
    }
}
