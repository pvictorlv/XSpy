using System;

namespace XSpy.Shared.Models.Responses.Users
{
    public class UserListResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string RankName { get; set; }
    }
}