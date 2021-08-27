﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CFCEad.Shared.Models.Requests.Financial;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Stock.Shared.Models.Data;
using XSpy.Database.Entities;
using XSpy.Database.Entities.Financial;
using XSpy.Database.Models.Data.Financial;
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

        public async Task AcceptTransaction(Transaction transaction, Guid schoolId)
        {
            //todo plan data!
            transaction.PaymentStatus = PaymentStatus.Success;
            DbContext.Transactions.Update(transaction);

            await Commit();
        }

        public async Task<Transaction> CreateCreditCardTransaction(User user, PurchaseCreditRequest request, double price)
        {
            if (request.Installments <= 0)
                request.Installments = 1;

            if (request.Installments > 12)
                request.Installments = 12;


            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Value = (decimal)(price * request.Amount),
                UserId = user.Id,
                Quantity = request.Amount,
                Installments = request.Installments,
                PaymentStatus = PaymentStatus.Pending,
                PaymentMethod = PaymentMethod.CreditCard,
                CreatedAt = DateTime.Now
            };

            await DbContext.Transactions.AddAsync(transaction);

            await Commit();

            return transaction;
        }
        public async Task<Transaction> CreateDepositTransaction(User user, int quantity, decimal price)
        {
            //todo: get price from db
            const int tax = 200;

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Value = price,
                UserId = user.Id,
                Quantity = quantity,
                PaymentStatus = PaymentStatus.Pending,
                PaymentMethod = PaymentMethod.Deposit,
                CreatedAt = DateTime.Now,
                TaxValue = tax / (decimal)100,
                Installments = 1
            };


            await DbContext.Transactions.AddAsync(transaction);
            await Commit();
            return transaction;
        }

        public async Task<DataTableResponse<TransactionData>> GetTable(DataTableRequest<ListOrdersRequest> request,
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
                    query = query.Where(s => s.User.Email == request.Filter.Username);
                }

                if (!string.IsNullOrEmpty(request.Filter.FullName))
                {
                    query = query.Where(s => s.User.Name == request.Filter.FullName);
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

            var queryData = query.Select(s => new TransactionData
            {
                Id = s.Id,
                UserData = new UserData
                {
                    Name = s.User.Name,
                    Email = s.User.Email
                },
                CreatedAt = s.CreatedAt,
                PaymentStatus = s.PaymentStatus,
                PaymentMethod = s.PaymentMethod,
                ExtraData = s.ExtraData,
                Value = s.Value,
                Quantity = s.Quantity
            });

            var total = await queryData.LongCountAsync();

            queryData = OrderResult(queryData, request).ApplyTableLimit(request);

            var data = await queryData.ToListAsync();

            return new DataTableResponse<TransactionData>()
            {
                Data = data,
                Draw = request.Draw,
                RecordsFiltered = total,
                RecordsTotal = countTotal
            };
        }

        public Task<TransactionData> GetById(Guid id)
        {
            return DbContext.Transactions.Where(s => s.Id == id)
                .Select(s => new TransactionData
                {
                    UserData = new UserData
                    {   Id                     = s.UserId,
                        Name = s.User.Name,
                        Email = s.User.Email
                    },
                    CreatedAt = s.CreatedAt,
                    PaymentStatus = s.PaymentStatus,
                    PaymentMethod = s.PaymentMethod,
                    ExtraData = s.ExtraData,
                    Value = s.Value,
                    Quantity = s.Quantity
                })
                .FirstOrDefaultAsync();
        }
    }
}