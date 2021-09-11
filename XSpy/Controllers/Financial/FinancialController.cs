using System;
using System.Threading.Tasks;
using CFCEad.Shared.Models.Views.Financial;
using Microsoft.AspNetCore.Mvc;
using XSpy.Controllers.Base;
using XSpy.Database.Services.Financial;
using XSpy.Shared.Models.Views.Financial.Plans;
using XSpy.Utils;

namespace XSpy.Controllers.Financial
{
    public class FinancialController : BaseController
    {
        private FinancialService _financialService;

        public FinancialController(FinancialService financialService)
        {
            _financialService = financialService;
        }

        [Route("orders"), PreExecution()]
        public IActionResult Orders()
        {
            return View();
        }

        [Route("orders/{id}"), PreExecution()]
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
                VoucherDiscount = 0
            });
        }


        [Route("purchase"), PreExecution()]
        public async Task<IActionResult> PlanPurchase()
        {
            var planList = await _financialService.GetPlans();
            return View(new PlanPurchaseViewModel
            {
                PlanList = planList
            });
        }
    }
}