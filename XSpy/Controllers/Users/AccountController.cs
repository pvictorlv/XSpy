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
    public class AccountController : BaseController
    {
        private UserService _userService;
        private RoleService _roleService;
        
        public AccountController(UserService userService, RoleService roleService)
        {
            _roleService = roleService;
            _userService = userService;
        }



        [HttpGet("settings"), PreExecution()]
        public async Task<IActionResult> Settings([FromRoute] Guid userId)
        {
            return View();
        }

    }
}