namespace Common.Data.Models
{
    public class LeaveMaster : BaseModel
    {
        public int? DepartmentId { get; set; }
        public int? SickLeaves { get; set; }
        public int? CasualLeaves { get; set; }
        public int? PaidLeaves { get; set; }
        public int? UnpaidLeaves { get; set; }
        public int? Year { get; set; }
    }
}
