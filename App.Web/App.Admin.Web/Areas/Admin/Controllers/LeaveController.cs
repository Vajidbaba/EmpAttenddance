using Common.Core.Services;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LeaveController : BaseController
    {
        private readonly ILeaveService _leaveService;
        private readonly IContextHelper _contextHelper;
        private readonly LogisticContext _dbcontext;

        public LeaveController(ILeaveService leaveService, IContextHelper contextHelper, LogisticContext dbcontext)
        {
            _dbcontext = dbcontext;
            _leaveService = leaveService;
            _contextHelper = contextHelper;

        }

        public async Task<IActionResult> List()
        {
            ViewBag.EmployeeLists = _dbcontext.LeaveRequests.ToList();

            var leaves = await _leaveService.GetAllLeaveRequests();
            return View(leaves);
        }

        [HttpGet]
        public IActionResult Apply()
        {
            ViewBag.EmployeeList = _dbcontext.Employees.Select(e => new EmployeeModel { Id = e.Id, Name = e.Name })
    .ToList();
            return View(new LeaveRequests());
        }

        [HttpPost]
        public async Task<IActionResult> Apply(LeaveRequests leave)
        {
          
            await _leaveService.ApplyLeave(leave);
            Toast("Leave Request Submitted!", ToastType.SUCCESS);
            return RedirectToAction("List", "Leave");
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            await _leaveService.ApproveLeave(id, _contextHelper.GetUsername());
            Toast("Leave Approved!", ToastType.SUCCESS);
            return RedirectToAction("List", "Leave");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            await _leaveService.RejectLeave(id, _contextHelper.GetUsername());
            Toast("Leave Rejected!", ToastType.ERROR);
            return RedirectToAction("List", "Leave");
        }
    }
}
