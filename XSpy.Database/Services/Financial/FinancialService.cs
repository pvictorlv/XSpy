using Microsoft.Extensions.Caching.Distributed;
using XSpy.Database.Services.Base;

namespace XSpy.Database.Services.Financial
{
    public class FinancialService : BaseEntityService
    {
        public FinancialService(DatabaseContext context, IDistributedCache cache) : base(context, cache)
        {
        }
    }
}