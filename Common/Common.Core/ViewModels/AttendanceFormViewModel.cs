using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.ViewModels
{
    public class AttendanceFormViewModel
    {
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string? AttendanceStatus { get; set; }
        public decimal? TotalOvertimeHours { get; set; }
        public decimal? AdvancePay { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Deducation { get; set; }


    }
}
