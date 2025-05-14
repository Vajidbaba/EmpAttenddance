using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Please enter Username")]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        public string? Password { get; set; }

    }
}
