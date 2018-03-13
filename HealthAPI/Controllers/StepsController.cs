﻿using System.Collections.Generic;
using System.Linq;
using HealthAPI.Models;
using HealthAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HealthAPI.Controllers
{
  //  [Route("api/[controller]")]
    public class StepsController : Controller
    {
        private readonly HealthContext _context;

        public StepsController(HealthContext context)
        {
            _context = context;
        }

        // GET api/bloodpressures
        [HttpGet]
        [Route("api/stepsx")]
        // public IEnumerable<DailySteps> Get([FromUri] string groupBy)
        public IEnumerable<StepCount> Get(string groupBy = "day")
        {
            var dailyStepCounts = _context.DailySteps.OrderBy(x => x.DateTime).Select(x=>new StepCount
            {
                DateTime = x.DateTime,
                Steps = x.Steps
            });

            if (groupBy.ToLower() == "week")
            {
                var weekGroups = dailyStepCounts.GroupBy(x => x.DateTime.AddDays(-(int)x.DateTime.DayOfWeek));


                var weeklyStepCounts = new List<StepCount>();
                foreach (var group in weekGroups)
                {
                    var stepCount = new StepCount
                    {
                        DateTime = group.Key,
                        Steps = group.Sum(x => x.Steps)
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

        
    }
}
