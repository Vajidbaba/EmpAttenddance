using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Models
{
    public class LeaveMasters: BaseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? YearlyLimit { get; set; }
    }

}
