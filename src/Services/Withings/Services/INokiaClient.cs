﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Withings.Domain;

namespace Services.Withings.Services
{
    public interface INokiaClient
    {
        //Task<IEnumerable<Response.Measuregrp>> GetBloodPressuresMeasureGroups(DateTime sinceDateTime);
        //Task<IEnumerable<Response.Measuregrp>> GetWeightMeasureGroups(DateTime sinceDateTime);
        Task Subscribe();
        //Task<string> GetSubscriptions();
        Task<IEnumerable<Response.Measuregrp>> GetMeasureGroups();
        Task<string> GetWeightSubscription();
        Task<string> GetBloodPressureSubscription();
    }
}