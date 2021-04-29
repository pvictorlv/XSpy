using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using XSpy.Controllers.Base;
using XSpy.Database.Models.Tables;
using XSpy.Database.Services;
using XSpy.Shared.Models.Requests.Devices;
using XSpy.Shared.Models.Requests.Devices.Search;
using XSpy.Shared.Models.Requests.Users;
using XSpy.Socket;
using XSpy.Utils;

namespace XSpy.Controllers.Api
{
    public class UserController : BaseApiController
    {
        private UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> ApiLogin(LoginRequest loginData)
        {
            if (User?.Identity != null && User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            var user = await _userService.Login(loginData.Username, loginData.Password);
            if (user == null)
                return Unauthorized();


            return Ok(new
            {
                DeviceToken = user.DeviceToken
            });
        }

        [Route("list"), PreExecution(Role = "ROLE_R_ROLE")]
        public async Task<IActionResult> List(DataTableRequest<SearchUserRequest> request)
        {
            return Ok(await _userService.GetTable(request, LoggedUser));
        }
    }
}