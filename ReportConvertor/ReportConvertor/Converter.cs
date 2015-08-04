using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    /* Interface for all the Converters*/
    public interface Converter
    {
        void convertReport(Report report);

        List<WorkOrder> getWorkOrders();

        List<WorkOrder> getFlaggedWO();
    }
}
