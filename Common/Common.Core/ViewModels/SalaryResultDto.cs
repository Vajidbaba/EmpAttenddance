using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.ViewModels
{
    public class SalaryResultDto
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        public int WorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int PaidLeaveDays { get; set; }
        public int HalfDays { get; set; }
        public int UnpaidLeaves { get; set; }
        public int AbsentDays { get; set; }

        public decimal EarnedSalary { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal Deduction { get; set; }
        public decimal NetSalary { get; set; }
    }

}
