using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XSpy.Controllers.Base;
using XSpy.Database.Services.Financial;
using XSpy.Utils;

namespace XSpy.Controllers.Financial
{
    public class FinancialController : BaseController
    {
        private FinancialService _financialService;

        [Route("orders"), PreExecution()]
        public IActionResult Orders()
        {
            return View();
        }

        [Route("orders/{id}"), PreExecution(Role = "IS_SCHOOL_FINANCIAL_ADMIN")]
        public async Task<IActionResult> Details(Guid id)
        {
            var transaction = await _financialService.GetById(id);
            if (transaction == null)
            {
                return RedirectToAction("Orders", "Financial");
            }



            return View(new InvoiceDataViewModel()
            {
                Transaction = transaction,
                School = school,
                VoucherDiscount = _financialService.CalculateVoucher(transaction)
            });
        }


        [Route("purchase"), PreExecution()]
        public async Task<IActionResult> PlanPurchase()
        {
            return View();
        }
    }
}