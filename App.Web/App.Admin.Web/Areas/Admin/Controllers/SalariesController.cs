using Common.Core.Services;
using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SalariesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IContextHelper _contextHelper;
        private readonly ISalariesService _salaryService;

        private readonly LogisticContext _dbcontext;
        
        public SalariesController(ISalariesService salaryService, IEmployeeService employeeService, LogisticContext dbcontext, IContextHelper contextHelper)
        {
            _employeeService = employeeService;
            _dbcontext = dbcontext;
            _contextHelper = contextHelper;
            _salaryService = salaryService;
        }

        public async Task<IActionResult> List()
        {
            var model = new EmployeeSalaryViewModel
            {
                Employees = await _employeeService.GetAllEmployees(),
                SalaryList = await _salaryService.GetAllSalaries()
            };
            return View(model);
        }
        public IActionResult AddOrUpdate()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetSalary(int id)
        {
            var salary = await _salaryService.GetSalaryByEmployeeId(id);
            return PartialView("_add", salary);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateSalary(Salaries salary)
        {
            bool success = await _salaryService.UpdateSalary(salary);
            return Json(new { success });
        }
    }
}
