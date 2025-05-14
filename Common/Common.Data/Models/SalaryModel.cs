using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Models
{
    public class SalaryModel:BaseModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [Required]
        [RegularExpression("Hourly|Daily|Monthly", ErrorMessage = "Invalid Salary Type")]
        public string SalaryType { get; set; }

        public decimal BaseSalary { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime PaymentDate { get; set; }
    }

}
