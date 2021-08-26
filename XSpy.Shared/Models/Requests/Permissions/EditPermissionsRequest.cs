using System;

namespace Stock.Shared.Models.Requests.Permissions
{
    public class EditPermissionsRequest
    {
        public string RankName { get; set; }
        public Guid RankId { get; set; }

        public string[] Roles { get; set; }
        public string[] FakeRoles { get; set; }
    }
}