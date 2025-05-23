using Common.Core.Services;
using Common.Core.ViewModels;
using Common.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MarkAttendanceController : BaseController
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
            var leaveTypes = await _dbcontext.LeaveMasters.ToListAsync();

            int GetLimit(int leaveId) => leaveTypes.FirstOrDefault(l => l.Id == leaveId)?.YearlyLimit ?? 0;

            // Get yearly limits
            int sickLimit = GetLimit(1);
            int casualLimit = GetLimit(2);
            int paidLimit = GetLimit(3);
            int unpaidLimit = GetLimit(4);

            // Calculate leave usages
            int sickUsed = await _dbcontext.Leaves
                .Where(x => x.EmployeeId == id && x.LeaveId == 1)
                .SumAsync(x => (int)x.TotalDays);
            int casualUsed = await _dbcontext.Leaves
                .Where(x => x.EmployeeId == id && x.LeaveId == 2)
                .SumAsync(x => (int)x.TotalDays);
            int paidUsed = await _dbcontext.Leaves
                .Where(x => x.EmployeeId == id && x.LeaveId == 3)
                .SumAsync(x => (int)x.TotalDays);
            int unpaidUsed = await _dbcontext.Leaves
                .Where(x => x.EmployeeId == id && x.LeaveId == 4)
                .SumAsync(x => (int)x.TotalDays);

            var model = new AttendanceFormViewModel
            {
                SickLeavesUsed = sickUsed,
                SickLeavesBalance = sickLimit - sickUsed,

                CasualLeavesUsed = casualUsed,
                CasualLeavesBalance = casualLimit - casualUsed,

                PaidLeavesUsed = paidUsed,
                PaidLeavesBalance = paidLimit - paidUsed,

                UnpaidLeavesUsed = unpaidUsed,
                UnpaidLeavesBalance = unpaidLimit - unpaidUsed,

                EmployeeId = id,
                EmployeeName = employee?.Name ?? "N/A",
                // attendance
                AttendanceDate = attendance?.AttendanceDate ?? DateTime.Today,
                AttendanceStatus = attendance?.AttendanceStatus,
                // overtime
                TotalOvertimeHours = overtime?.TotalOvertimeHours,
                AdvancePay = overtime?.AdvancePay,
                Bonus = overtime?.Bonus,
                Deducation = overtime?.Deducation,
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
