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
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _salaryService.GetAllSalariesWithEmployeeNameAsync();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> CalculateSalary(int id)
        {
            var year = DateTime.Now.Year;   
            var month = DateTime.Now.Month;
            var model = await _salaryService.GetSalaryDetailsAsync(id, year, month);
            return PartialView("_AddOrGenerate", model);
        }
        [HttpGet]
        public async Task<IActionResult> AddOrGenerate(int id)
        {
            var model = await _salaryService.GetAllSalariesWithEmployeeNameAsync();
            return View("_AddOrGenerate", model);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrGenerate(SalaryViewModel model)
        {
            var data = await _salaryService.GetAllSalariesWithEmployeeNameAsync();
            return View("_AddOrGenerate", data);
        }

        [HttpPost]
        public async Task<IActionResult> PostAddOrUpdate(SalaryViewModel model)
        {
            try
            {
                var result = await _salaryService.AddOrUpdateSalaryAsync(model);
                return Json(new { success = true, message = "✅ Salary saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult GetAddOrUpdate(int id)
        {
            if (id == 0)
            {
                return PartialView("_Add", new SalaryViewModel());
            }

            var salary = _dbcontext.Salaries.FirstOrDefault(x => x.Id == id);

            if (salary == null)
            {
                return NotFound();
            }

            var viewModel = new SalaryViewModel
            {
                Id = salary.Id,
                EmployeeId = salary.EmployeeId,
                Month = salary.Month,
                Year = salary.Year,
                BaseSalary = salary.BaseSalary,
                OvertimePay = salary.OvertimePay,
                Bonus = salary.Bonus,
                Advance = salary.Advance,
                Deduction = salary.Deduction,
                NetSalary = salary.NetSalary,
                PaymentDate = salary.PaymentDate,
                IsPaid = salary.IsPaid
            };

            return PartialView("_Add", viewModel);
        }
    }
}
