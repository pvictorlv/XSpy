﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using XSpy.Database.Services;
using XSpy.Models;
using XSpy.Shared.Models.Requests.Users;
using XSpy.Utils;

namespace XSpy.Controllers
{
    public class HomeController : Controller
    {
        private UserService _userService;
        public HomeController(UserService userService)
        {
            _userService = userService;
        }

        [Route("/"), AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost("/"), AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginRequest loginData)
        {
            var user = await _userService.Login(loginData.Username, loginData.Password);
            if (user == null)
            {
                ViewBag.Error = "Usuário ou senha incorreto!";
            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal claimPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authenticationProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.ToLocalTime().AddDays(365),
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal,
                    authenticationProperties);
                
                return RedirectToAction("Dashboard");
            }

            return View();
        }


        [Route("/logout"), PreExecution()]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
        [Route("/dashboard"), PreExecution()]
        public async Task<IActionResult> Dashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}