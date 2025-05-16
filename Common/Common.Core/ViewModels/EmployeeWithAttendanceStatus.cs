using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.ViewModels
{
    public class EmployeeWithAttendanceStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string AttendanceStatus { get; set; }
    }
}
