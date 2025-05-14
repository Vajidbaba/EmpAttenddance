using Common.Core.Services;
using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class UsersController : BaseController
    {
        private readonly IUsersService _userService;
        private readonly IContextHelper _contextHelper;

        private readonly LogisticContext _dbcontext;

        public UsersController(IUsersService userService, LogisticContext dbcontext, IContextHelper contextHelper)
        {
            _userService = userService;
            _dbcontext = dbcontext;
            _contextHelper = contextHelper;
        }
        public async Task<IActionResult> List()
        {
            var userName = _contextHelper.GetUsername();
            List<Users> listEmployees = await _userService.GetAllUsers();
            return View(listEmployees);
        }
        [HttpGet]

        public IActionResult Add()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(UsersModel user)
        {
            if (user.Username != null && user.Password != null && user.Role != null)
            {
                var getLastId = _userService.GetUserCount();
                int nextId = getLastId + 1;

                UsersModel data = new UsersModel
                {
                    isActive = true,
                    UserId = "USR" + nextId.ToString("D4"),
                    Username = user.Username,
                    Password = user.Password,
                    Role = user.Role
                };

                _userService.CreateUser(data);

                Toast("User created successfully!", ToastType.SUCCESS);
                return RedirectToAction("List", "Users");
            }

            Toast("Please enter all fields", ToastType.ERROR);
            return RedirectToAction("List", "Users");
        }


        [HttpGet]
        public IActionResult Update(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var user = _userService.GetUsersDetails(Id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost]
        public IActionResult UpdateUser(int id, UsersModel user)
        {
            if (user != null)
            {
                var data = new UsersModel();
                data.Id = user.Id;
                data.isActive = user.isActive;
                data.Username = user.Username;
                data.Mobile = user.Mobile;
                data.Password = user.Password;
                data.Role = user.Role;
                _userService.UpdateUser(id, data);
                Toast("User Updated Successfully!", ToastType.SUCCESS);
                return RedirectToAction("List", "Users");
            }
            return View(user);
        }

        public IActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var user = _userService.GetUsersDetails(Id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
