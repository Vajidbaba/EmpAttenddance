using System.ComponentModel.DataAnnotations;

namespace Common.Data.Models
{
    public class Users : BaseModel
    {

        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "UserId required")]
        public string? UserId { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string? Role { get; set; }
    }
}
