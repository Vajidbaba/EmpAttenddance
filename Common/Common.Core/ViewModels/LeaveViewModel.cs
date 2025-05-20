using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.ViewModels
{
    public class LeaveViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? LeaveDate { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
    }

    public class LeaveMasterViewModel
    {
        public int? Id { get; set; }
        public string? DepartmentName { get; set; }
        public int? SickLeaves { get; set; }
        public int? CasualLeaves { get; set; }
        public int? PaidLeaves { get; set; }
        public int? UnpaidLeaves { get; set; }
        public int? Year { get; set; }
    }

}
