using Common.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.ViewModels
{
    public class AttendanceDataResult
    {
        public List<AttendanceModel> Data { get; set; }
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
    }
}
