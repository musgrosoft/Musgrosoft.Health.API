﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Withings.Domain;
using Xunit;

namespace Withings.Tests.Unit
{
    public class WithingsClientQueryAdapterTests
    {
        private WithingsClientQueryAdapter _withingsClientQueryAdapter;
        private Mock<IWithingsClient> _withingsClient;

        public WithingsClientQueryAdapterTests()
        {
            _withingsClient = new Mock<IWithingsClient>();
            _withingsClientQueryAdapter = new WithingsClientQueryAdapter(_withingsClient.Object);

        }

        [Fact]
        public async Task ShouldGetMeasureGroups()
        {
            //Given
            var accessToken = "fsdfsdfdsf";
            var filterDate = new DateTime(2010, 1, 15);
            var someMeasureGroups = new List<WithingsMeasureGroupResponse.Measuregrp>
            {
                new WithingsMeasureGroupResponse.Measuregrp {date = (int)ToUnixTimeFromDate(new DateTime(2010,1,1))},
                new WithingsMeasureGroupResponse.Measuregrp {date = (int)ToUnixTimeFromDate(new DateTime(2010,1,2))},
                new WithingsMeasureGroupResponse.Measuregrp {date = (int)ToUnixTimeFromDate(new DateTime(2010,1,3))},
                new WithingsMeasureGroupResponse.Measuregrp {date = (int)ToUnixTimeFromDate(new DateTime(2010,1,16))},
                new WithingsMeasureGroupResponse.Measuregrp {date = (int)ToUnixTimeFromDate(new DateTime(2010,1,17))},
            };

            _withingsClient.Setup(x => x.GetMeasureGroups(accessToken)).Returns(Task.FromResult((IEnumerable<WithingsMeasureGroupResponse.Measuregrp>)someMeasureGroups));

            //When
            var result = await _withingsClientQueryAdapter.GetMeasureGroups(filterDate, accessToken);

            //Then
            Assert.Equal(2,result.Count());
            Assert.Contains(result, x => x.date == (int)ToUnixTimeFromDate(new DateTime(2010, 1, 16)));
            Assert.Contains(result, x => x.date == (int)ToUnixTimeFromDate(new DateTime(2010, 1, 17)));


        }

        public static double ToUnixTimeFromDate(DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (dateTime - epoch).TotalSeconds;
        }
    }
}