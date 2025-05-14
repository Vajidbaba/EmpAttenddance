using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.ViewModels
{
    public class DataTableModel
    {
        public string draw { get; set; }
        public string start { get; set; }
        public int TotalCount { get; set; }
        public string sortColumn { get; set; }
        public string length { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public int pageSize { get; set; }
        public string sortColumnDirection { get; set; }
        public string searchValue { get; set; }
        public int skip { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string WorkHours { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
    }
}
