using Common.Core.Services;
using Common.Core.ViewModels;
using Common.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MarkAttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeeService _employeeService;
        private readonly IContextHelper _contextHelper;
        private readonly ILeaveService _leaveService;
        private readonly IMasterDepartmentService _masterDepartmentService;
        private readonly LogisticContext _dbcontext;


        public MarkAttendanceController(LogisticContext dbcontext, IMasterDepartmentService masterDepartmentService, ILeaveService leaveService, IAttendanceService attendanceService, IEmployeeService employeeService, IContextHelper contextHelper)
        {
            _attendanceService = attendanceService;
            _employeeService = employeeService;
            _contextHelper = contextHelper;
            _dbcontext = dbcontext;
            _leaveService = leaveService;
            _masterDepartmentService = masterDepartmentService;


        }
        public async Task<IActionResult> List()
        {
            var model = await _attendanceService.GetEmployeesWithTodayAttendanceAsync();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            ViewBag.LeaveList = _leaveService.GetListMaster();
            var attendance = await _attendanceService.GetTodayAttendanceAsync(id);
            var employee = await _employeeService.GetEmployeeById(id);
            var overtime = await _attendanceService.GetTodayOvertime(id);

            int sickLeaves = await _dbcontext.Leaves.Where(x => x.EmployeeId == id && x.LeaveId == 1)
                .SumAsync(x => (int)x.TotalDays);

            int casualLeaves = await _dbcontext.Leaves.Where(x => x.EmployeeId == id && x.LeaveId == 2)
                .SumAsync(x => (int)x.TotalDays);

            int unpaidLeaves = await _dbcontext.Leaves.Where(x => x.EmployeeId == id && x.LeaveId == 3)
                .SumAsync(x => (int)x.TotalDays);

            int paidLeaves = await _dbcontext.Leaves.Where(x => x.EmployeeId == id && x.LeaveId == 4)
                .SumAsync(x => (int)x.TotalDays);


            var model = new AttendanceFormViewModel
            {
                EmployeeId = id,
                EmployeeName = employee?.Name ?? "N/A",
                //attendance
                AttendanceDate = attendance?.AttendanceDate ?? DateTime.Today,
                AttendanceStatus = attendance?.AttendanceStatus,
                //overtime
                TotalOvertimeHours = overtime?.TotalOvertimeHours,
                AdvancePay = overtime?.AdvancePay,
                Bonus = overtime?.Bonus,
                Deducation = overtime?.Deducation,
                // Add leave counts here
                SickLeaves = sickLeaves,
                CasualLeaves = casualLeaves,
                UnpaidLeaves = unpaidLeaves,
                PaidLeaves = paidLeaves
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
