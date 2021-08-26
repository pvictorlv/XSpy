using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using XSpy.Database.Entities;
using XSpy.Database.Entities.Financial;
using XSpy.Database.Models.Responses;
using XSpy.Database.Models.Tables;
using XSpy.Database.Services.Base;

namespace XSpy.Database.Services.Financial
{
    public class FinancialService : BaseEntityService
    {
        public FinancialService(DatabaseContext context, IMemoryCache cache) : base(context, cache)
        {
        }

        public async Task<DataTableResponse<ListOrdersResponse>> GetTable(DataTableRequest<ListOrdersRequest> request,
            User user)
        {
            var query = DbContext.Transactions.AsQueryable();

            if (!await HasPermission(user.RankId, "ROLE_R_ALL_FINANCIAL"))
                query = query.Where(s => s.UserId == user.Id);

            var countTotal = await query.LongCountAsync();

            if (request.Filter != null)
            {
                if (!string.IsNullOrEmpty(request.Filter.Username))
                {
                    query = query.Where(s => s.User.UserName == request.Filter.Username);
                }

                if (!string.IsNullOrEmpty(request.Filter.FullName))
                {
                    query = query.Where(s => s.User.FullName == request.Filter.FullName);
                }

                if (request.Filter.PaymentStatus != null)
                {
                    query = query.Where(s => s.PaymentStatus == request.Filter.PaymentStatus);
                }
            }


            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                var intStringVal = Regex.Replace(request.Search.Value, "[^0-9]", string.Empty);

                if (!string.IsNullOrEmpty(intStringVal))
                {
                    var intVal = Convert.ToUInt32(intStringVal);
                    query = query.Where(s => s.Value >= intVal || s.Value < intVal);
                }
            }

            var queryData = query.Select(s => new ListOrdersResponse
            {
                Id = s.Id,
                Username = s.User.UserName,
                FullName = s.User.FullName,
                CreatedAt = s.CreatedAt,
                PaymentStatus = s.PaymentStatus,
                PaymentType = s.PaymentType,
                UserId = s.UserId,
                PaymentMethod = s.PaymentMethod,
                ExtraData = s.ExtraData,
                Value = s.Value,
                Quantity = s.Quantity
            });

            var total = await queryData.LongCountAsync();

            queryData = OrderResult(queryData, request).ApplyTableLimit(request);

            var data = await queryData.ToListAsync();

            return new DataTableResponse<ListOrdersResponse>()
            {
                Data = data,
                Draw = request.Draw,
                RecordsFiltered = total,
                RecordsTotal = countTotal
            };
        }
        public async Task<Transaction> GetById(Guid id)
        {
            return await DbContext.Transactions.FirstOrDefaultAsync(s => s.Id == id);
        }

    }
}