using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stock.Shared.Models.Views.User;
using XSpy.Controllers.Base;
using XSpy.Database.Models.Views.User;
using XSpy.Database.Services;
using XSpy.Database.Services.Users;
using XSpy.Utils;

namespace XSpy.Controllers.Users
{
    public class UserController : BaseController
    {
        private UserService _userService;
        private RoleService _roleService;
        
        public UserController(UserService userService, RoleService roleService)
        {
            _roleService = roleService;
            _userService = userService;
        }

        [Route("list"), PreExecution(Role = "ROLE_R_USER")]
        public async Task<IActionResult> List()
        {
            return View(new UserListViewModel
            {
                CanCreate = await _roleService.HasPermission(LoggedUser.RankId, "ROLE_C_USER"),
                CanEdit = await _roleService.HasPermission(LoggedUser.RankId, "ROLE_U_USER"),
                Ranks = await _roleService.GetRanks()
            });
        }

        [HttpGet("profile/{userId}"), PreExecution()]
        public async Task<IActionResult> Profile([FromRoute] Guid userId)
        {
            var user = await _userService.GetById(userId);
           
            return View(new UserDetailsViewModel()
            {
                User = user
            });
        }

    }
}