namespace Common.Data.Models
{
    public class Leaves : BaseModel
    {
        public int EmployeeId { get; set; }
        public DateTime? LeaveDate { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
    }
}
