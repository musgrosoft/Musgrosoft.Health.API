﻿using System;
using Microsoft.EntityFrameworkCore;
using Repositories.Health.Models;
using Repositories.OAuth.Models;
using Utils;

namespace Repositories
{
    public class HealthContext : DbContext
    {
        public virtual DbSet<BloodPressure> BloodPressures { get; set; }
        public virtual DbSet<RestingHeartRate> RestingHeartRates { get; set; }
        public virtual DbSet<Drink> Drinks { get; set; }
        public virtual DbSet<Weight> Weights { get; set; }
        public virtual DbSet<Exercise> Exercises { get; set; }
        public virtual DbSet<Target> Targets { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<CalendarDate> CalendarDates { get; set; }
        public virtual DbSet<SleepSummary> SleepSummaries { get; set; }
        public virtual DbSet<SleepState> SleepStates { get; set; }

        private readonly ILogger _logger;
        
        public HealthContext(DbContextOptions<HealthContext> options, ILogger logger) : base(options)
        {
            _logger = logger;
        }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Exercise>().HasKey(c => new { c.CreatedDate, c.Description });

            if (this.Database.IsSqlServer())
            {

                //Seed CalendarDates
                for (var date = new DateTime(2010, 1, 1); date < new DateTime(2030, 1, 1); date = date.AddDays(1))
                {
                    modelBuilder.Entity<CalendarDate>().HasData(new CalendarDate {Date = date});
                }


            }
        }


        //private int GetTargetErgoMetresFor15Minutes(DateTime date)
        //{
        //    return (int)(3386 + (date - new DateTime(2019, 1, 1)).TotalDays);
        //}

        //private int GetTargetMetresFor30MinuteTreadmill(DateTime date)
        //{
        //    return (int)(5000 + 2.74725275 * (date - new DateTime(2019, 1, 1)).TotalDays);
        //}


    }
}
