using Common.Core.Services;
using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OvertimeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IOvertimeRecordsService _overtimeRecordsService;

        public OvertimeController(IEmployeeService employeeService, IOvertimeRecordsService overtimeRecordsService)
        {
            _employeeService = employeeService;
            _overtimeRecordsService = overtimeRecordsService;
        }

        public async Task<IActionResult> List()
        {
            var model = new EmployeeOvertimeViewModel
            {
                Employees = await _employeeService.GetAllEmployees(),
                OvertimeList = await _overtimeRecordsService.GetAllOvertimes()
            };
            return View(model);
        }
        public IActionResult AddOrUpdate()
        {
            return View();
        }
    }
}
