using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public interface Converter
    {
        Dictionary<string, WorkOrder> convertReport(Report report);

        int getDownTime();
    }
}
