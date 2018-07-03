﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Models;
using Utils;

namespace HealthAPI.Controllers.OData
{
//    [Route("api/[controller]")]
    public class ActivitySummariesController : ODataController
    {
        private readonly HealthContext _context;

        public ActivitySummariesController(HealthContext context)
        {
            _context = context;
        }

        // GET api/ActivitySummaries
        [HttpGet]
        [EnableQuery(AllowedQueryOptions = Microsoft.AspNet.OData.Query.AllowedQueryOptions.All)]
        public IEnumerable<ActivitySummary> Get()
        {
            return _context.ActivitySummaries.AsQueryable();
        }


        [HttpGet]
        [Route("odata/ActivitySummaries/GroupByWeek")]
        [EnableQuery(AllowedQueryOptions = Microsoft.AspNet.OData.Query.AllowedQueryOptions.All)]
        public IEnumerable<ActivitySummary> GetByWeek()
        {
            var dailyActivities = _context.ActivitySummaries.OrderBy(x => x.CreatedDate).ToList();

            var weekGroups = dailyActivities.GroupBy(x => x.CreatedDate.GetWeekStartingOnMonday());


            var weeklyActivities = new List<ActivitySummary>();
            foreach (var group in weekGroups)
            {
                var activity = new ActivitySummary
                {
                    CreatedDate = group.Key,
                    SedentaryMinutes = group.Sum(x => x.SedentaryMinutes),
                    LightlyActiveMinutes = group.Sum(x => x.LightlyActiveMinutes),
                    FairlyActiveMinutes = group.Sum(x => x.FairlyActiveMinutes),
                    VeryActiveMinutes = group.Sum(x => x.VeryActiveMinutes)
                };

                weeklyActivities.Add(activity);
            }

            return weeklyActivities.AsQueryable();
        }

        [HttpGet]
        [Route("odata/ActivitySummaries/GroupByMonth")]
        [EnableQuery(AllowedQueryOptions = Microsoft.AspNet.OData.Query.AllowedQueryOptions.All)]
        public IEnumerable<ActivitySummary> GetByMonth()
        {
            var dailyActivities = _context.ActivitySummaries.OrderBy(x => x.CreatedDate).ToList();

            var monthGroups = dailyActivities.GroupBy(x => x.CreatedDate.GetFirstDayOfMonth());


            var monthlyActivities = new List<ActivitySummary>();
            foreach (var group in monthGroups)
            {
                var activity = new ActivitySummary
                {
                    CreatedDate = group.Key,
                    SedentaryMinutes = (int)group.Average(x => x.SedentaryMinutes),
                    LightlyActiveMinutes = (int)group.Average(x => x.LightlyActiveMinutes),
                    FairlyActiveMinutes = (int)group.Average(x => x.FairlyActiveMinutes),
                    VeryActiveMinutes = (int)group.Average(x => x.VeryActiveMinutes)
                };

                monthlyActivities.Add(activity);
            }

            return monthlyActivities.AsQueryable();
        }



    }
}
