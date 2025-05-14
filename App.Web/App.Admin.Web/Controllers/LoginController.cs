using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Admin.Web.Controllers
{
    public class LoginController : BaseController
    {
        private readonly LogisticContext _dbcontext;
        private const string ACCOUNT_UID_PASSWORD = "Username or Password Incorrect";
        private const string ACCOUNT_DESABLE = "Username or Password Incorrect";


        public LoginController(LogisticContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new LoginVM();
#if DEBUG
            model.UserId = "USR0002";
#endif
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                Toast(ACCOUNT_UID_PASSWORD, ToastType.ERROR);
                return View(model);
            }
            var userInfo = new Users();

            try
            {
                userInfo = _dbcontext.Users.Where(x => x.Active == true && x.UserId == model.UserId && x.Password == model.Password).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
            if (userInfo == null || string.IsNullOrEmpty(userInfo.UserId))
            {
                Toast("Please enter user Id", ToastType.ERROR);
                return View(model);
            }
            if (!userInfo.Active)
            {
                Toast("In Active", ToastType.ERROR);
                return View(model);
            }
            return CreateIdentity(model.UserId, userInfo.Role);
        }

        [NonAction]
        private IActionResult CreateIdentity(string UserId, string role)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, UserId),
                    new Claim(ClaimTypes.Role, role)
                };

            var claimIdentiy = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
            };
            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentiy), authProperties);
            return Redirect($"~/admin/employees/list");
        }

        public IActionResult SignOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
