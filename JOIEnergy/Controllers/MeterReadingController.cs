using System.Collections.Generic;
using JOIEnergy.Domain;
using JOIEnergy.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JOIEnergy.Controllers
{
    [Route("readings")]
    public class MeterReadingController : Controller
    {
        private readonly IMeterReadingService _meterReadingService;

        public MeterReadingController(IMeterReadingService meterReadingService)
        {
            _meterReadingService = meterReadingService;
        }

        // POST api/values
        [HttpPost ("store")]
        public IActionResult Post([FromBody]MeterReadings meterReadings)
        {
            if (!IsMeterReadingsValid(meterReadings)) {
                return BadRequest("Internal Server Error");
            }

            _meterReadingService.StoreReadings(meterReadings.SmartMeterId,meterReadings.ElectricityReadings);
            return Ok();
        }

        private static bool IsMeterReadingsValid(MeterReadings meterReadings)
        {
            string smartMeterId = meterReadings.SmartMeterId;
            List<ElectricityReading> electricityReadings = meterReadings.ElectricityReadings;
            return smartMeterId != null && smartMeterId.Length != 0
                    && electricityReadings != null && electricityReadings.Count != 0;
        }

        [HttpGet("read/{smartMeterId}")]
        public IActionResult GetReading(string smartMeterId) {
            return Ok(_meterReadingService.GetReadings(smartMeterId));
        }
    }
}
