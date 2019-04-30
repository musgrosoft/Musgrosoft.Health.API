﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Importer.Withings.Domain;
using Newtonsoft.Json;
using Utils;

namespace Importer.Withings
{
    public class WithingsClient : IWithingsClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly IConfig _config;
        private const int WeightKgMeasureTypeId = 1;
        private const int SubscribeBloodPressureId = 4;

        private const string NOKIA_BASE_URL = "https://wbsapi.withings.net";
        private const string NOKIA_REDIRECT_URL = "https://musgrosoft-health-api.azurewebsites.net/api/nokia/oauth/";


        public WithingsClient(HttpClient httpClient, ILogger logger, IConfig config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = config;
        }

        public async Task<WithingsTokenResponse> GetTokensByAuthorisationCode(string authorizationCode)
        {

                var url = $"{NOKIA_BASE_URL}/oauth2/token";

                var nvc = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", _config.NokiaClientId),
                    new KeyValuePair<string, string>("client_secret", _config.NokiaClientSecret),
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("redirect_uri", NOKIA_REDIRECT_URL)


                };

                var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(nvc));

                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = JsonConvert.DeserializeObject<WithingsTokenResponse>(responseBody);

                    return tokenResponse;

//                    var tokenResponseAccessToken = tokenResponse.access_token;
//                    var tokenResponseRefreshToken = tokenResponse.refresh_token;
//
//                    await _oAuthService.SaveWithingsAccessToken(tokenResponseAccessToken);
//                    await _oAuthService.SaveWithingsRefreshToken(tokenResponseRefreshToken);
                }
                else
                {
                    throw  new Exception($"non success status code : {(int)response.StatusCode} , content: {responseBody}");
                    //await _logger.LogMessageAsync($"non success status code : {response.StatusCode} , content: {responseBody}");
                }
            

        }


        public async Task<WithingsTokenResponse> GetTokensByRefreshToken(string refreshToken)
        {
            var url = $"{NOKIA_BASE_URL}/oauth2/token";

            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", _config.NokiaClientId),
                new KeyValuePair<string, string>("client_secret", _config.NokiaClientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("redirect_uri", NOKIA_REDIRECT_URL)
            };

            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(nvc));

            string responseBody = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonConvert.DeserializeObject<WithingsTokenResponse>(responseBody);

            return tokenResponse;
        }



        private async Task Revoke()
        {
            //    var url1 = "http://musgrosoft-health-api.azurewebsites.net/api/Nokia/Notify";
            //    var url2 = "http://musgrosoft-health-api.azurewebsites.net/api/NokiaNotify/Weights";
            //    var url3 = "http://musgrosoft-health-api.azurewebsites.net/api/Nokia/Notify/Weights";
            //    var url4 = "http://musgrosoft-health-api.azurewebsites.net/api/NokiaNotify/BloodPressures";
            //    var url5 = "http://musgrosoft-health-api.azurewebsites.net/api/Nokia/Notify/BloodPressures";

            //    var accessToken = await _nokiaAuthenticator.GetAccessToken();

            //    _httpClient.DefaultRequestHeaders.Clear();
            //    _httpClient.DefaultRequestHeaders.Accept.Clear();

            //    var uri = NOKIA_BASE_URL + $"/notify?action=revoke&access_token={accessToken}&callbackurl={HttpUtility.UrlEncode(weightsCallback)}&appli={WeightKgMeasureTypeId}";

            //    var response = await _httpClient.GetAsync(uri);

        }

        public async Task Subscribe(string accessToken)
        {
            await Revoke();

            
            //var callback = "http://musgrosoft-health-api.azurewebsites.net/api/nokia";

            var weightsCallback = "http://musgrosoft-health-api.azurewebsites.net/api/Nokia/Notify/Weights";
            var bloodPressuresCallback = "http://musgrosoft-health-api.azurewebsites.net/api/Nokia/Notify/BloodPressures";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Clear();

            // var uri = NOKIA_BASE_URL + $"/notify?action=revoke&access_token={accessToken}&callback={HttpUtility.UrlEncode(callback)}&appli={WeightKgMeasureTypeId}";


            //var uri = NOKIA_BASE_URL + $"/notify?action=subscribe&access_token={accessToken}&callbackurl={HttpUtility.UrlEncode(callback)}&appli={WeightKgMeasureTypeId}";
            var uri = NOKIA_BASE_URL + $"/notify?action=subscribe&access_token={accessToken}&callbackurl={HttpUtility.UrlEncode(weightsCallback)}&appli={WeightKgMeasureTypeId}";

            var response = await _httpClient.GetAsync(uri);
            var content = await response.Content.ReadAsStringAsync();

            await _logger.LogMessageAsync("Status code ::: " + response.StatusCode);
            await _logger.LogMessageAsync("content ::: " + content);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Clear();

            //uri = NOKIA_BASE_URL + $"/notify?action=subscribe&access_token={accessToken}&callbackurl={HttpUtility.UrlEncode(callback)}&appli={SubscribeBloodPressureId}";
            uri = NOKIA_BASE_URL + $"/notify?action=subscribe&access_token={accessToken}&callbackurl={HttpUtility.UrlEncode(bloodPressuresCallback)}&appli={SubscribeBloodPressureId}";

            response = await _httpClient.GetAsync(uri);
            content = await response.Content.ReadAsStringAsync();

            await _logger.LogMessageAsync("Status code ::: " + response.StatusCode);
            await _logger.LogMessageAsync("content ::: " + content);

        }

        //public async Task<string> GetSubscriptions()
        //{
        //    string subscriptions;

        //    var accessToken = await _nokiaAuthenticator.GetAccessToken();
            
        //    //return null;
        //    _httpClient.DefaultRequestHeaders.Clear();
        //    _httpClient.DefaultRequestHeaders.Accept.Clear();

        //    var response = await _httpClient.GetAsync($"{NOKIA_BASE_URL}/notify?action=list&access_token={accessToken}&appli={WeightKgMeasureTypeId}");
        //    var content = await response.Content.ReadAsStringAsync();

        //    subscriptions = $"Subscription Weight ::: {response.StatusCode} , {content} <br/>" ;

        //    response = await _httpClient.GetAsync($"{NOKIA_BASE_URL}/notify?action=list&access_token={accessToken}&appli={SubscribeBloodPressureId}");
        //    content = await response.Content.ReadAsStringAsync();

        //    subscriptions += $"Subscription blood pressure ::: {response.StatusCode} , {content}";

        //    return subscriptions;
        //}

        public async Task<string> GetWeightSubscription(string accessToken)
        {
            
            //var accessToken = await _withingsAuthenticator.GetAccessToken();

            //return null;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Clear();

            var response = await _httpClient.GetAsync($"{NOKIA_BASE_URL}/notify?action=list&access_token={accessToken}&appli={WeightKgMeasureTypeId}");
            var content = await response.Content.ReadAsStringAsync();

            return $"Subscription Weight ::: {response.StatusCode} , {content} <br/>";
            
        }

        public async Task<string> GetBloodPressureSubscription(string accessToken)
        {
            
            //var accessToken = await _withingsAuthenticator.GetAccessToken();

            //return null;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Clear();


            var response = await _httpClient.GetAsync($"{NOKIA_BASE_URL}/notify?action=list&access_token={accessToken}&appli={SubscribeBloodPressureId}");
            var content = await response.Content.ReadAsStringAsync();

            return $"Subscription blood pressure ::: {response.StatusCode} , {content}";
            
        }
        
        public async Task<IEnumerable<Response.Measuregrp>> GetMeasureGroups(string accessToken)
        {
            //var accessToken = await _withingsAuthenticator.GetAccessToken();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Clear();

            var response = await _httpClient.GetAsync($"{NOKIA_BASE_URL}/measure?action=getmeas&access_token={accessToken}");

            

            //TODO : success status code, does not indicate lack of error
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                await _logger.LogMessageAsync("NOK AUTH " + content);

                var data = JsonConvert.DeserializeObject<Response.RootObject>(content);
                return data.body.measuregrps;

            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();

                await _logger.LogMessageAsync("NOK AUTH e " + content);

                throw new Exception($"Error calling nokia api , status code is {response.StatusCode} , and content is {await response.Content.ReadAsStringAsync()}");
            }
        }


    }
}