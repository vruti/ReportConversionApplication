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
            //Nothing to initialize
        }

        /**
         * This function checks if a work order is valid.
         * Since most of the information parsed from the 
         * reports are validated in the converter, the 
         * function only checks if the asset and task IDs
         * are valid
         **/
        public bool isValid(WorkOrder wo)
        {
            if (wo.AssetID == null || wo.AssetID.Equals("")) return false;
            if (wo.TaskID == null || wo.TaskID.Equals("")) return false;
            return true;
        }
    }
}
