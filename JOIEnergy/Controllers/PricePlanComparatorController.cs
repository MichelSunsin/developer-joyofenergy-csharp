using System.Collections.Generic;
using System.Linq;
using JOIEnergy.Enums;
using JOIEnergy.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JOIEnergy.Controllers
{
    [Route("price-plans")]
    public class PricePlanComparatorController : Controller
    {
        public const string PRICE_PLAN_ID_KEY = "pricePlanId";
        public const string PRICE_PLAN_COMPARISONS_KEY = "pricePlanComparisons";
        private readonly IPricePlanService _pricePlanService;
        private readonly IAccountService _accountService;

        public PricePlanComparatorController(IPricePlanService pricePlanService, IAccountService accountService)
        {
            _pricePlanService = pricePlanService;
            _accountService = accountService;
        }

        [HttpGet("compare-all/{smartMeterId}")]
        public IActionResult CalculatedCostForEachPricePlan(string smartMeterId)
        {
            string pricePlanId = _accountService.GetPricePlanIdForSmartMeterId(smartMeterId);
            Dictionary<string, decimal> costPerPricePlan = _pricePlanService.GetConsumptionCostOfElectricityReadingsForEachPricePlan(smartMeterId);
            
            if (costPerPricePlan.Count == 0)
            {
                return NotFound(string.Format("Smart Meter ID ({0}) not found", smartMeterId));
            }

            return Ok(new Dictionary<string, object>() {
                {PRICE_PLAN_ID_KEY, pricePlanId},
                {PRICE_PLAN_COMPARISONS_KEY, costPerPricePlan},
            });
        }

        [HttpGet("recommend/{smartMeterId}")]
        public IActionResult RecommendCheapestPricePlans(string smartMeterId, int? limit = null) {
            var consumptionForPricePlans = _pricePlanService.GetConsumptionCostOfElectricityReadingsForEachPricePlan(smartMeterId);

            if (consumptionForPricePlans.Count == 0) {
                return NotFound(string.Format("Smart Meter ID ({0}) not found", smartMeterId));
            }

            var recommendations = consumptionForPricePlans.OrderBy(pricePlanComparison => pricePlanComparison.Value);

            if (limit.HasValue && limit.Value < recommendations.Count())
            {
                return Ok(recommendations.Take(limit.Value));
            }

            return Ok(recommendations);
        }
    }
}
