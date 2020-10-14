using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntensisKatzeService1.Models
{
    public class EmployeeReport
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public DateTime SelectedDate { get; set; }
        public string DailyWork { get; set; }
        public int Minutes { get; set; }
        public string ErrorMessage { get; set; }

        public EmployeeReport(int employeeId, string name, DateTime selectedDate, 
            string dailyWork, int minutes, string errorMessage)
        {
            EmployeeId = employeeId;
            Name = name;
            SelectedDate = selectedDate;
            DailyWork = dailyWork;
            Minutes = minutes;
            ErrorMessage = errorMessage;

        }
    }
}
