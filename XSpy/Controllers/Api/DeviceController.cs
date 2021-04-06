using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XSpy.Controllers.Base;
using XSpy.Database.Services;
using XSpy.Utils;

namespace XSpy.Controllers.Api
{
    public class DeviceController : BaseApiController
    {
        private DeviceService _deviceService;

        public DeviceController(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [Route("list"), PreExecution]
        public IActionResult List()
        {
            return Ok();
        }
    }
}