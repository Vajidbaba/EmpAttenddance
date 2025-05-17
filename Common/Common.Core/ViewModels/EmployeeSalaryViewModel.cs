using Common.Core.Services;
using Common.Data.Models;

namespace Common.Core.ViewModels
{
    public class EmployeeSalaryViewModel
    {
        public List<EmployeeModel> Employees { get; set; }
        public List<Salaries> SalaryList { get; set; }

    }
    public class EmployeeOvertimeViewModel
    {
        public List<EmployeeModel> Employees { get; set; }
        public List<OvertimeRecords> OvertimeList { get; set; }

    }
}
