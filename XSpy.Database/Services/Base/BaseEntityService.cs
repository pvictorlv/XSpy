using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using XSpy.Database.Extensions;
using XSpy.Database.Models.Tables;

namespace XSpy.Database.Services.Base
{
    public class BaseEntityService
    {
        internal DatabaseContext DbContext;
        private readonly IDistributedCache _cache;

        public BaseEntityService(DatabaseContext context, IDistributedCache cache)
        {
            DbContext = context;
            _cache = cache;
        }

        public Task Commit()
        {
            return DbContext.SaveChangesAsync();
        }

        public void Save<T>(T entity) where T : class
        {
            DbContext.Update(entity);
        }

        public Task SetCache(string key, string value)
        {
            return _cache.SetStringAsync(key, value);
        }

        public Task RemoveCache(string key)
        {
            return _cache.RemoveAsync(key);
        }

        public Task<string> GetStringFromCache(string key)
        {
            return _cache.GetStringAsync(key);
        }

        public async Task<T> GetFromCache<T>(string key)
        {
            var cached = await GetStringFromCache(key).ConfigureAwait(false);
            if (string.IsNullOrEmpty(cached))
                return default;

            return JsonConvert.DeserializeObject<T>(cached);
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