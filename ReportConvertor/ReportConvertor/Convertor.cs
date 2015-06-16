using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public interface Convertor
    {
        public Dictionary<string, WorkOrder> convertReports(Dictionary<string, Record>);

        public int getDownTime();
    }
}
