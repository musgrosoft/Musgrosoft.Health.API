﻿using System;

namespace HealthAPI.Models
{
    public class BloodPressure
    {
        public DateTime DateTime { get; set; }
        public int? Diastolic { get; set; }
        public int? Systolic { get; set; }
    }
}