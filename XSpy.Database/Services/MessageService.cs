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
using XSpy.Shared;
using XSpy.Shared.Models;
using XSpy.Shared.Models.Requests.Devices;
using XSpy.Shared.Models.Requests.Devices.Search;
using XSpy.Shared.Models.Views;
using XSpy.Extensions;
using File = XSpy.Database.Entities.Devices.File;

namespace XSpy.Database.Services
{
    public class MessageService : BaseEntityService
    {
        public MessageService(DatabaseContext context, IDistributedCache cache) : base(context, cache)
        {
        }

        public Task<List<AppMessage>> GetMessagesForContact(Guid contactId)
        {
            return DbContext.AppMessages.Where(s => s.ContactId == contactId).ToListAsync();
        }

    }
}