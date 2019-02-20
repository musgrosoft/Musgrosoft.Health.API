﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Health.Models;
using Utils;

namespace Fitbit.Services
{
    public class FitbitService : IFitbitService
    {
        private readonly ILogger _logger;

        private const int FITBIT_HOURLY_RATE_LIMIT = 150;

        private readonly IFitbitClientQueryAdapter _fitbitClientQueryAdapter;
        private readonly IFitbitClient _fitbitClient;
        private readonly IFitbitAuthenticator _fitbitAuthenticator;
        private readonly IFitbitMapper _fitbitMapper;

        public FitbitService(ILogger logger, IFitbitClientQueryAdapter fitbitClientQueryAdapter, IFitbitClient fitbitClient, IFitbitAuthenticator fitbitAuthenticator, IFitbitMapper fitbitMapper)
        {
            _logger = logger;
            _fitbitClientQueryAdapter = fitbitClientQueryAdapter;
            _fitbitClient = fitbitClient;
            _fitbitAuthenticator = fitbitAuthenticator;
            _fitbitMapper = fitbitMapper;
        }
        

        
        public async Task<IEnumerable<RestingHeartRate>> GetRestingHeartRates(DateTime fromDate, DateTime toDate)
        {
            var heartActivies = await _fitbitClientQueryAdapter.GetFitbitHeartActivities(fromDate, toDate);

            return _fitbitMapper.MapActivitiesHeartsToRestingHeartRates(heartActivies);
        }

        
        public async Task Subscribe()
        {
            await _fitbitClient.Subscribe();
        }

        public async Task SetTokens(string code)
        {
            await _fitbitAuthenticator.SetTokens(code);
        }



    }
}