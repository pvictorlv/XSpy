using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using XSpy.Database.Models.Tables;

namespace XSpy.Database.Services.Base
{
    public class BaseEntityService
    {
        internal DatabaseContext DbContext;
        private readonly IMemoryCache _cache;

        public BaseEntityService(DatabaseContext context, IMemoryCache cache)
        {
            DbContext = context;
            _cache = cache;
        }


        public async Task<bool> RankIsSuperior(Guid rankId, Guid compareRank)
        {
            var rankRoles = await DbContext.RankRoles.Where(s => s.RankId == rankId).Select(s => s.RoleName)
                .ToListAsync();
            var roles = await DbContext.RankRoles.Where(s => s.RankId == compareRank).ToListAsync();
            return roles.All(role =>
                rankRoles.Any(s => s == "IS_ADMIN" || s == role.RoleName));
            ;
        }

        public async Task<List<string>> GetUserRoles(Guid rankId)
        {
            var cached = GetFromCache<List<string>>("roles-" + rankId);
            if (cached == null)
            {
                var roles = await DbContext.RankRoles.Where(s => s.RankId == rankId).Select(s => s.RoleName)
                    .ToListAsync();

                SetCache("roles-" + rankId, roles);
                return roles;
            }

            return cached;
        }


        protected async Task<List<string>> GetAllRoles(Guid rankId)
        {
            var cached = GetFromCache<List<string>>("allroles-" + rankId);
            if (cached == null || cached.Count <= 0)
            {
                var roles = await DbContext.RankRoles.Where(s => s.RankId == rankId)
                    .Select(s => s.RoleName)
                    .ToListAsync();

                SetCache("allroles-" + rankId, roles);
                return roles;
            }

            return cached;
        }

        public async Task<bool> HasPermission(Guid rankId, string roleName, bool ignoreFake = false)
        {
            List<string> roles;
            if (!ignoreFake)
            {
                roles = await GetUserRoles(rankId);
            }
            else
            {
                roles = await GetAllRoles(rankId);
            }

            return roles.Any(s => s == roleName || s == "IS_ADMIN");
        }

        public Task Commit()
        {
            return DbContext.SaveChangesAsync();
        }

        public void Save<T>(T entity) where T : class
        {
            DbContext.Update(entity);
        }
        
        public void SetCache<T>(string key, T value)
        {
            _cache.Set(key, value, TimeSpan.FromMinutes(15));
        }

        public void RemoveCache(string key)
        {
            _cache.Remove(key);
        }

        public T GetFromCache<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        protected string CalculateMd5(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data =
                    md5Hash.ComputeHash(
                        Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                foreach (var t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        protected DateTime JavaTimeStampToDateTime(double javaTimeStamp)
        {
            // Java timestamp is milliseconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public IQueryable<T> OrderResult<T>(IQueryable<T> query, DataTableRequest dataTableRequest)
        {
            var orderedColumns = dataTableRequest.GetOrderedColumns();

            if (orderedColumns == null || orderedColumns.Count <= 0)
                return query;

            var firstOrder = true;

            foreach (var orderedColumn in orderedColumns)
            {
                if (orderedColumn?.Column != null && orderedColumn.Order != null)
                {
                    string column = orderedColumn.Column.Data != null && orderedColumn.Column.Data != "null"
                        ? orderedColumn.Column.Data
                        : orderedColumn.Column.Name;

                    if (firstOrder)
                    {
                        firstOrder = false;
                        query = OrderResult(query, orderedColumn, column);
                    }
                    else
                        query = SecondOrderResult(query, orderedColumn, column);
                }
            }


            return query;
        }


        public IQueryable<T> OrderResult<T, TKey>(IQueryable<T> query, DataTableRequest dataTableRequest,
            Expression<Func<T, TKey>> keySelector)
        {
            var orderedColumn = dataTableRequest.GetOrderedColumn();

            if (orderedColumn?.Column != null && orderedColumn.Order != null)
            {
                string column = orderedColumn.Column.Data != null && orderedColumn.Column.Data != "null"
                    ? orderedColumn.Column.Data
                    : orderedColumn.Column.Name;

                query = OrderResult(query, orderedColumn, column);
            }
            else
            {
                query = query.OrderBy(keySelector);
            }

            return query;
        }

        public virtual IQueryable<T> OrderResult<T>(IQueryable<T> query, ColumnOrderDataTable orderedColumn,
            string column)
        {
            return orderedColumn.Order.Dir == "asc" ? query.OrderBy(column) : query.OrderByDescending(column);
        }

        public virtual IQueryable<T> SecondOrderResult<T>(IQueryable<T> query, ColumnOrderDataTable orderedColumn,
            string column)
        {
            return orderedColumn.Order.Dir == "asc" ? query.ThenBy(column) : query.ThenByDescending(column);
        }
    }
}