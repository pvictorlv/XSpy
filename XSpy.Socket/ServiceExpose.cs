using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using XSpy.Socket.Auth;

namespace XSpy.Socket
{
    public static  class ServiceExpose
    {
        public static IServiceCollection AddSocketsServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<HubMethods>();
          
            serviceCollection.AddSingleton<IAuthorizationHandler, HubAuthorizationHandler>();
            serviceCollection.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            return serviceCollection;
        }
    }
}
