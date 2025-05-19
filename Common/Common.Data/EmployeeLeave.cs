using Common.Data.Models;

namespace Common.Data
{
    public class EmployeeLeave: BaseModel
    {
        public int EmployeeId { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime?EndDate { get; set; }
        public int NumberOfDays { get; set; }
        public string Status { get; set; }
        public string? Reason { get; set; }
    }
}
