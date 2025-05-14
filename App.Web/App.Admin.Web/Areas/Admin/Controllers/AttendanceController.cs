using Common.Core.Services;
using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Common.Core.Services.IEmployeeService;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class AttendanceController : BaseController
    {
        private readonly IAttendanceService _attendanceService;
        private readonly LogisticContext _dbcontext;


        public AttendanceController(IAttendanceService attendanceService, LogisticContext dbcontext)
        {
            _attendanceService = attendanceService;
            _dbcontext = dbcontext;

        }

        public IActionResult AddOrUpdate(int? id)
        {
            AttendanceModel model = id.HasValue? _attendanceService.GetAttendanceById(id.Value).Result: new AttendanceModel();

            var today = DateTime.Today;

            ViewBag.EmployeeList = id.HasValue ? _dbcontext.Employees.Select(e => new EmployeeModel { Id = e.Id, Name = e.Name }).ToList()
                : _dbcontext.Employees.Where(e => !_dbcontext.Attendance.Any(a => a.EmployeeId == e.Id && a.Date == today))
                    .Select(e => new EmployeeModel { Id = e.Id, Name = e.Name }).ToList();

            return PartialView("_Add", model);
        }


        [HttpPost]
        public async Task<IActionResult> GetAttendanceData()
        {
            try
            {
                var model = new DataTableModel();
                model.draw = Request.Form["draw"].FirstOrDefault();
                model.start = Request.Form["start"].FirstOrDefault();
                model.length = Request.Form["length"].FirstOrDefault();
                model.searchValue = Request.Form["search[value]"].FirstOrDefault();
                model.sortColumn = "0";
                model.sortColumnDirection = "desc";
                model.StartDate = Request.Form["StartDate"].FirstOrDefault();
                model.EndDate = Request.Form["EndDate"].FirstOrDefault();
                model.pageSize = model.length != null ? Convert.ToInt32(model.length) : 0;
                model.skip = model.start != null ? Convert.ToInt32(model.start) : 0;
                model.Status = Request.Form["Status"].FirstOrDefault();
                var result = _attendanceService.GetDataTable(model);
                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<IActionResult> List()
        {
            var records = await _attendanceService.GetAllAttendanceRecords();
            return View(records);
        }

        public async Task<IActionResult> Details()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddOrUpdate(AttendanceModel attendance)
        {
            if (attendance.Id > 0)
            {
                var updated = await _attendanceService.UpdateAttendance(attendance.Id, attendance);
                Toast(updated ? "Attendance Updated Successfully!" : "Error updating attendance!", updated ? ToastType.SUCCESS : ToastType.ERROR);
            }
            else
            {
                await _attendanceService.AddAttendance(attendance);
                Toast("Attendance Created Successfully!", ToastType.SUCCESS);
            }
            return RedirectToAction("List", "Attendance");
        }
    }
}
