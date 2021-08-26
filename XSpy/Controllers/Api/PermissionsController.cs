using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock.Shared.Models.Requests.Permissions;
using XSpy.Controllers.Base;
using XSpy.Database.Models.Tables;
using XSpy.Database.Services.Users;
using XSpy.Utils;

namespace StockManagement.Controllers.Api.Users
{
    public class PermissionsController : BaseApiController
    {
        private RoleService _roleRepository;

        public PermissionsController(RoleService roleService)
        {
            _roleRepository = roleService;
        }

        [HttpPatch, PreExecution(Role = "ROLE_U_ROLE")]
        public async Task<IActionResult> EditRank(EditPermissionsRequest request)
        {
            var roles = await _roleRepository.GetRolesForRank(LoggedUser.RankId);
            var isAdmin = roles.Any(s => s == "IS_ADMIN");

            request.FakeRoles ??= new string[0];

            var fakeRoles = isAdmin ? request.FakeRoles : request.FakeRoles.Where(s => roles.Contains(s));

            var newRoles = isAdmin
                ? request.Roles.Where(s => !fakeRoles.Contains(s))
                : request.Roles.Where(s => roles.Contains(s) && !fakeRoles.Contains(s));


            await _roleRepository.UpdateRankRoles(request.RankId, newRoles, fakeRoles, request.RankName);


            return Ok(request);
        }

        [HttpPut, PreExecution(Role = "ROLE_C_ROLE")]
        public async Task<IActionResult> EditRank(CreatePermissionsRequest request)
        {
            var roles = await _roleRepository.GetRolesForRank(LoggedUser.RankId);
            var isAdmin = roles.Any(s => s == "IS_ADMIN");

            var newRoles = isAdmin ? request.Roles : request.Roles.Where(s => roles.Contains(s));

            var rank = await _roleRepository.CreateRankWithRoles(newRoles, request.RankName);

            return Created("", rank);
        }

        [HttpPost("list"), PreExecution(Role = "ROLE_R_ROLE")]
        public async Task<IActionResult> GetRanks(DataTableRequest request)
        {
            return Ok(await _roleRepository.GetRanksTable(request, LoggedUser));
        }
    }
}