﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Services.Fitbit;
using Utils;
using Xunit;

namespace Services.Tests.Fitbit
{
    public class FitbitClientTests
    {

        
        private Mock<HttpMessageHandler> _httpMessageHandler;
        private Mock<IConfig> _config;
        private Mock<ILogger> _logger;
        private HttpClient _httpClient;
        private FitbitClient _fitbitClient;

        private Mock<IFitbitAuthenticator> _fitbitAuthenticator;
       // private string _accessToken;

        public FitbitClientTests()
        {
            _httpMessageHandler = new Mock<HttpMessageHandler>();


            _config = new Mock<IConfig>();
            _logger = new Mock<ILogger>();
            _fitbitAuthenticator = new Mock<IFitbitAuthenticator>();

            _httpClient = new HttpClient(_httpMessageHandler.Object);

            _fitbitClient = new FitbitClient(_httpClient, _config.Object, _fitbitAuthenticator.Object, _logger.Object);
        }

        [Fact]
        public async Task ShouldGetFitbitDailyActivity()
        {
            Uri _capturedUri = new Uri("http://www.null.com");
            _httpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fitbitDailyActivityContent)
                })).Callback<HttpRequestMessage, CancellationToken>((h, c) => _capturedUri = h.RequestUri); ;

         //   var fitbitDailyActivity = await _fitbitClient.GetFitbitDailyActivity(new DateTime());

            //Assert.Equal("", _capturedUri.AbsoluteUri);

            //Assert.Equal(123, activity.activities.Count);


        }


        [Fact]
        public async Task ShouldGetFitbitMonthlyActivity()
        {
            Uri _capturedUri = new Uri("http://www.null.com");
            _httpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fitbitMonthlyActivityContent)
                })).Callback<HttpRequestMessage, CancellationToken>((h, c) => _capturedUri = h.RequestUri); ;

            var fitbitActivity = await _fitbitClient.GetMonthOfFitbitActivities(new DateTime());

            //Assert.Equal("", _capturedUri.AbsoluteUri);

            //Assert.Equal(123, activity.activities.Count);


        }

        private readonly string fitbitDailyActivityContent = "";

        private readonly string fitbitMonthlyActivityContent = "";

    }
}
