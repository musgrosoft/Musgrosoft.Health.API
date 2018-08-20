﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Repositories.Models;

namespace HealthAPI.Acceptance.Tests
{
    public class RestingHeartRateAcceptanceTests : IClassFixture<WebApplicationFactory<HealthAPI.Startup>>
    {
        private WebApplicationFactory<Startup> _factory;

        public RestingHeartRateAcceptanceTests(WebApplicationFactory<HealthAPI.Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder => builder.UseStartup<TestStartup>());
        }

        [Fact]
        public async Task ShouldGetRestingHeartRates()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/RestingHeartRates");

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<RestingHeartRate>>(content);

            //  Assert.Equal(960, data.Count);
            Assert.Contains(data, x => x.CreatedDate == new DateTime(2018, 1, 1) && x.Beats == 101);
            Assert.Contains(data, x => x.CreatedDate == new DateTime(2018, 1, 2) && x.Beats == 102);
            Assert.Contains(data, x => x.CreatedDate == new DateTime(2018, 1, 3) && x.Beats == 103);
        }
    }
}
