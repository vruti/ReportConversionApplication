﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportConvertor
{
    public interface Convertor
    {
        Dictionary<string, WorkOrder> convertReports(Dictionary<string, Report> dict);

        int getDownTime();
    }
}
