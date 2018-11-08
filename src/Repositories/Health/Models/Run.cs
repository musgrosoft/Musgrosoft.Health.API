﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Repositories.Health.Models
{
    public class Run
    {
        [Key]
        public DateTime CreatedDate { get; set; }
        public TimeSpan Time { get; set; }
        public Double Metres { get; set; }

    }
}