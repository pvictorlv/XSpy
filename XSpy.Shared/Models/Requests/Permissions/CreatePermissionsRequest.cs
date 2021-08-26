namespace Stock.Shared.Models.Requests.Permissions
{
    public class CreatePermissionsRequest
    {
        public string RankName { get; set; }

        public string[] Roles { get; set; }
    }
}