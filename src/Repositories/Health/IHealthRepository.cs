﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Health.Models;

namespace Repositories.Health
{
    public interface IHealthRepository
    {
        DateTime? GetLatestBloodPressureDate();
        DateTime? GetLatestWeightDate();
        DateTime? GetLatestRestingHeartRateDate();
        DateTime? GetLatestDrinkDate();
        DateTime? GetLatestSleepSummaryDate();
        DateTime? GetLatestSleepStateDate(DateTime defaultDateTime);

        Task UpsertAsync(IEnumerable<Weight> weights);
        Task UpsertAsync(IEnumerable<BloodPressure> bloodPressure);
        Task UpsertAsync(IEnumerable<RestingHeartRate> restingHeartRates);
        Task UpsertAsync(IEnumerable<Drink> drinks);
        Task UpsertAsync(IEnumerable<Exercise> exercise);
        Task UpsertAsync(IEnumerable<SleepSummary> sleepSummaries);

        //Task UpsertAsync(IEnumerable<SleepState> sleepState);
        Task UpsertAsync(IEnumerable<SleepState> sleepStates);

        List<Weight> GetLatestWeights(int num);
        List<BloodPressure> GetLatestBloodPressures(int num);
        DateTime? GetLatestExerciseDate();
        List<Exercise> GetLatestExercises(int num);
        List<Drink> GetLatestDrinks(int num);
        List<RestingHeartRate> GetLatestRestingHeartRate(int num);


        Target GetTarget(DateTime date);


        List<SleepSummary> GetLatestSleeps(int num);
        

    }
}