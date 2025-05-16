namespace Common.Data.Models
{
    public class LeaveRequests : BaseModel
    {

        public int EmployeeId { get; set; }

        public string LeaveType { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now.Date;

        public DateTime EndDate { get; set; } = DateTime.Now.Date;

        public string TotalDays { get; set; }

        public string Status { get; set; } 
        public string Reason { get; set; }

        public DateTime AppliedOn { get; set; } = DateTime.Now;
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
    }
}
