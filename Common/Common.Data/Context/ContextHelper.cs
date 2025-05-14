using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Common.Data.Context
{

    public interface IContextHelper
    {
        string GetUsername();
    }

    public class ContextHelper : IContextHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ContextHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

        }
        public string GetUsername()
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                string userName = _httpContextAccessor.HttpContext.User.Identity.Name;
                return $"{userName}";
            }

            return ("User is not authenticated.");
        }
    }

}
