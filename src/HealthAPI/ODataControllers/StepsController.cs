﻿using System;
using System.Collections.Generic;
using System.Linq;
using HealthAPI.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace HealthAPI.Controllers
{
    //[Route("api/[controller]")]
    public class StepsController : ODataController
    {
        private readonly HealthContext _context;

        public StepsController(HealthContext context)
        {
            _context = context;
        }

        // GET api/Stepsx
        [HttpGet]
        [EnableQuery(AllowedQueryOptions = Microsoft.AspNet.OData.Query.AllowedQueryOptions.All)]
        public IEnumerable<StepCount> Get(string groupBy = "day")
        {
            var dailyStepCounts = _context.StepCounts.OrderBy(x => x.DateTime);

            if (groupBy.ToLower() == "week")
            {
                var weekGroups = dailyStepCounts.GroupBy(x => x.DateTime.AddDays(-(int)x.DateTime.DayOfWeek));


                var weeklyStepCounts = new List<StepCount>();
                foreach (var group in weekGroups)
                {
                    var stepCount = new StepCount
                    {
                        DateTime = group.Key,
                        Count = group.Sum(x => x.Count)
                    };

                    weeklyStepCounts.Add(stepCount);
                }

                return weeklyStepCounts;
            }

            return dailyStepCounts;

            // return _context.DailySteps.OrderBy(x=>x.DateTime);

            //------------------------------------------------------------------------------------------

            //var dailyStepCounts = GetDailyStepCounts();

            //var weekGroups = dailyStepCounts.GroupBy(x => x.DateTime.AddDays(-(int)x.DateTime.DayOfWeek));


            //var weeklyStepCounts = new List<StepCount>();
            //foreach (var group in weekGroups)
            //{
            //    var stepCount = new StepCount
            //    {
            //        DateTime = group.Key,
            //        Steps = group.Sum(x => x.Steps)
            //    };

            //    weeklyStepCounts.Add(stepCount);
            //}

            //return weeklyStepCounts;


        }


        [HttpPost]
        [Route("api/Steps")]
        public IActionResult Create([FromBody] Models.StepCount stepCount)
        {
            try
            {
                if (stepCount == null)
                {
                    return BadRequest();
                }

                var existingItem = _context.StepCounts.FirstOrDefault(x => x.DateTime == stepCount.DateTime);

                if (existingItem != null)
                {
                    existingItem.DateTime = stepCount.DateTime;
                    existingItem.Count = stepCount.Count;

                    _context.StepCounts.Update(existingItem);
                }
                else
                {
                    _context.StepCounts.Add(stepCount);
                }


                _context.SaveChanges();

                //return CreatedAtRoute("GetTodo", weight);
                return Created("/bum", stepCount);
                //return new NoContentResult();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

    }
}
