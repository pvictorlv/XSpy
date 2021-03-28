using Microsoft.Extensions.Caching.Distributed;
using XSpy.Database.Services.Base;

namespace XSpy.Database.Services.Users
{
    public class RoleService : BaseEntityService
    {
        public RoleService(DatabaseContext context, IDistributedCache cache) : base(context, cache)
        {
        }
    }
}