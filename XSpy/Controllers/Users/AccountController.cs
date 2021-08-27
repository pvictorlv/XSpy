﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stock.Shared.Models.Views.User;
using XSpy.Controllers.Base;
using XSpy.Database.Models.Views.Account;
using XSpy.Database.Models.Views.User;
using XSpy.Database.Services;
using XSpy.Database.Services.Users;
using XSpy.Utils;

namespace XSpy.Controllers.Users
{
    public class AccountController : BaseController
    {
        private UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet("settings"), PreExecution()]
        public async Task<IActionResult> Settings()
        {
            var user = await _userService.GetById(GetUserId());
            var address = await _userService.GetUserAddressByUserId(GetUserId());

            return View(new SettingsViewModel()
            {
                UserData = user,
                AddressData = address
            });
        }
    }
}