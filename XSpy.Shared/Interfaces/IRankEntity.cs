using System;
using System.Collections.Generic;

namespace XSpy.Database.Interfaces
{
    public interface IRankEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public IEnumerable<IRankRoleEntity> Roles { get; set; }
    }
}