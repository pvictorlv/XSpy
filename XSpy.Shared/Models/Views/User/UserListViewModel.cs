using System.Collections.Generic;
using Stock.Database.Stock.Shared.Models.Data.Roles;

namespace Stock.Shared.Models.Views.User
{
    public class UserListViewModel
    {
        public IEnumerable<RankData> Ranks { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
    }
}