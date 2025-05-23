using Microsoft.AspNetCore.Mvc;
using System;

namespace App.Admin.Web.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected void Toast(string message, ToastType toastType)
        {
            TempData[toastType.ToString()] = message;
        }

        protected void Alert(string message, ToastType toastType)
        {
            TempData["Error" + toastType.ToString()] = message;
        }

        protected enum ToastType
        {
            SUCCESS,
            ERROR,
            WARNING,
            INFO
        }
    }

    public static class DateExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
