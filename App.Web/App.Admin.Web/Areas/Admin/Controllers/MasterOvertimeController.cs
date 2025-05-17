using Common.Core.Services;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class MasterOvertimeController : Controller
    {
        private readonly IMasterOvertimeService _service;
        private readonly IContextHelper _contextHelper;

        public MasterOvertimeController(IMasterOvertimeService service, IContextHelper contextHelper)
        {
            _service = service;
            _contextHelper = contextHelper;
        }
        public async Task<IActionResult> List()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            var model = id == 0 ? new OvertimeMaster() : await _service.GetByIdAsync(id);
            return PartialView("_Add", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEdit(OvertimeMaster model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Add", model);

            var userId = _contextHelper.GetUsername();
            await _service.AddOrUpdateAsync(model, userId);
            return RedirectToAction("List");
        }
    }
}
