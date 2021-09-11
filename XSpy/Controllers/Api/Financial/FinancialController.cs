using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CFCEad.Shared.Models.Requests.Financial;
using CreditCardValidator;
using JunoApi;
using JunoApi.Models;
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
using Charge = JunoApi.Requests.Charge;

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
            if (LoggedUser.UserAddress == null)
                return BadRequest("Você deve cadastrar seu endereço");

            var planData = await _financialService.GetPlanById(request.PlanId);

            var auth = new JunoAuthorization("ExPNu0oatZdjXAze", ".q;#_WaGPP~M-3in;^fU|OM68EVTlp*^");
            var token = await auth.GenerateTokenAsync();
            var charge = new JunoCharge(token.AccessToken,
                "EEF49FAD646CAFCD83DFF834AA2B045169A0553A1C862A55382A5EEC829B0F85 ");


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
                    Amount = planData.PriceCents.Value / 100d,
                    Description = planData.Name,
                    PaymentAdvance = false,
                    PaymentTypes = new List<string>
                    {
                        "CREDIT_CARD"
                    },
                    DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                }
            });

            if (!string.IsNullOrEmpty(chargeRequest.Error))
            {
                return BadRequest(chargeRequest);
            }

            var paymentRequest = new JunoPayment(token.AccessToken,
                "EEF49FAD646CAFCD83DFF834AA2B045169A0553A1C862A55382A5EEC829B0F85 ");
            var payment = await paymentRequest.DoPaymentAsync(new PaymentRequest()
            {
                Billing = new Billing()
                {
                    Email = LoggedUser.Email,
                    Address = new Address()
                    {
                        Street =  LoggedUser.UserAddress.Street,
                        Number = LoggedUser.UserAddress.Number,
                        City = LoggedUser.UserAddress.City,
                        State = LoggedUser.UserAddress.State,
                        PostCode = LoggedUser.UserAddress.Zip
                    }
                },
                ChargeId = chargeRequest.Embedded.Charges.FirstOrDefault()?.Id,
                CreditCardDetails = new CreditCardDetails()
                {
                    CreditCardHash = request.CardHash
                }
            });

            if (payment == null || payment.Payments.FirstOrDefault().Status == "ERROR")
            {
                return BadRequest(payment);
            }

            var transaction = await _financialService.CreateCreditCardTransaction(LoggedUser, request,
                planData, JsonConvert.SerializeObject(chargeRequest));

            return Ok(transaction);
        }


        [HttpPost("purchase/deposit"), PreExecution()]
        public async Task<IActionResult> Get(PurchaseDepositRequest request)
        {
            var planData = await _financialService.GetPlanById(request.PlanId);
            var price = planData.PriceCents;


            var auth = new JunoAuthorization("ExPNu0oatZdjXAze", ".q;#_WaGPP~M-3in;^fU|OM68EVTlp*^");
            var token = await auth.GenerateTokenAsync();
            var charge = new JunoCharge(token.AccessToken,
                "EEF49FAD646CAFCD83DFF834AA2B045169A0553A1C862A55382A5EEC829B0F85 ");

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
                    Amount = price.GetValueOrDefault(0) / 100d,
                    Description = planData.Name,
                    PaymentAdvance = false,
                    PaymentTypes = new List<string>
                    {
                        "BOLETO_PIX"
                    },
                    DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                }
            });

            if (string.IsNullOrEmpty(chargeRequest.Error))
            {
                var transaction = await _financialService.CreateDepositTransaction(LoggedUser,
                    planData, JsonConvert.SerializeObject(chargeRequest));

                return Ok(chargeRequest.Embedded);
            }

            return BadRequest(chargeRequest.Embedded);
        }


        [HttpPost("notification"), ApiExplorerSettings(IgnoreApi = true), AllowAnonymous]
        public async Task<IActionResult> Notification([FromForm] ChargeNotification notification)
        {
            //todo

            return Ok();
        }
    }
}