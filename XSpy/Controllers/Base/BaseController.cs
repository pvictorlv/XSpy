using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XSpy.Database.Entities;

namespace XSpy.Controllers.Base
{
    [Controller, Authorize, Route("[controller]")]
    public class BaseController : Controller, IController
    {
        public User LoggedUser { get; set; }
        protected Guid GetUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.Sid);
            return idClaim == null ? Guid.Empty : Guid.Parse(idClaim);
        }
    }
}