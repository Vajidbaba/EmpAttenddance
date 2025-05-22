namespace Common.Data.Models
{
    public class Leaves : BaseModel
    {
        public int EmployeeId { get; set; }
        public int? LeaveId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TotalDays { get; set; }
        public string? Reason { get; set; }
        public DateTime? ApplyDate { get; set; }
        public string? Status { get; set; }
    }
}
