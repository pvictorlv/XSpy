using System;
using System.Collections.Generic;
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
using XSpy.Database.Models.Tables;
using XSpy.Database.Services.Base;
using File = XSpy.Database.Entities.Devices.File;

namespace XSpy.Database.Services
{
    public class MessageService : BaseEntityService
    {
        public MessageService(DatabaseContext context, IMemoryCache cache) : base(context, cache)
        {
        }

        public Task<List<AppMessage>> GetMessagesForContact(Guid contactId)
        {
            return DbContext.AppMessages.Where(s => s.ContactId == contactId).ToListAsync();
        }

    }
}