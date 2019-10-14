﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Health;
using Utils;
using MoreLinq;
using Repositories;
using Repositories.Health.Models;
using GoogleSheets;

namespace Health.API.Controllers
{


    [Route("api/Grafana[controller]")]
    [ApiController]
    public class WeightController : ControllerBase
    {
        private enum TTarget
        {
            Sleeps,

            WeightKg,
            WeightKgMovingAverage,
            WeightPercentageFat,
            WeightPercentageFatMovingAverage,
            WeightFatKg,
            WeightFatKgMovingAverage,
            WeightLeanKg,
            WeightLeanKgMovingAverage,

        }

        private readonly IHealthService _healthService;
        private readonly ISheetsService _sheetsService;

        public WeightController(IHealthService healthService, ISheetsService sheetsService)
        {
            _healthService = healthService;
            _sheetsService = sheetsService;
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
                    Enum.GetNames(typeof(TTarget)).ToList()
                );
        }

        [HttpGet, HttpPost]
        [Route("query")]
        public IActionResult Query([FromBody] GrafanaRequest grafanaRequest)
        {
            var responses = new List<QueryResponse>();
            
            foreach(var target in grafanaRequest.targets)
            {
                var t = Enum.Parse(typeof(TTarget), target.target);

                var qr = GetQueryResponse((TTarget)t);

                responses.Add(qr);


            }

            if(responses.Any())
            {
                return Ok(responses);
            }
            else {
                return Ok("no matching target " + grafanaRequest.ToString());
            }


            
        }

       // private List<Weight> _orderedWeights;
        private List<Weight> GetAllWeightsAggregatedByDay()
        {
            //if (_orderedWeights == null)
            //{
                return _healthService.GetLatestWeights(20000)
                    .GroupBy(x=>x.CreatedDate.Date)
                    .Select(x=> new Weight
                    {
                        CreatedDate = x.Key,
                        Kg = x.Average(y => y.Kg),
                        FatRatioPercentage = x.Average(y=>y.FatRatioPercentage) ,
                    })
                    .OrderBy(x => x.CreatedDate).ToList();
            //}

            //return _orderedWeights;

        }

        private QueryResponse GetQueryResponse(TTarget tt)
        {
            switch (tt)
            {
                case TTarget.Sleeps:
                        return
                        new QueryResponse
                        {
                            Target = tt.ToString(),
                            Datapoints = _healthService.GetLatestSleeps(20000)
                                .OrderBy(x => x.DateOfSleep)
                                .Select(x => new double?[] { x.AsleepMinutes, x.DateOfSleep.ToUnixTimeMillisecondsFromDate() })
                                .ToList()

                        };


                case TTarget.WeightKg:

                    return new QueryResponse
                    {
                        Target = tt.ToString(),
                        Datapoints = GetAllWeightsAggregatedByDay()
                                .Select(x => new double?[] { x.Kg, x.CreatedDate.ToUnixTimeMillisecondsFromDate() })
                                .ToList()
                    };

                case TTarget.WeightKgMovingAverage:
                    return
                    new QueryResponse
                    {
                        //var averaged = mySeries.Windowed(period).Select(window => window.Average(keyValuePair => keyValuePair.Value));

                        Target = tt.ToString(),
                        Datapoints = GetAllWeightsAggregatedByDay()
                                .WindowRight(10)
                                .Select(window => new double?[] { window.Average(x => x.Kg), window.Max(x => x.CreatedDate).ToUnixTimeMillisecondsFromDate() })
                                //                                .Select( x => new double?[] { x.Kg, x.CreatedDate.ToUnixTimeMillisecondsFromDate() } )
                                .ToList()
                    };

                case TTarget.WeightPercentageFat:

                    return new QueryResponse
                    {
                        Target = tt.ToString(),
                        Datapoints = GetAllWeightsAggregatedByDay()
                            .Select(x => new double?[] { x.FatRatioPercentage, x.CreatedDate.ToUnixTimeMillisecondsFromDate() })
                            .ToList()
                    };

                case TTarget.WeightPercentageFatMovingAverage:
                    return
                        new QueryResponse
                        {
                            //var averaged = mySeries.Windowed(period).Select(window => window.Average(keyValuePair => keyValuePair.Value));

                            Target = tt.ToString(),
                            Datapoints = GetAllWeightsAggregatedByDay()
                                .WindowRight(10)
                                .Select(window => new double?[] { window.Average(x => x.FatRatioPercentage), window.Max(x => x.CreatedDate).ToUnixTimeMillisecondsFromDate() })
                                //                                .Select( x => new double?[] { x.Kg, x.CreatedDate.ToUnixTimeMillisecondsFromDate() } )
                                .ToList()
                        };

                case TTarget.WeightFatKg:

                    return new QueryResponse
                    {
                        Target = tt.ToString(),
                        Datapoints = GetAllWeightsAggregatedByDay()
                            .Select(x => new double?[] { x.FatKg, x.CreatedDate.ToUnixTimeMillisecondsFromDate() })
                            .ToList()
                    };

                case TTarget.WeightFatKgMovingAverage:
                    return
                        new QueryResponse
                        {
                            //var averaged = mySeries.Windowed(period).Select(window => window.Average(keyValuePair => keyValuePair.Value));

                            Target = tt.ToString(),
                            Datapoints = GetAllWeightsAggregatedByDay()
                                .WindowRight(10)
                                .Select(window => new double?[] { window.Average(x => x.FatKg), window.Max(x => x.CreatedDate).ToUnixTimeMillisecondsFromDate() })
                                //                                .Select( x => new double?[] { x.Kg, x.CreatedDate.ToUnixTimeMillisecondsFromDate() } )
                                .ToList()
                        };

                case TTarget.WeightLeanKg:

                    return new QueryResponse
                    {
                        Target = tt.ToString(),
                        Datapoints = GetAllWeightsAggregatedByDay()
                            .Select(x => new double?[] { x.LeanKg, x.CreatedDate.ToUnixTimeMillisecondsFromDate() })
                            .ToList()
                    };

                case TTarget.WeightLeanKgMovingAverage:
                    return
                        new QueryResponse
                        {
                            //var averaged = mySeries.Windowed(period).Select(window => window.Average(keyValuePair => keyValuePair.Value));

                            Target = tt.ToString(),
                            Datapoints = GetAllWeightsAggregatedByDay()
                                .WindowRight(10)
                                .Select(window => new double?[] { window.Average(x => x.LeanKg), window.Max(x => x.CreatedDate).ToUnixTimeMillisecondsFromDate() })
                                //                                .Select( x => new double?[] { x.Kg, x.CreatedDate.ToUnixTimeMillisecondsFromDate() } )
                                .ToList()
                        };


                default:
                    return null;


            }

        }

    }


}