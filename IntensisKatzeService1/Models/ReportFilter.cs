using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntensisKatzeService1.Models
{
    public class ReportFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Operation { get; set; }
        public int Hours { get; set; }

        public ReportFilter(DateTime startDate, DateTime endDate, string operation, int hours)
        {
            StartDate = startDate;
            EndDate = endDate;
            Operation = operation;
            Hours = hours;
        }
    }
}
