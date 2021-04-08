using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XSpy.Controllers.Base;
using XSpy.Database.Services;
using XSpy.Shared.Models.Views.Device;
using XSpy.Utils;

namespace XSpy.Controllers
{
    public class DeviceController : BaseController
    {
        private DeviceService _deviceService;

        public DeviceController(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [Route("{deviceId}/phone"), PreExecution]
        public async Task<IActionResult> Phone(Guid deviceId)
        {
            var data = await _deviceService.GetDeviceById(deviceId);

            return View(new DeviceDataViewModel
            {
                Device = data
            });
        }

        [Route("list"), PreExecution]
        public IActionResult List()
        {
            return View();
        }
    }
}