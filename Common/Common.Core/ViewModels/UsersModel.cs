using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.ViewModels
{
    public class UsersModel: BaseModel
    {
        public string? Username { get; set; }
        public string? UserId { get; set; }
        public string? Mobile { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}
