using System;

namespace XSpy.Shared.Models.Requests.Users
{
    public class SearchUserRequest
    {
        public string FullName { get; set; }
        public Guid? RankId { get; set; }
        public bool? IsActive { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}