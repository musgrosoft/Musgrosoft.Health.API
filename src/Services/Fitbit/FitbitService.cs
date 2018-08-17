﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Models;
using Services.Fitbit.Domain;
using Utils;

namespace Services.Fitbit
{
    public class FitbitService : IFitbitService
    {
        private readonly ILogger _logger;

        private const int FITBIT_HOURLY_RATE_LIMIT = 150;

        private IConfig _config { get; }
        private readonly IFitbitClientQueryAdapter _fitbitClientQueryAdapter;
        private readonly IFitbitClient _fitbitClient;
        private readonly IFitbitAuthenticator _fitbitAuthenticator;
        private readonly IFitbitMapper _fitbitMapper;

        public FitbitService(IConfig config, ILogger logger, IFitbitClientQueryAdapter fitbitClientQueryAdapter, IFitbitClient fitbitClient, IFitbitAuthenticator fitbitAuthenticator, IFitbitMapper fitbitMapper)
        {
            _logger = logger;
            _config = config;
            _fitbitClientQueryAdapter = fitbitClientQueryAdapter;
            _fitbitClient = fitbitClient;
            _fitbitAuthenticator = fitbitAuthenticator;
            _fitbitMapper = fitbitMapper;
        }

        public async Task<IEnumerable<StepCount>> GetStepCounts(DateTime fromDate, DateTime toDate)
        {
            var fitbitDailyActivities = await _fitbitClientQueryAdapter.GetFitbitDailyActivities(fromDate, toDate);

            return _fitbitMapper.MapToStepCounts(fitbitDailyActivities);
        }

        public async Task<IEnumerable<ActivitySummary>> GetActivitySummaries(DateTime fromDate, DateTime toDate)
        {
            var fitbitDailyActivities = await _fitbitClientQueryAdapter.GetFitbitDailyActivities(fromDate, toDate);

            return _fitbitMapper.MapFitbitDailyActivitiesToActivitySummaries(fitbitDailyActivities);
        }
        
        public async Task<IEnumerable<RestingHeartRate>> GetRestingHeartRates(DateTime fromDate, DateTime toDate)
        {
            var heartActivies = await _fitbitClientQueryAdapter.GetFitbitHeartActivities(fromDate, toDate);

            return _fitbitMapper.MapActivitiesHeartToRestingHeartRates(heartActivies);
        }

        public async Task<IEnumerable<HeartRateSummary>> GetHeartSummaries(DateTime fromDate, DateTime toDate)
        {
            var heartActivies = await _fitbitClientQueryAdapter.GetFitbitHeartActivities(fromDate, toDate);

            return _fitbitMapper.MapActivitiesHeartToHeartRateSummaries(heartActivies);
        }

        public void Subscribe()
        {
            _fitbitClient.Subscribe();
        }

        public async Task SetTokens(string code)
        {
            await _fitbitAuthenticator.SetTokens(code);
        }



    }
}