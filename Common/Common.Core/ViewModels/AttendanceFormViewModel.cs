namespace Common.Core.ViewModels
{
    public class AttendanceFormViewModel
    {
        public int EmployeeId { get; set; }
        public int LeaveId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string? AttendanceStatus { get; set; }
        public int? LeaveType { get; set; }
        public int? TotalDays { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Reason { get; set; }
        public DateTime? ApplyDate { get; set; }
        public string? Status { get; set; }
        public int SickLeaves { get; set; }
        public int CasualLeaves { get; set; }
        public int UnpaidLeaves { get; set; }
        public int PaidLeaves { get; set; }
        public decimal? TotalOvertimeHours { get; set; }
        public decimal? AdvancePay { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Deducation { get; set; }
    }
}
