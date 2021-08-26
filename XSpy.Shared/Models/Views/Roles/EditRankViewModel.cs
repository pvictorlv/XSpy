using System.Collections.Generic;
using Stock.Database.Stock.Shared.Models.Data.Roles;

namespace Stock.Shared.Models.Views.Roles
{
    public class EditRankViewModel
    {
        public List<RolesData> Roles { get; set; }
        public List<RolesData> RankRoles { get; set; }
        public RankData RankData { get; set; }
    }
}