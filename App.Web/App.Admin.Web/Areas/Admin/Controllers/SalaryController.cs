using Common.Core.Services;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SalaryController : BaseController
    {
        private readonly ISalaryService _salaryService;
        private readonly LogisticContext _dbcontext;


        public SalaryController(ISalaryService salaryService, LogisticContext dbcontext)
        {
            _dbcontext = dbcontext;
            _salaryService = salaryService;

        }

        public async Task<IActionResult> List()
        {
            var salaries = await _salaryService.GetAllSalaries();
           
            return View(salaries);
        }

        [HttpGet]
        public IActionResult AddOrUpdate(int? Id)
        {
            ViewBag.EmployeeList = _dbcontext.Employees.Select(e => new EmployeeModel { Id = e.Id, Name = e.Name })
            .ToList();
            SalaryModel model = Id.HasValue ? _salaryService.GetSalaryByEmployeeId(Id.Value).Result : new SalaryModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdate(SalaryModel salary)
        {
            if (salary.Id > 0)
            {
                var updated = await _salaryService.UpdateSalary(salary.Id, salary);
                Toast(updated ? "Salary Updated Successfully!" : "Error updating salary!", updated ? ToastType.SUCCESS : ToastType.ERROR);
            }
            else
            {
                await _salaryService.AddSalary(salary);
                Toast("Salary Created Successfully!", ToastType.SUCCESS);
            }
            return RedirectToAction("List", "Salary");
        }
    }
}
