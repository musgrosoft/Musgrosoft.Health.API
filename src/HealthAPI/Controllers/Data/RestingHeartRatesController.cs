﻿using Microsoft.AspNetCore.Mvc;
using Services.MyHealth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthAPI.Controllers.Data
{
    [Produces("application/json")]
    [Route("api/RestingHeartRates")]

    public class RestingHeartRatesController : Controller
    {
        private readonly IHealthService _healthService;

        public RestingHeartRatesController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(_healthService.GetAllRestingHeartRates());
        }


    }
}