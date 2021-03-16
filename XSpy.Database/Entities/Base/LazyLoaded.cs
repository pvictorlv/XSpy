using Microsoft.EntityFrameworkCore.Infrastructure;

namespace XSpy.Database.Entities.Base
{
    public class LazyLoaded
    {
        protected ILazyLoader LazyLoader { get; set; }

        public LazyLoaded()
        {
            
        }
        public LazyLoaded(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
    }
}