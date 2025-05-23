using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Models
{
    public class Holidays
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; }  // e.g., "Weekend", "Public Holiday", "Festival"
        public bool IsWeekend { get; set; }
        public bool IsCompanyHoliday { get; set; }
    }

}
