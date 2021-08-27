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

        [HttpPost("purchase/credits"), PreExecution(Role = "IS_SCHOOL_FINANCIAL_ADMIN")]
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



        [HttpGet("deposit/{quantity}"), PreExecution()]

        public async Task<IActionResult> Get(int quantity, [FromQuery] string voucher)
        {
            var price = 1;
            
            var transaction = await _financialService.CreateDepositTransaction(LoggedUser, quantity,
                (decimal)(price * quantity));
          
            var auth = new JunoAuthorization("T8llYSko6B67EsqS", "OjORdH9OQxgPlsgS>A#_r)+fvS|0=l+");
            var token = await auth.GenerateTokenAsync();
            var charge = new JunoCharge(token.AccessToken, "A5DB8257C3A8C03DA5F898D12F7EC89B8AB449F9970C77D3AA710C9350F958A7");
          
            //charge.
            /*
            var discount = await _financialService.ApplyVoucher(transaction, voucher);

            var items = new List<BoletoItem>
            {
                new BoletoItem
                {
                    ItemId = 1,
                    Description = $"Crédito Teórico Online ({quantity})",
                    PriceCents = (uint) (price * 100),
                    Quantity = quantity
                },
                new BoletoItem
                {
                    ItemId = 2,
                    Description = "Custos operacionais",
                    Quantity = 1,
                    PriceCents = (uint) (transaction.TaxValue * 100)
                }
            };


            var email = LoggedUser.Email;
            if (string.IsNullOrEmpty(email))
            {
                email = LoggedUser.School.Email;
            }

            var data = new Dictionary<string, dynamic>
            {
                {"apiKey", "apk_42464491-iqvIMwgDZXssmyVvOmNyjBfNrHTfsLsa"},
                {"order_id", transaction.Id},
                {"payer_email", email},
                {"payer_name", LoggedUser.FullName},
                {"payer_cpf_cnpj", LoggedUser.School.Cnpj},
                {"days_due_date", 3},
                {"type_bank_slip", "boletoA4"},
                {"items", items}
            };

            if (discount > 0)
                data["discount_cents"] = (uint)(discount * 100);

            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(new Uri("https://api.paghiper.com/transaction/create/"), data);

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                // model n tem TaxValue 
                await _financialService.TransactionFailed(transaction);
                return BadRequest(content);
            }

            var boletoResponse = JsonConvert.DeserializeObject<BoletoResponse>(content);

            if (boletoResponse.CreateRequest.Result == "reject")
            {
                await _financialService.TransactionFailed(transaction);
                return BadRequest(boletoResponse.CreateRequest.ResponseMessage);
            }

            await _financialService.SetTransactionExtraData(transaction,
                JsonConvert.SerializeObject(boletoResponse.CreateRequest.BankSlip));

            return Ok(boletoResponse.CreateRequest.BankSlip);
            */
            return Ok();
        }


        [HttpPost("notification"), ApiExplorerSettings(IgnoreApi = true), AllowAnonymous]
        public async Task<IActionResult> Notification([FromForm] ChargeNotification notification)
        {

            //todo

            return Ok();
        }
    }
}