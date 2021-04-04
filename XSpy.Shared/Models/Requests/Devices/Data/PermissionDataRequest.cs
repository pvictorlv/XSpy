using System.Collections.Generic;

namespace XSpy.Shared.Models.Requests.Devices
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