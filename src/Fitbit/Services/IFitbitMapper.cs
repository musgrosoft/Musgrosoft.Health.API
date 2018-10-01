﻿using System.Collections.Generic;
using Fitbit.Domain;
using Repositories.Models;

namespace Fitbit.Services
{
    public interface IFitbitMapper
    {
        IEnumerable<RestingHeartRate> MapActivitiesHeartsToRestingHeartRates(IEnumerable<ActivitiesHeart> activitiesHearts);
        IEnumerable<HeartRateSummary> MapActivitiesHeartsToHeartRateSummaries(IEnumerable<ActivitiesHeart> activitiesHearts);
        IEnumerable<ActivitySummary> MapFitbitDailyActivitiesToActivitySummaries(IEnumerable<FitbitDailyActivity> fitbitDailyActivities);
        IEnumerable<StepCount> MapFitbitDailyActivitiesToStepCounts(IEnumerable<FitbitDailyActivity> fitbitDailyActivities);
    }
}