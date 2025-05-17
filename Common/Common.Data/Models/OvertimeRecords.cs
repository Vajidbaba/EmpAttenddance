using Common.Data.Models;

namespace Common.Core.Services
{
    public class OvertimeRecords : BaseModel
    {
        public int EmployeeId { get; set; }
        public DateTime? Date { get; set; }
        public decimal? TotalOvertimeHours { get; set; }
        public decimal? RatePerHour { get; set; }
        public decimal? OvertimePay { get; set; }
        public decimal? AdvancePay { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Deducation { get; set; }

    }
}
