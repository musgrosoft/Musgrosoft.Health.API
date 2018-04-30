﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Repositories.Models;
using Services.Nokia.Domain;
using Utils;

namespace Services.Nokia
{
    public class NokiaClient
    {
        private const int WeightKgMeasureTypeId = 1;
        private const int FatRatioPercentageMeasureTypeId = 6;
        private const int DiastolicBlooPressureMeasureTypeId = 9;
        private const int SystolicBlooPressureMeasureTypeId = 10;

        private const string NOKIA_BASE_URL = "http://api.health.nokia.com";
        
        public async Task<IEnumerable<Weight>> GetScaleMeasures(DateTime sinceDateTime)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();

            var response = await client.GetAsync($"{NOKIA_BASE_URL}/measure?action=getmeas&oauth_consumer_key=ebb1cbd42bb69687cb85ccb20919b0ff006208b79c387059123344b921837d8d&oauth_nonce=742bef6a3da52fbf004573d18b8f04cf&oauth_signature=cgO95H%2Fg2qx0VQ9ma2k8qeHronM%3D&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1503326610&oauth_token=7f027003b78369d415bd0ee8e91fdd43408896616108b72b97fd7c153685f&oauth_version=1.0&userid=8792669");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Response.RootObject>(content);
                
                var weights = new List<Weight>();

                var dateFilteredMeasures = data.body.measuregrps.Where(x => x.date.ToDateFromUnixTime() >= sinceDateTime);
                var weightMeasures = dateFilteredMeasures.Where(x => x.measures.Any(y => y.type == WeightKgMeasureTypeId)).ToList();

                foreach (var weightMeasure in weightMeasures)
                {
                    weights.Add(new Weight
                    {
                        DateTime = weightMeasure.date.ToDateFromUnixTime(),
                        Kg = (Decimal)( weightMeasure.measures.First(x => x.type == WeightKgMeasureTypeId).value * Math.Pow(10, weightMeasure.measures.First(x => x.type == WeightKgMeasureTypeId).unit)),

                        //todo set if available
                        //  FatRatioPercentage = bodyMeasuregrp.measures.First(x => x.type == FatRatioPercentageMeasureTypeId).value * Math.Pow(10, bodyMeasuregrp.measures.First(x => x.type == FatRatioPercentageMeasureTypeId).unit),
                    });
                }

                return weights;
            }
            else
            {
                throw new Exception($"Error calling nokia api , status code is {response.StatusCode} , and content is {await response.Content.ReadAsStringAsync()}");
            }
        }

        public async Task<IEnumerable<BloodPressure>> GetBloodPressures(DateTime sinceDateTime)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();

            var response = await client.GetAsync($"{NOKIA_BASE_URL}/measure?action=getmeas&oauth_consumer_key=ebb1cbd42bb69687cb85ccb20919b0ff006208b79c387059123344b921837d8d&oauth_nonce=742bef6a3da52fbf004573d18b8f04cf&oauth_signature=cgO95H%2Fg2qx0VQ9ma2k8qeHronM%3D&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1503326610&oauth_token=7f027003b78369d415bd0ee8e91fdd43408896616108b72b97fd7c153685f&oauth_version=1.0&userid=8792669");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Response.RootObject>(content);

                var bloodPressures = new List<BloodPressure>();

                var dateFilteredMeasures = data.body.measuregrps.Where(x => x.date.ToDateFromUnixTime() >= sinceDateTime);

                var bloodPressureMeasures = dateFilteredMeasures.Where(x =>
                    x.measures.Any(y => y.type == DiastolicBlooPressureMeasureTypeId) &&
                    x.measures.Any(y => y.type == SystolicBlooPressureMeasureTypeId)).ToList();

                foreach (var bloodPressureMeasure in bloodPressureMeasures)
                {
                    bloodPressures.Add(new BloodPressure
                    {
                        DateTime = bloodPressureMeasure.date.ToDateFromUnixTime(),
                        Diastolic = (int)( bloodPressureMeasure.measures.First(x => x.type == DiastolicBlooPressureMeasureTypeId).value * Math.Pow(10, bloodPressureMeasure.measures.First(x => x.type == DiastolicBlooPressureMeasureTypeId).unit)),
                        Systolic = (int)( bloodPressureMeasure.measures.First(x => x.type == SystolicBlooPressureMeasureTypeId).value * Math.Pow(10, bloodPressureMeasure.measures.First(x => x.type == SystolicBlooPressureMeasureTypeId).unit)),
                    });
                }

                return bloodPressures;
            }
            else
            {
                throw new Exception($"Error calling nokia api , status code is {response.StatusCode} , and content is {await response.Content.ReadAsStringAsync()}");
            }
        }


    }
}