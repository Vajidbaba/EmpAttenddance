using System.ComponentModel.DataAnnotations;

namespace Common.Data.Models
{
    public class EmployeeModel : BaseModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        public string? FatherName { get; set; }
        public string? Email { get; set; }
        public string? Contact { get; set; }
        public string? Department { get; set; }
        public string? Role { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public DateTime? DateOfResign { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pin { get; set; }
        public string? Address { get; set; }
    }
}
