using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CFCEad.Shared.Models.Requests.Financial;
using CreditCardValidator;
using JunoApi;
using JunoApi.Notifications;
using JunoApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using XSpy.Controllers.Base;
using XSpy.Database.Models.Data.Financial;
using XSpy.Database.Models.Tables;
using XSpy.Database.Services.Financial;
using XSpy.Database.Services.Users;
using XSpy.Utils;

namespace XSpy.Controllers.Api.Financial
{
    public class FinancialController : BaseApiController
    {
        private readonly FinancialService _financialService;
        private readonly UserService _userService;

        public FinancialController(
            FinancialService financialService,
            UserService userService)
        {
            _userService = userService;
            _financialService = financialService;
        }

        [HttpPost("orders/list"), PreExecution(Role = "IS_SCHOOL_FINANCIAL_ADMIN")]
        public async Task<IActionResult> GetOrderList(DataTableRequest<ListOrdersRequest> request)
        {
            var orders = await _financialService.GetTable(request, LoggedUser);
            return Ok(orders);
        }

        [HttpPost("payment/accept"), PreExecution(Role = "IS_FINANCIAL_ADMIN")]
        public async Task<IActionResult> AcceptPayment(AcceptPaymentRequest request)
        {
            var transaction = await _financialService.GetById(request.PaymentId);
            if (transaction.PaymentStatus != PaymentStatus.Pending)
                return Ok();

            var user = await _userService.GetById(transaction.UserData.Id.Value);

            //todo
            //  await _financialService.AcceptTransaction(transaction, user.SchoolId.Value);

            return Ok(request);
        }

        [HttpPost("purchase/card"), PreExecution(Role = "IS_SCHOOL_FINANCIAL_ADMIN")]
        public async Task<IActionResult> PurchaseCredits(PurchaseCreditRequest request)
        {
            if (request.Amount <= 0)
                return BadRequest();

            CreditCardDetector detector = new CreditCardDetector(request.CreditCard.CardNumber);
            if (!detector.IsValid())
            {
                return BadRequest("Cartão inválido");
            }

            request.CreditCard.CardNumber = Regex.Replace(request.CreditCard.CardNumber, "[^0-9]", string.Empty);

            request.CreditCard.Brand = detector.Brand switch
            {
                //Visa / Master / Amex / Elo / Aura / JCB / Diners / Discover / Hipercard
                CardIssuer.AmericanExpress => "Amex",
                CardIssuer.Visa => "Visa",
                CardIssuer.MasterCard => "Master",
                CardIssuer.JCB => "JCB",
                CardIssuer.DinersClub => "Diners",
                CardIssuer.Discover => "Discover",
                CardIssuer.Hipercard => "Hipercard",
                _ => detector.BrandName
            };


            //todo get plan price!

            var transaction = await _financialService.CreateCreditCardTransaction(LoggedUser,
                request, 0);

            //    var discount = await _financialService.ApplyVoucher(transaction, request.VoucherCode);

            // transaction = await _cieloApi.Purchase(LoggedUser, transaction, request.CreditCard, discount);

            if (transaction.PaymentStatus == PaymentStatus.Success)
            {
                //       await _financialService.AcceptTransaction(transaction, school.Id);
            }
            else
            {
                //       await _financialService.SaveTransaction(transaction);
            }


            return Ok(transaction);
        }


        [HttpPost("purchase/deposit"), PreExecution()]
        public async Task<IActionResult> Get(PurchaseDepositRequest request)
        {
            var planData = await _financialService.GetPlanById(request.PlanId);
            var price = planData.PriceCents;

            var transaction = await _financialService.CreateDepositTransaction(LoggedUser,
                planData.AppendDays.GetValueOrDefault(0),
                price.GetValueOrDefault(0));

            var auth = new JunoAuthorization("T8llYSko6B67EsqS", "OjORdH9OQxgPlsgS>A#_r)+fvS|0=l+");
            var token = await auth.GenerateTokenAsync();
            var charge = new JunoCharge(token.AccessToken,
                "A5DB8257C3A8C03DA5F898D12F7EC89B8AB449F9970C77D3AA710C9350F958A7");

            var chargeRequest = await charge.GenerateBillingBillAsync(new ChargeRequest()
            {
                Billing = new Billing()
                {
                    BirthDate = LoggedUser.BirthDate?.ToString("yyyy-MM-dd"),
                    Document = LoggedUser.Document,
                    Email = LoggedUser.Email,
                    Name = LoggedUser.Name,
                    Notify = true
                },
                Charge = new Charge
                {
                    PixKey = Guid.Parse("290bac19-0189-4a28-aa6d-f900908f849c"),
                    Amount = price.GetValueOrDefault(0),
                    Description = planData.Name,
                    DiscountAmount = 0,
                    DiscountDays = 0,
                    PaymentAdvance = false,
                    PaymentTypes = new List<string> {
                        "BOLETO_PIX"
                    },
                    DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                }
            });
            return Ok(chargeRequest);
        }


        [HttpPost("notification"), ApiExplorerSettings(IgnoreApi = true), AllowAnonymous]
        public async Task<IActionResult> Notification([FromForm] ChargeNotification notification)
        {
            //todo

            return Ok();
        }
    }
}