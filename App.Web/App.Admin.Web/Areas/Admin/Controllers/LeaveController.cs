using Common.Core.Services;
using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LeaveMasterController : BaseController
    {
        private readonly ILeaveService _leaveService;
        private readonly IContextHelper _contextHelper;
        private readonly LogisticContext _dbcontext;

        public LeaveMasterController(ILeaveService leaveService, IContextHelper contextHelper, LogisticContext dbcontext)
        {
            _dbcontext = dbcontext;
            _leaveService = leaveService;
            _contextHelper = contextHelper;

        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var leaves = await _leaveService.AllLeaveMasterAsync();
            return View(leaves);
        }
        [HttpGet]
        public async Task<IActionResult> SaveLeaveMaster(int id)
        {
            var model = await _leaveService.GetMasterById(id);
            return PartialView("_AddOrEdit", model);
        }
        [HttpPost]
        public async Task<IActionResult> SaveLeaveMaster(LeaveMaster model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var userId = _contextHelper.GetUsername();
                var result = await _leaveService.SaveMaster(model, userId);
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
