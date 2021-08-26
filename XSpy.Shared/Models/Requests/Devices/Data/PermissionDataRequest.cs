using System.Collections.Generic;

namespace XSpy.Database.Models.Requests.Devices.Data
{
    public class PermissionDataRequest
    {
        public string Perm { get; set; }
        public bool IsAllowed { get; set; }
    }

    public class GrantedPermissionRequest
    {
        public List<string> Permissions { get; set; }
    }
}