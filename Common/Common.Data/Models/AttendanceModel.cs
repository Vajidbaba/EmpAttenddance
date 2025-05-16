using Common.Data.Models;

namespace Common.Data.Models
{
    public class AttendanceModel : BaseModel
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now.Date;
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public decimal WorkHours { get; set; }
        public string Status { get; set; }
    }
}
