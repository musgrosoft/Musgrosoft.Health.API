﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class Ergo

    {
        [Key]
        public DateTime CreatedDate { get; set; }
        public TimeSpan Time { get; set; }
        public Double Metres { get; set; }
    }
}
