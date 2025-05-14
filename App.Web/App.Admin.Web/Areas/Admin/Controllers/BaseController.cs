using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected void Toast(string message, ToastType toastType)
        {
            TempData[toastType + ""] = message;
        }
        protected void Alert(string message, ToastType toastType)
        {
            TempData["Error" + toastType + " "] = message;
        }
        


        protected enum ToastType
        {
            SUCCESS,
            ERROR,
            WARNING,
            INFO
        }
    }
}