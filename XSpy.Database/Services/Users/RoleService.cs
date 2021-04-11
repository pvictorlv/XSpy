using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFCEad.Shared.Models.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using XSpy.Database.Entities;
using XSpy.Database.Entities.Roles;
using XSpy.Database.Models.Tables;
using XSpy.Database.Services.Base;
using XSpy.Shared.Models.Responses.Permission;

namespace XSpy.Database.Services.Users
{
    public class RoleService : BaseEntityService
    {
        public RoleService(DatabaseContext context, IDistributedCache cache) : base(context, cache)
        {
        }


        public Task<List<Roles>> GetRoles()
        {
            return DbContext.Roles.Where(s => s.Name != "IS_ADMIN").ToListAsync();
        }


        public async Task<Rank> GetByName(string rankName)
        {
            return await DbContext.Ranks.FirstOrDefaultAsync(s => s.Name == rankName);
        }

        public Task<List<Rank>> GetRanks(IEnumerable<string> userRoles)
        {
            return DbContext.Ranks.Where(s =>
                s.Roles.All(r => r.RoleName != "IS_ADMIN" && userRoles.Contains(r.RoleName))).ToListAsync();
        }

        public async Task<List<Rank>> GetRanks(User user)
        {
            var roles = user.RankData.Roles.Select(s => s.RoleData).Select(s => s.Name).ToList();

            var isAdmin = roles.Any(s => s == "IS_ADMIN");
            if (isAdmin)
            {
                return await GetRanks();
            }

            return await GetRanks(roles);
        }

        public IQueryable<Rank> GetRankQueryForRoles(IEnumerable<string> userRoles, bool isAdmin)
        {
            var query = DbContext.Ranks.Include(s => s.Roles).AsQueryable();
            if (!isAdmin)
            {
                query = query.Where(s =>
                    s.Roles.All(r => r.RoleName != "IS_ADMIN" && userRoles.Contains(r.RoleName)));
            }
            else
            {
                query = query.Where(s =>
                    s.Roles.All(r => r.RoleName != "IS_ADMIN"));
            }

            return query;
        }

        public async Task<DataTableResponse<RankListResponse>> GetRanksTable(DataTableRequest request,
            IEnumerable<string> userRoles, bool isAdmin)
        {
            var query = GetRankQueryForRoles(userRoles, isAdmin);

            var queryData = query.Select(s =>
                new RankListResponse
                {
                    Id = s.Id,
                    Name = s.Name
                });

            var countTotal = await queryData.LongCountAsync();

            if (request.Search != null && !string.IsNullOrEmpty(request.Search.Value))
            {
                queryData = queryData.Where(s =>
                    s.Name.Contains(request.Search.Value));
            }

            queryData = OrderResult(queryData, request);
            var total = await queryData.LongCountAsync();
            queryData = queryData.ApplyTableLimit(request);

            return new DataTableResponse<RankListResponse>()
            {
                Data = await queryData.ToListAsync(),
                Draw = request.Draw,
                RecordsFiltered = total,
                RecordsTotal = countTotal
            };
        }

        public async Task<Rank> UpdateRankRoles(Guid rankId, IEnumerable<string> roles, IEnumerable<string> fakeRoles, string rankName)
        {
            var rank = await DbContext.Ranks.FirstOrDefaultAsync(s => s.Id == rankId);
            rank.Name = rankName;
            DbContext.Ranks.Update(rank);
            DbContext.RankRoles.RemoveRange(DbContext.RankRoles.Where(s => s.RankId == rankId));
            var newRoles = roles.Select(role => new RankRole() { Id = Guid.NewGuid(), RankId = rankId, RoleName = role });
            var newFakes = fakeRoles.Select(role => new RankRole() { Id = Guid.NewGuid(), RankId = rankId, RoleName = role, FakeRole = true });
            await DbContext.RankRoles.AddRangeAsync(newRoles.Concat(newFakes));
            await DbContext.SaveChangesAsync();

            return rank;
        }

        public async Task<Rank> CreateRankWithRoles(IEnumerable<string> roles, string rankName)
        {
            var rank = new Rank()
            {
                Id = Guid.NewGuid(),
                Name = rankName
            };
            await DbContext.Ranks.AddAsync(rank);
            await DbContext.SaveChangesAsync();

            var newRoles = roles.Select(role => new RankRole() { Id = Guid.NewGuid(), RankId = rank.Id, RoleName = role });
            await DbContext.RankRoles.AddRangeAsync(newRoles);
            await DbContext.SaveChangesAsync();

            return rank;
        }

        public async Task<Rank> GetRankById(Guid rankId, bool isAdmin, IEnumerable<string> userRoles)
        {
            return await GetRankQueryForRoles(userRoles, isAdmin).FirstOrDefaultAsync(s => s.Id == rankId);
        }

        public async Task<Rank> GetRankById(Guid rankId)
        {
            return await DbContext.Ranks.FirstOrDefaultAsync(s => s.Id == rankId);
        }
        public Task<List<Rank>> GetRanks()
        {
            return DbContext.Ranks.Where(s => s.Roles.All(r => r.RoleName != "IS_ADMIN")).ToListAsync();
        }

        public bool UserIsSuperior(User user, Rank compareRank)
        {
            return compareRank.Roles.All(studentRole => user.RankData.Roles.Select(s => s.RoleName).Any(s => s == "IS_ADMIN" || s == studentRole.RoleName)); ;
        }
    }
}