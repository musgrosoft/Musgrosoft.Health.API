﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Importer.Fitbit.Domain;
using Newtonsoft.Json;
using Utils;

namespace Importer.Fitbit
{
    public class FitbitClient : IFitbitClient
    {
        private const string FITBIT_BASE_URL = "https://api.fitbit.com";

        private readonly HttpClient _httpClient;
        private readonly IConfig _config;

        //private readonly string _accessToken;
        private readonly ILogger _logger;

        public FitbitClient(HttpClient httpClient, IConfig config, ILogger logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;

        }

        //POST https://api.fitbit.com/1/user/-/apiSubscriptions/123.json

        public async Task Subscribe(string accessToken)
        {
            var uri = FITBIT_BASE_URL + $"/1/user/{_config.FitbitUserId}/apiSubscriptions/123.json";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var response = await _httpClient.PostAsync(uri,null);

            await _logger.LogMessageAsync("Status code ::: " + response.StatusCode);
            await _logger.LogMessageAsync("content ::: " + response.Content);

        }



        public async Task<FitBitActivity> GetMonthOfFitbitActivities(DateTime startDate, string accessToken)
        {
            //var accessToken = await _fitbitAuthenticator.GetAccessToken();

            var uri = FITBIT_BASE_URL + $"/1/user/{_config.FitbitUserId}/activities/heart/date/{startDate:yyyy-MM-dd}/1m.json";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var response = await _httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<FitBitActivity>(content);
                return data;
            }
            else if (response.StatusCode == (HttpStatusCode) 429)
            {
                throw new TooManyRequestsException(
                    $"Too many requests made to Fitbit API. Failed call to fitbit api {uri} , status code is {response.StatusCode} , and content is {response.Content}");
            }
            else
            {
                throw new Exception(
                    $"Failed call to fitbit api {uri} , status code is {(int)response.StatusCode} , and content is {await response.Content.ReadAsStringAsync()}");
//                _logger.Log($"Failed call to fitbit api {uri} , status code is {response.StatusCode} , and content is {response.Content}");
//                return Maybe<FitBitActivity>.None;
            }

        }

        //public async Task<FitbitDailyActivity> GetFitbitDailyActivity(DateTime date)
        //{
        //    var accessToken = await _fitbitAuthenticator.GetAccessToken();


        //    var uri = FITBIT_BASE_URL + $"/1/user/{_config.FitbitUserId}/activities/date/{date:yyyy-MM-dd}.json";
        //    _httpClient.DefaultRequestHeaders.Clear();
        //    _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        //    var response = await _httpClient.GetAsync(uri);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var content = await response.Content.ReadAsStringAsync();

        //        var data = JsonConvert.DeserializeObject<FitbitDailyActivity>(content);
        //        data.DateTime = date;
        //        return data;
        //    }
        //    else if (response.StatusCode == (HttpStatusCode) 429)
        //    {
        //        throw new TooManyRequestsException(
        //            $"Too many requests made to Fitbit API. Failed call to fitbit api {uri} , status code is {response.StatusCode} , and content is {response.Content}");
        //    }
        //    else
        //    {

        //        throw new Exception(
        //            $"Failed call to fitbit api {uri} , status code is {response.StatusCode} , and content is {response.Content.ReadAsStringAsync()}");
        //        //_logger.Log($"No FitbitDailyActivity found for date : {date}");
        //        //return null;
        //    }
        //}






        //public async Task<List<Dataset>> GetDetailedHeartRates(DateTime date)
        //{
        //    var accessToken = await _fitbitAuthenticator.GetAccessToken();

        //    var uri = FITBIT_BASE_URL + $"/1/user/{_config.FitbitUserId}/activities/heart/date/{date:yyyy-MM-dd}/1d/1min.json";

        //    _httpClient.DefaultRequestHeaders.Clear();
        //    _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        //    var response = await _httpClient.GetAsync(uri);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var content = await response.Content.ReadAsStringAsync();
        //        var data = JsonConvert.DeserializeObject<FitBitActivity>(content);
        //        //foreach (var dataset in data.activitiesHeartIntraday.dataset)
        //        //{
        //        //    dataset.time = new DateTime(date.Year, date.Month, date.Day, (int)dataset.time.TotalHours, (int)dataset.time.TotalMinutes, (int)dataset.time.TotalSeconds);
        //        //}

        //        foreach (var dataset in data.activitiesHeartIntraday.dataset)
        //        {
        //            dataset.theDateTime = date;
        //        }


        //        return data.activitiesHeartIntraday.dataset;
        //    }
        //    else if (response.StatusCode == (HttpStatusCode)429)
        //    {
        //        throw new TooManyRequestsException(
        //            $"Too many requests made to Fitbit API. Failed call to fitbit api {uri} , status code is {response.StatusCode} , and content is {response.Content}");
        //    }
        //    else
        //    {

        //        throw new Exception(
        //            $"Failed call to fitbit api {uri} , status code is {response.StatusCode} , and content is {response.Content}");
        //        //_logger.Log($"No FitbitDailyActivity found for date : {date}");
        //        //return null;
        //    }

        //}

        public async Task<FitbitRefreshTokenResponse> GetTokensWithRefreshToken(string refreshToken)
        {
            var uri = $"{_config.FitbitBaseUrl}/oauth2/token";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Base64Encode($"{_config.FitbitClientId}:{_config.FitbitClientSecret}"));

            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            };

            var response = await _httpClient.PostAsync(uri, new FormUrlEncodedContent(nvc));

            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<FitbitRefreshTokenResponse>(responseBody);
            }
            else
            {
                throw new Exception($"Fitbit GetTokensWithRefreshToken FAIL  non success status code is : {(int)response.StatusCode} , content: {responseBody}");
            }



        }

        public async Task<FitbitAuthTokensResponse> GetTokensWithAuthorizationCode(string authorizationCode)
        {
            var url = $"{_config.FitbitBaseUrl}/oauth2/token";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Base64Encode($"{_config.FitbitClientId}:{_config.FitbitClientSecret}"));

            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", _config.FitbitClientId),
                new KeyValuePair<string, string>("code", authorizationCode),
                new KeyValuePair<string, string>("redirect_uri", "http://musgrosoft-health-api.azurewebsites.net/api/fitbit/oauth/"),
            };

            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(nvc));

            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                await _logger.LogMessageAsync($"Fitbit SetTokens SUCCESS status code : {response.StatusCode} , content: {responseBody}");
                var tokenResponse = JsonConvert.DeserializeObject<FitbitAuthTokensResponse>(responseBody);

                return tokenResponse;
            }
            else
            {
                throw new Exception($"Fitbit GetTokensWithAuthorizationCode FAIL  non success status code is : {(int)response.StatusCode} , content: {responseBody}");
            }

        }


        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }
}
