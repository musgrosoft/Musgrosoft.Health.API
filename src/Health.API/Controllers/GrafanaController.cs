﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Health;
using Utils;

namespace Health.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrafanaController : ControllerBase
    {
        private readonly IHealthService _healthService;

        public GrafanaController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet]
        
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet, HttpPost]
        [Route("search")]
        public IActionResult Search()
        {
            return Ok(
                new List<string>
                {
                    "sleeps",
                    "weights"
                }
                );
        }

        [HttpGet, HttpPost]
        [Route("query")]
        public IActionResult Query([FromBody] GrafanaRequest grafanaRequest)
        {

            if (grafanaRequest.targets[0].target == "sleeps")
            {
                return Ok(
                    new List<QueryResponse>
                    {
                        new QueryResponse
                        {
                            Target = "sleeps",
                            Datapoints = _healthService.GetLatestSleeps(20000)
                                .OrderBy( x => x.DateOfSleep )
                                .Select( x => new double?[] { x.AsleepMinutes, x.DateOfSleep.ToUnixTimeMillisecondsFromDate() } )
                                .ToList()

                        },


                    }
                );

            }

            if (grafanaRequest.targets[0].target == "weights")
            {
                return Ok(
                    new List<QueryResponse>
                    {
                        new QueryResponse
                        {
                            Target = "weights",
                            Datapoints = _healthService.GetLatestWeights(20000)
                                .OrderBy( x => x.CreatedDate )
                                .Select( x => new double?[] { x.Kg, x.CreatedDate.ToUnixTimeMillisecondsFromDate() } )
                                .ToList()
                        }

                    }
                );

            }

            return Ok("no matching target " + grafanaRequest.ToString());

        }

    }

    public class QueryResponse
    {
        public string Target { get; set; }
        public List<double?[]> Datapoints { get; set; }
    }

//    public class Datapoint
//    {
//        public double Value { get; set; }
//        public double UnixTimestamp { get; set; }
//    }

}