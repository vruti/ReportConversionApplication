using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public interface Converter
    {
        void convertReport(Report report);

        List<Part> getNewParts();

        List<WorkOrder> getWorkOrders();
    }
}
