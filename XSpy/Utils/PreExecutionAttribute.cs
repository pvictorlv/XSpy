using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using XSpy.Controllers.Base;
using XSpy.Database.Services;

namespace XSpy.Utils
{
    public class PreExecutionAttribute : ActionFilterAttribute
    {
        private IServiceScope _scope;

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            _scope?.Dispose();
            _scope = null;
            base.OnResultExecuted(context);
        }

        public string Role { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is IController internalController)
            {
                var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.Sid);

                if (!string.IsNullOrEmpty(userId))
                {
                    _scope = context.HttpContext.RequestServices.CreateScope();
                    var userRepo = _scope.ServiceProvider.GetService<UserService>();
                    var user = await userRepo.GetById(Guid.Parse(userId));
                    if (user == null)
                    {
                        await context.HttpContext.SignOutAsync();
                        return;
                    }

                    internalController.LoggedUser = user;
                    
                    if (!string.IsNullOrEmpty(Role))
                    {
                        if (!user.HasPermission(Role))
                        {
                            context.Result = new RedirectResult("/");
                            return;
                        }
                    }
                    
                    if (context.Controller is Controller mvcController)
                    {
                        mvcController.ViewBag.User = user;
                    }
                    
                }
                
                await base.OnActionExecutionAsync(context, next);
            }
        }
    }
}