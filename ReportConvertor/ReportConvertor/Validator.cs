using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    public class Validator
    {
        public Validator()
        {

        }

        public bool isValid(WorkOrder wo)
        {
            if (wo.AssetID == null || wo.AssetID.Equals("")) return false;
            return true;
        }
    }
}
