using Microsoft.Extensions.Caching.Distributed;
using XSpy.Database.Services.Base;

namespace XSpy.Database.Services
{
    public class DeviceService : BaseEntityService
    {
        public DeviceService(DatabaseContext context, IDistributedCache cache) : base(context, cache)
        {
        }
        
        
    }
}