﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models
{
    public class HeartSummary
    {
        [Key]
        [Column(TypeName = "DateTime")]
        public DateTime DateTime { get; set; }
        //public int? RestingHeartRate { get; set; }
        public int? OutOfRangeMinutes { get; set; }
        public int? FatBurnMinutes { get; set; }
        public int? CardioMinutes { get; set; }
        public int? PeakMinutes { get; set; }

        [NotMapped]
        public double? TargetFatBurnAndAbove
        {
            get
            {
                var days = DateTime - new DateTime(2017, 5, 3);

                return days.TotalDays * 30;
            }
        }

        [NotMapped]
        public double? TargetCardioAndAbove
        {
            get
            {
                var days = DateTime - new DateTime(2017, 5, 3);

                return days.TotalDays * 5;
            }
        }

        //[NotMapped]
        //public DateTime Week { get { return DateTime.AddDays(-(int)DateTime.DayOfWeek); } }
    }
}
