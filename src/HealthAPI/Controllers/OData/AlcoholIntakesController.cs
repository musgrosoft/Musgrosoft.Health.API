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
    //[Route("api/[controller]")]
    public class AlcoholIntakesController : ODataController
    {
        private readonly HealthContext _context;

        public AlcoholIntakesController(HealthContext context)
        {
            _context = context;
        }

        // GET api/Units
        [HttpGet]
        [EnableQuery(AllowedQueryOptions = Microsoft.AspNet.OData.Query.AllowedQueryOptions.All)]
        public IEnumerable<AlcoholIntake> Get()
        {
            return _context.AlcoholIntakes.AsQueryable();
        }


        [HttpGet]
        [Route("odata/AlcoholIntakes/GroupByWeek")]
        [EnableQuery(AllowedQueryOptions = Microsoft.AspNet.OData.Query.AllowedQueryOptions.All)]
        public IEnumerable<AlcoholIntake> GetByWeek()
        {
            var dailyAlcoholIntakes = _context.AlcoholIntakes;

            var weekGroups = dailyAlcoholIntakes.GroupBy(x => x.DateTime.HealthWeek());

            var weeklyAlcoholIntakes = new List<AlcoholIntake>();
            foreach (var group in weekGroups)
            {
                var alcoholIntake = new AlcoholIntake
                {
                    DateTime = group.Key,
                    Units = group.Sum(x => x.Units)
                };

                weeklyAlcoholIntakes.Add(alcoholIntake);
            }

            return weeklyAlcoholIntakes.AsQueryable();
        }

    }
}
