using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using XSpy.Database.Services;
using XSpy.Database.XSpy.Shared.DataTransfer;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared.DataTransfer;

namespace XSpy.Socket
{
    [Authorize(Policy = "HubAuthorizationPolicy")]
    public class MainHub : Hub
    {
        private readonly HubMethods _methods;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        public MainHub(HubMethods hubMethods, DeviceService deviceService, UserService userService)
        {
            _userService = userService;
            _deviceService = deviceService;
            _methods = hubMethods;
        }

        public override async Task OnConnectedAsync()
        {
            _methods.ClientsByUserId[GetUserId()] = Context.ConnectionId;
            
            //todo
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await GetUserAsync().ConfigureAwait(false);
            var userId = user.Id;

            _methods.ClientsByUserId.TryRemove(userId, out _);

            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }

        //Process Files
        public void _0xFI()
        {

        }
        //Call List
        public void _0xCL(List<CallData> callsList)
        {

        }

        private string GetDeviceId()
        {
            var userId = Context.User.FindFirst(ClaimTypes.SerialNumber);
            return userId?.Value;
        }
        
        private Guid GetUserToken()
        {
            var userId = Context.User.FindFirst(ClaimTypes.Sid);
            return userId == null ? Guid.Empty : Guid.Parse(userId.Value);
        }
    }
}