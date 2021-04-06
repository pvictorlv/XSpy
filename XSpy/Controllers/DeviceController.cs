using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XSpy.Controllers.Base;
using XSpy.Utils;

namespace XSpy.Controllers
{
    [Authorize, Controller, Route("[controller]")]
    public class DeviceController : BaseController
    {
        public DeviceController()
        {
            
        }


        [Route("list"), PreExecution]

        public IActionResult List()
        {
            return View();
        }
    }
}