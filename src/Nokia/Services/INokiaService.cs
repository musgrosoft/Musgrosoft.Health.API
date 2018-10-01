﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Models;

namespace Nokia.Services
{
    public interface INokiaService
    {
        Task<IEnumerable<Weight>> GetWeights(DateTime sinceDateTime);
        Task<IEnumerable<BloodPressure>> GetBloodPressures(DateTime sinceDateTime);
        Task<List<string>> GetSubscriptions();
        Task Subscribe();
        Task SetTokens(string authorizationCode);
    }
}