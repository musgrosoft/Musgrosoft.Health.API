﻿using Repositories.Health;
using Repositories.Models;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.MyHealth
{
    public class TargetService : ITargetService
    {
        private IHealthRepository _healthRepository;

        public TargetService(IHealthRepository healthRepository) {
            _healthRepository = healthRepository;
        }

        public List<TargetWeight> GetTargetWeights()
        {

            var targetStartDate = new DateTime(2018, 5, 1);
            var targetEndDate = DateTime.Now.AddDays(600);
            var totalDays = (targetEndDate - targetStartDate).TotalDays;


            var allWeights = _healthRepository.GetWeightsFromDate(targetStartDate);
            
            var groups = allWeights.GroupBy(x => x.CreatedDate.Date);

            allWeights = groups.Select(x => new Weight
            {
                CreatedDate = x.Key.Date,
                Kg = x.Average(w => w.Kg),
                MovingAverageKg = x.Average(w => w.MovingAverageKg)
            }).ToList();

            var targetWeights = allWeights.Select(x => new TargetWeight
            {
                DateTime = x.CreatedDate,
                TargetKg = GetTargetWeight(x.CreatedDate),
                ActualKg = x.Kg,
                ActualMovingAverageKg = x.MovingAverageKg
            }).ToList();

            var futuredays = (targetEndDate - targetWeights.Min(x=>x.DateTime)).TotalDays;

            for (int i = 0; i < futuredays; i++)
            {
                var target = new TargetWeight
                {
                    DateTime = DateTime.Now.AddDays(i),
                    TargetKg = GetTargetWeight(DateTime.Now.AddDays(i))
                };

                targetWeights.Add(target);
            }

            targetWeights = targetWeights.OrderBy(x => x.DateTime).ToList();

            return targetWeights;
        }

        public IList<Weight> SetTargetWeights(IList<Weight> weights, int extraFutureDays)
        {
            var targetStartDate = new DateTime(2018, 5, 1);
            var targetEndDate = DateTime.Now.AddDays(600);
            var totalDays = (targetEndDate - targetStartDate).TotalDays;


            //var allWeights = _healthRepository.GetWeightsFromDate(targetStartDate);

            var groups = weights.GroupBy(x => x.CreatedDate.Date);

            weights = groups.Select(x => new Weight
            {
                CreatedDate = x.Key.Date,
                Kg = x.Average(w => w.Kg),
                MovingAverageKg = x.Average(w => w.MovingAverageKg),
                TargetKg = GetTargetWeight(x.Key.Date),
            }).ToList();

            //var targetWeights = allWeights.Select(x => new TargetWeight
            //{
            //    DateTime = x.DateTime,
            //    TargetKg = GetTargetWeight(x.DateTime),
            //    ActualKg = x.Kg,
            //    ActualMovingAverageKg = x.MovingAverageKg
            //}).ToList();

            var futuredays = (targetEndDate - weights.Min(x => x.CreatedDate)).TotalDays;

            for (int i = 0; i < futuredays; i++)
            {
                var target = new Weight
                {
                    CreatedDate = DateTime.Now.AddDays(i),
                    TargetKg = GetTargetWeight(DateTime.Now.AddDays(i))
                };

                weights.Add(target);
            }

            weights = weights.OrderBy(x => x.CreatedDate).ToList();

            return weights;
        }


        public decimal? GetTargetWeight(DateTime dateTime)
        {
            var targets = new List<TargetWeight>();

            var targetStartDate = new DateTime(2018, 5, 1);
            var targetEndDate = DateTime.Now.AddDays(600);
            var totalDays = (targetEndDate - targetStartDate).TotalDays;

            var weightOnTargetStartDate = 90.74;
            var targetDailyWeightLoss = 0.5 / 30;
            var targetDailyWeightLoss2 = 0.25 / 30;

            var daysToHitHealthyWeight = 123;

            var daysDiff = (dateTime - targetStartDate).TotalDays;

            if (daysDiff < 0)
            {
                return null;
            }

            if (daysDiff <= daysToHitHealthyWeight)
            {
                return (decimal)(weightOnTargetStartDate - (daysDiff * targetDailyWeightLoss));
            }

            if (daysDiff <= totalDays)
            {
                return (decimal)((weightOnTargetStartDate - (daysToHitHealthyWeight * targetDailyWeightLoss + (daysDiff - daysToHitHealthyWeight) * targetDailyWeightLoss2)));
            }

            return null;


        }


    }
}
