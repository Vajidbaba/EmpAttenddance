using Common.Data.Models;

namespace Common.Core.Services
{
    public class Salaries : BaseModel
    {
        public int EmployeeId { get; set; }
        public string? SalaryType { get; set; }
        public DateTime? Date { get; set; }  
        public decimal? BaseSalary { get; set; }
        public decimal? OvertimePay { get; set; }
        public decimal? AdvancePay { get; set; }
        public decimal? Deductions { get; set; }
        public decimal? NetSalary { get; set; }
    }

}
