using Common.Core.Services;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MasterDepartmentController : Controller
    {
        private readonly IMasterDepartmentService _masterDepartmentService;
        private readonly ILeaveService _leaveService;
        private readonly IContextHelper _contextHelper;
        private readonly LogisticContext _dbcontext;
        public MasterDepartmentController(IMasterDepartmentService masterDepartmentService, ILeaveService leaveService, IContextHelper contextHelper, LogisticContext dbcontext)
        {
            _dbcontext = dbcontext;
            _leaveService = leaveService;
            _contextHelper = contextHelper;
            _masterDepartmentService = masterDepartmentService;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var leaves = await _masterDepartmentService.GetActiveDepartmentsAsync();
            return View(leaves);
        }
        [HttpGet]
        public async Task<IActionResult> SaveLeaveMaster(int id)
        {
            var model = await _masterDepartmentService.GetMasterById(id);
            return PartialView("_AddOrEdit", model);
        }
        [HttpPost]
        public async Task<IActionResult> SaveLeaveMaster(DepartmentMaster model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var userId = _contextHelper.GetUsername();
                var result = await _masterDepartmentService.SaveMaster(model, userId);
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}
