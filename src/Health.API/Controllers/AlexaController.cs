﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Services.Health;

namespace HealthAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Alexa")]
    public class AlexaController : Controller
    {
        private readonly IHealthService _healthService;

        public AlexaController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        // GET
        [HttpGet]
        [Route("FlashBriefing")]
        public IActionResult FlashBriefing()
        {
            var latestWeightDate = _healthService.GetLatestWeightDate(DateTime.MinValue);
            var latestBloodPressureDate = _healthService.GetLatestBloodPressureDate(DateTime.MinValue);

            var daysOldWeight = (DateTime.Now.Date - latestWeightDate.Date).TotalDays;
            var daysOldWeightExpression = "daysOldWeightExpression";

            var daysOldBloodpressure = (DateTime.Now.Date - latestBloodPressureDate.Date).TotalDays;
            var daysOldBloodpressureExpression = "daysOldBloodpressureExpression";

            var latestWeights = _healthService.GetLatestWeights();
            var latestBloodpressures = _healthService.GetLatestBloodPressures();

            switch (daysOldWeight)
            {
                case 0:
                    daysOldWeightExpression = "today";
                    break;
                case 1:
                    daysOldWeightExpression = "yesterday";
                    break;
                default:
                    daysOldWeightExpression = $"from {daysOldWeight} days ago";
                    break;
            }

            switch (daysOldBloodpressure)
            {
                case 0:
                    daysOldBloodpressureExpression = "today";
                    break;
                case 1:
                    daysOldBloodpressureExpression = "yesterday";
                    break;
                default:
                    daysOldBloodpressureExpression = $"from {daysOldWeight} days ago";
                    break;
            }


            var flashBriefings = new List<FlashBriiefing>
                {
                new FlashBriiefing
                {
                    uid = "1_WEIGHT",
                    //updateDate = latestWeightDate.ToString("yyyy-MM-ddTHH:mm:ss0Z"), //"2019-06-10T22:34:51.0Z" ,
                    updateDate = new DateTime(2019,6,11,9,0,0).ToString("yyyy-MM-ddTHH:mm:ss0Z"), //"2019-06-10T22:34:51.0Z" ,
                    titleText = "Latest Weight",
                    mainText = $"Your weight from {daysOldWeightExpression} is {latestWeights.Average(x=>x.Kg).Value:0.#} kg. " +
                               $"Your blood pressure from {daysOldBloodpressureExpression} is {latestBloodpressures.Average(x=>x.Systolic).Value:0.#} over {latestBloodpressures.Average(x=>x.Systolic).Value:0.#}. ",
                    redirectionUrl = "https://www.amazon.com"

                },
                new FlashBriiefing
                {
                    uid = "2_BLOOD_PRESSURE",
                    //updateDate = latestBloodPressureDate.ToString("yyyy-MM-ddTHH:mm:ss0Z"), //"2019-06-10T22:34:51.0Z" ,
                    updateDate = new DateTime(2019,6,11,9,0,0).ToString("yyyy-MM-ddTHH:mm:ss0Z"), //"2019-06-10T22:34:51.0Z" ,
                    titleText = "Latest Blood Pressure",
                    mainText = $"Your blood pressure from {daysOldBloodpressureExpression} is {latestBloodpressures.Average(x=>x.Systolic)} over {latestBloodpressures.Average(x=>x.Systolic)}. ",
                    redirectionUrl = "https://www.amazon.com"

                }

                };


            //Content-Type: application/json
            return Ok(flashBriefings);
        }
    }
}