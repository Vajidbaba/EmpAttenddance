using Common.Core.Services;
using Common.Data.Context;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class SalaryController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IContextHelper _contextHelper;
        private readonly ISalariesService _salaryService;
        private readonly LogisticContext _dbcontext;
        public SalaryController(ISalariesService salaryService, IEmployeeService employeeService, LogisticContext dbcontext, IContextHelper contextHelper)
        {
            _employeeService = employeeService;
            _dbcontext = dbcontext;
            _contextHelper = contextHelper;
            _salaryService = salaryService;
        }
        public async Task<IActionResult> Index(int year, int month)
        {
            if (year == 0) year = DateTime.Now.Year;
            if (month == 0) month = DateTime.Now.Month;

            var list = await _salaryService.GetSalaryListAsync(year, month);
            return View("index", list);
        }
        public async Task<IActionResult> SalaryDetails(int employeeId, int year, int month)
        {
            var salaryResult = await _salaryService.CalculateEmployeeSalaryAsync(employeeId, year, month);
            return View("salarydetails", salaryResult);
        }
    }
}
