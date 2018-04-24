﻿using System;
using System.Collections.Generic;
using System.Linq;
using HealthAPI.Controllers;
using HealthAPI.Models;
using HealthAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HealthAPI.Unit.Tests.Controllers
{
    public class WeightsControllerTests
    {
        [Fact]
        public void Test1()
        {
            var context = new FakeLocalContext();
            
            context.Add(new Models.Weight { Kg = 123, DateTime = DateTime.Now});
            context.Add(new Models.Weight { Kg = 345, DateTime = DateTime.Now});
            context.Add(new Models.Weight { Kg = 678, DateTime = DateTime.Now});
            context.SaveChanges();

            var weightsControlller = new WeightsController(context);



            //var data = new List<Weight>
            //{
            //    new Weight{Kg = 123, DateTime = DateTime.Now},
            //    new Weight{Kg = 456, DateTime = DateTime.Now},
            //    new Weight{Kg = 789, DateTime = DateTime.Now},
            //}.AsQueryable();

            //var dbSet = new Mock<DbSet<Weights>>();

            //dbSet.As<IQueryable<Weight>>().Setup(m => m.Provider).Returns(data.Provider);
            //dbSet.As<IQueryable<Weight>>().Setup(m => m.Expression).Returns(data.Expression);
            //dbSet.As<IQueryable<Weight>>().Setup(m => m.ElementType).Returns(data.ElementType);
            //dbSet.As<IQueryable<Weight>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            ////dbSet.As<IEnumerable<Weight>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());


            //dbSet.As<IEnumerable<Weight>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            //var healthContxt = new Mock<IHealthContext>();
            //healthContxt.Setup(x => x.Weights).Returns(dbSet.Object);

            //var weightsControlller = new WeightsController(healthContxt.Object);

            var result = weightsControlller.Get().ToList();

            Assert.Equal(3, result.Count());
            Assert.Contains(result, x =>x.Kg == 123);

        }
    }


}