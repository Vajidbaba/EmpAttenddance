using System.ComponentModel.DataAnnotations;

namespace Common.Data.Models
{
    public class MarkAttendance: BaseModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AttendanceDate { get; set; }
        [Required]
        public string AttendanceStatus { get; set; }
    }
}
