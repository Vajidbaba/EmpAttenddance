using Common.Core.Services;
using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MarkAttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeeService _employeeService;
        private readonly IContextHelper _contextHelper;
        public MarkAttendanceController(IAttendanceService attendanceService, IEmployeeService employeeService, IContextHelper contextHelper)
        {
            _attendanceService = attendanceService;
            _employeeService = employeeService;
            _contextHelper = contextHelper;
        }
        public async Task<IActionResult> List()
        {
            var model = await _attendanceService.GetEmployeesWithTodayAttendanceAsync();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            var attendance = await _attendanceService.GetTodayAttendanceAsync(id);
            var employee = await _employeeService.GetEmployeeById(id);

            var model = new AttendanceFormViewModel
            {
                EmployeeId = id,
                EmployeeName = employee?.Name ?? "N/A",
                AttendanceDate = attendance?.AttendanceDate ?? DateTime.Today,
                AttendanceStatus = attendance?.AttendanceStatus,
            };

            return PartialView("_Add", model);
        }

        [HttpGet]
        public async Task<IActionResult> Summery()
        {
            var attendanceList = await _attendanceService.GetAttendanceSummery();
            var employees = await _employeeService.GetAllEmployees();

            var model = employees.Select(emp => new AttendanceFormViewModel
            {
                EmployeeId = emp.Id,
                EmployeeName = emp.Name ?? "N/A",
                AttendanceDate = attendanceList.FirstOrDefault(a => a.EmployeeId == emp.Id)?.AttendanceDate ?? DateTime.Today,
                AttendanceStatus = attendanceList.FirstOrDefault(a => a.EmployeeId == emp.Id)?.AttendanceStatus ?? "Absent"
            }).ToList();

            return PartialView("_Summery", model);
        }


        [HttpPost]
        public async Task<IActionResult> Add(AttendanceFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Add", model);
            var userId = _contextHelper.GetUsername();
            await _attendanceService.SaveOrUpdateAttendance(model, userId); 
            return RedirectToAction("List");
        }


    }
}
