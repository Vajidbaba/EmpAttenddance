using System.ComponentModel.DataAnnotations;

namespace Common.Data.Models
{
    public class EmployeeModel : BaseModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        public string? Contact { get; set; }
        public string? Department { get; set; }
        public string? Role { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string SalaryType { get; set; } // Hourly, Daily, Monthly
    }
}
