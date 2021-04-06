using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XSpy.Database.Entities;

namespace XSpy.Controllers.Base
{
    [Authorize, ApiController, Route("api/[controller]")]
    public class BaseApiController : ControllerBase, IController
    {
        public User LoggedUser { get; set; }

        protected Guid GetUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return idClaim == null ? Guid.Empty : Guid.Parse(idClaim);
        }
    }
}