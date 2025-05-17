using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Models
{
    public class OvertimeMaster : BaseModel
    {
        public string Type { get; set; }
        public decimal RatePerHour { get; set; }
        public string? Description { get; set; }
    }

}
