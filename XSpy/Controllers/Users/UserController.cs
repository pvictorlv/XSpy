using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XSpy.Controllers.Base;
using XSpy.Database.Services;
using XSpy.Database.Services.Users;
using XSpy.Shared.Models.Views.User;
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
            return View();
        }

        [HttpGet("edit/{userId}"), PreExecution(Role = "ROLE_U_USER")]
        public async Task<IActionResult> Edit([FromRoute] Guid userId)
        {
            var user = await _userService.GetById(userId);
            var roles = LoggedUser.RankData.Roles.Select(s => s.RoleData).Select(s => s.Name).ToList();

            var isAdmin = roles.Any(s => s == "IS_ADMIN");

            return View(new EditUserViewModel()
            {
                User = user,
                Roles = isAdmin ? await _roleService.GetRanks() : await _roleService.GetRanks(roles),
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