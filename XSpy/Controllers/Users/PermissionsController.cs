using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock.Shared.Models.Views.Roles;
using XSpy.Controllers.Base;
using XSpy.Database.Services.Users;
using XSpy.Utils;

namespace StockManagement.Controllers.Users
{ 
    public class PermissionsController : BaseController
    {
        private readonly RoleService _roleService;

        public PermissionsController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [Route("edit/{rankId}"), PreExecution(Role = "ROLE_U_ROLE")]
        public async Task<IActionResult> Edit(Guid rankId)
        {
            var rank = await _roleService.GetRankById(rankId, LoggedUser);
            if (rank == null)
            {
                return RedirectToAction("list");
            }

            return View(new EditRankViewModel
            {
                RankData = rank,
                RankRoles = await _roleService.GetRoleDataForRank(rankId),
                Roles = await _roleService.HasPermission(LoggedUser.RankId, "IS_ADMIN")
                    ? await _roleService.GetRoles()
                    : await _roleService.GetRoleDataForRank(LoggedUser.RankId)
            });
        }

        [Route("list"), PreExecution(Role = "ROLE_R_ROLE")]
        public async Task<IActionResult> List()
        {
            return View(new RankListViewModel()
            {
                CanCreate = await _roleService.HasPermission(LoggedUser.RankId, "ROLE_C_ROLE"),
                CanEdit = await _roleService.HasPermission(LoggedUser.RankId, "ROLE_U_ROLE"),
            });
        }

        [Route("create"), PreExecution(Role = "ROLE_C_ROLE")]
        public async Task<IActionResult> Create()
        {
            return View(new CreateRankViewModel()
            {
                Roles = await _roleService.HasPermission(LoggedUser.RankId, "IS_ADMIN")
                    ? await _roleService.GetRoles()
                    : await _roleService.GetRoleDataForRank(LoggedUser.RankId)
            });
        }
    }
}