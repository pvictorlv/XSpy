using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using XSpy.Database.Services;
using Microsoft.AspNetCore.Http;
using XSpy.Database.Services.Users;

namespace XSpy.Socket.Auth
{
    public class HubAuthorizationHandler : AuthorizationHandler<HubAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;


        public HubAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            HubAuthorizationRequirement requirement)
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("user-token", out var tokenStr))
            {
                context.Fail();
                return;
            }

            if (!Guid.TryParse(tokenStr, out var token))
            {
                context.Fail();
                return;
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<UserService>();

                var user = await userService.IsValidToken(token);
                if (!user)
                {
                    context.Fail();
                }
                else
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}