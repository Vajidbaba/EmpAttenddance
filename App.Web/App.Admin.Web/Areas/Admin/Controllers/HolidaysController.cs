using Common.Core.Services;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HolidaysController : Controller
    {
        private readonly IHolidayService _holidayService;

        public HolidaysController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        // List 
        public async Task<IActionResult> List()
        {
            var holidays = await _holidayService.GetAllAsync();
            return View(holidays);
        }
        [HttpGet]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
                return PartialView("_AddOrEdit", new Holidays { Date = DateTime.Today });

            var holiday = await _holidayService.GetByIdAsync(id);
            if (holiday == null) return NotFound();

            return PartialView("_AddOrEdit", holiday);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(Holidays model)
        {
            if (!ModelState.IsValid)
                return PartialView("_AddOrEdit", model);

            await _holidayService.SaveAsync(model);

            return Json(new { isValid = true });
        }

    }
}
