﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Repositories.Health;
using Repositories.Models;
using Services.Domain;
using Services.MyHealth;

namespace HealthAPI.Controllers.Migration
{
    public class TargetsController : Controller
    {
        private readonly IHealthRepository _healthRepository;
        private readonly ITargetService _targetService;

        public TargetsController(IHealthRepository healthRepository, ITargetService targetService)
        {
            _healthRepository = healthRepository;
            _targetService = targetService;
        }

        [HttpGet]
        public IActionResult ActivitySummaries()
        {
            var targets = new List<ActivitySummary>();

            var targetStartDate = new DateTime(2017, 5, 2);
            var targetEndDate = DateTime.Now.AddDays(100);
            var totalDays = (targetEndDate - targetStartDate).TotalDays;

            var activeMinutesOnTargetStartDate = 0;
            var targetDailyActiveMinutes = 30;

            for (var i = 0; i <= totalDays; i++)
            {
                var target = new ActivitySummary
                {
                    DateTime = targetStartDate.AddDays(i),
                    CumSumActiveMinutes = (int)(activeMinutesOnTargetStartDate + (i * targetDailyActiveMinutes))
                };

                targets.Add(target);
            }

            return Json(targets);
        }

        [HttpGet]
        public IActionResult AlcoholIntakes()
        {
            var targets = new List<TargetAlcoholIntake>();

            var targetStartDate = new DateTime(2018, 5, 29);
            var targetEndDate = DateTime.Now.AddDays(100);
            var totalDays = (targetEndDate - targetStartDate).TotalDays;

            var unitsOnTargetStartDate = 5148;
            var targetDailyUnits = 4;

            var allAlcoholIntakes = _healthRepository.GetAllAlcoholIntakes();

            for (var i = 0; i <= totalDays; i++)
            {
                var actualAlcoholIntake = allAlcoholIntakes.FirstOrDefault(x => x.DateTime.Date == targetStartDate.AddDays(i).Date);

                var target = new TargetAlcoholIntake
                {
                    DateTime = targetStartDate.AddDays(i),
                    TargetCumSumUnits = (Decimal)(unitsOnTargetStartDate + (i * targetDailyUnits)),
                    ActualCumSumUnits = actualAlcoholIntake?.CumSumUnits
                };

                targets.Add(target);
            }

            return Json(targets);
        }

        [HttpGet]
        public IActionResult HeartRateSummaries()
        {
            var targets = new List<HeartRateSummary>();

            var targetStartDate = new DateTime(2018, 5, 19);
            var targetEndDate = DateTime.Now.AddDays(100);
            var totalDays = (targetEndDate - targetStartDate).TotalDays;

            var minutesOnTargetStartDate = 1775;
            var targetDailyMinutes = 11;

            for (var i = 0; i <= totalDays; i++)
            {
                var target = new HeartRateSummary
                {
                    DateTime = targetStartDate.AddDays(i),
                    CumSumCardioAndAbove = (int)(minutesOnTargetStartDate + (i * targetDailyMinutes))
                };

                targets.Add(target);
            }

            return Json(targets);
        }

        [HttpGet]
        public IActionResult Weights()
        {
            var allWeights = _healthRepository.GetAllWeights();
            var groups = allWeights.GroupBy(x => x.DateTime.Date);

            allWeights = groups.Select(x => new Weight {
                DateTime = x.Key.Date,
                Kg = x.Average(w=>w.Kg),
                MovingAverageKg = x.Average(w=>w.MovingAverageKg)
            });

            var targetWeights = allWeights.Select(x => new TargetWeight {
                DateTime = x.DateTime,
                TargetKg = _targetService.GetTargetWeight(x.DateTime),
                ActualKg = x.Kg,
                ActualMovingAverageKg = x.MovingAverageKg
            });

            //targetWeights = targetWeights.Where(x => x.TargetKg != null);

            return Json(targetWeights);
        }


        [HttpGet]
        public IActionResult StepCounts()
        {
            var targets = new List<StepCount>();

            var targetStartDate = new DateTime(2017, 5, 3);
            var targetEndDate = DateTime.Now.AddDays(100);
            var totalDays = (targetEndDate - targetStartDate).TotalDays;

            var stepsOnTargetStartDate = 0;
            var targetDailySteps = 10000;

            for (var i = 0; i <= totalDays; i++)
            {
                var target = new StepCount()
                {
                    DateTime = targetStartDate.AddDays(i),
                    CumSumCount = stepsOnTargetStartDate + (i * targetDailySteps)
                };

                targets.Add(target);
            }

            return Json(targets);
        }
    }
}