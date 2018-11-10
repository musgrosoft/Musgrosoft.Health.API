﻿using System;
using System.Collections.Generic;
using System.Linq;
using Fitbit.Domain;
using Repositories.Health.Models;

namespace Fitbit.Services
{
    public class FitbitMapper : IFitbitMapper
    {
        public IEnumerable<RestingHeartRate> MapActivitiesHeartsToRestingHeartRates(IEnumerable<ActivitiesHeart> activitiesHearts)
        {
            return activitiesHearts
                .Where(a => a.value.restingHeartRate != 0)
                .Select(x => new RestingHeartRate
                {
                    CreatedDate = x.dateTime,
                    Beats = x.value.restingHeartRate
                });
        }

        public IEnumerable<HeartRateSummary> MapActivitiesHeartsToHeartRateSummaries(IEnumerable<ActivitiesHeart> activitiesHearts)
        {
            return activitiesHearts.Select(x => new HeartRateSummary
            {
                CreatedDate = x.dateTime,
                OutOfRangeMinutes = x.value.heartRateZones.First(y => y.name == "Out of Range").minutes,
                FatBurnMinutes = x.value.heartRateZones.First(y => y.name == "Fat Burn").minutes,
                CardioMinutes = x.value.heartRateZones.First(y => y.name == "Cardio").minutes,
                PeakMinutes = x.value.heartRateZones.First(y => y.name == "Peak").minutes,
                Source = "Fitbit"
            });
        }

        public IEnumerable<ActivitySummary> MapFitbitDailyActivitiesToActivitySummaries(IEnumerable<FitbitDailyActivity> fitbitDailyActivities)
        {
            return fitbitDailyActivities.Select(x => new ActivitySummary
            {
                CreatedDate = x.DateTime,
                FairlyActiveMinutes = x.summary.fairlyActiveMinutes,
                LightlyActiveMinutes = x.summary.lightlyActiveMinutes,
                SedentaryMinutes = x.summary.sedentaryMinutes,
                VeryActiveMinutes = x.summary.veryActiveMinutes
            });
        }

        public IEnumerable<StepCount> MapFitbitDailyActivitiesToStepCounts(IEnumerable<FitbitDailyActivity> fitbitDailyActivities)
        {
            return fitbitDailyActivities.Select(x => new StepCount
            {
                CreatedDate = x.DateTime,
                Count = x.summary.steps
            });
        }

        public IEnumerable<Run> MapFitbitDailyActivitiesToRuns(IEnumerable<FitbitDailyActivity> fitbitDailyActivities)
        {
            var allTheRuns = new List<Run>();

            foreach (var fitbitDailyActivity in fitbitDailyActivities)
            {
                //TimeSpan startTime;
                //filter by some indicator that its a run

                var someRuns = fitbitDailyActivity.activities.Select(y =>
                    new Run
                    {
                        //add start time
                        //CreatedDate = TimeSpan.TryParse("07:35", out startTime) ? fitbitDailyActivity.DateTime.Add(startTime) : fitbitDailyActivity.DateTime,
                        CreatedDate = fitbitDailyActivity.DateTime,
                        Time = TimeSpan.FromMilliseconds(y.duration),
                        Metres = y.distance * 1000

                    });

                allTheRuns.AddRange(someRuns);
            }

            return allTheRuns;
        }

        public IEnumerable<HeartRate> MapDataSetToDetailedHeartRates(IEnumerable<Dataset> detailedHeartRates)
        {
            var allTheDetailedHeartRates = new List<HeartRate>();

            foreach (var detailedHeartRate in detailedHeartRates)
            {
                var detail = new HeartRate
                {
                    CreatedDate = new DateTime(detailedHeartRate.theDateTime.Year, detailedHeartRate.theDateTime.Month, detailedHeartRate.theDateTime.Day, detailedHeartRate.time.Hours, detailedHeartRate.time.Minutes, detailedHeartRate.time.Seconds),
                    Bpm = detailedHeartRate.value,
                    Source = "Fitbit"
                };

                allTheDetailedHeartRates.Add(detail);
            }


            return allTheDetailedHeartRates;
        }
    }
}