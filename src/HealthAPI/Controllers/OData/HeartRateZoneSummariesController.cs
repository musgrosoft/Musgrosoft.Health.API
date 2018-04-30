﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Models;

namespace HealthAPI.Controllers.OData
{
    //[Route("api/[controller]")]
    public class HeartRateZoneSummariesController : ODataController
    {
        private readonly HealthContext _context;

        public HeartRateZoneSummariesController(HealthContext context)
        {
            _context = context;
        }

        // GET api/HeartRateDailySummaries
        [HttpGet]
        [EnableQuery(AllowedQueryOptions = Microsoft.AspNet.OData.Query.AllowedQueryOptions.All)]
        public IEnumerable<HeartRateZoneSummary> Get()
        {
            return _context.HeartRateDailySummaries.OrderBy(x=>x.DateTime);
        }

        // GET api/HeartRateDailySummaries
        [HttpGet]
        [Route("api/HeartRateDailySummaries/GroupByWeek")]
        [EnableQuery(AllowedQueryOptions = Microsoft.AspNet.OData.Query.AllowedQueryOptions.All)]
        public IEnumerable<HeartRateZoneSummary> GetByWeek()
        {
            return _context.HeartRateDailySummaries.OrderBy(x => x.DateTime);
        }

        [HttpPost]
        [Route("api/HeartRateDailySummaries")]
        public IActionResult Create([FromBody] HeartRateZoneSummary heartRateDailySummaries)
        {
            try
            {
                if (heartRateDailySummaries == null)
                {
                    return BadRequest();
                }

                var existingItem = _context.HeartRateDailySummaries.FirstOrDefault(x => x.DateTime == heartRateDailySummaries.DateTime);

                if (existingItem != null)
                {
                    existingItem.DateTime = heartRateDailySummaries.DateTime;
                    existingItem.RestingHeartRate = heartRateDailySummaries.RestingHeartRate;
                    existingItem.OutOfRangeMinutes = heartRateDailySummaries.OutOfRangeMinutes;
                    existingItem.FatBurnMinutes = heartRateDailySummaries.FatBurnMinutes;
                    existingItem.CardioMinutes = heartRateDailySummaries.CardioMinutes;
                    existingItem.PeakMinutes = heartRateDailySummaries.PeakMinutes;
                    

                    _context.HeartRateDailySummaries.Update(existingItem);
                }
                else
                {
                    _context.HeartRateDailySummaries.Add(heartRateDailySummaries);
                }


                _context.SaveChanges();

                //return CreatedAtRoute("GetTodo", weight);
                return Created("/bum", heartRateDailySummaries);
                //return new NoContentResult();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

    }
}
