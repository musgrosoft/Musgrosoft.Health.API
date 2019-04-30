﻿using System;
using System.Collections.Generic;
using Repositories.Health.Models;

namespace Importer.GoogleSheets
{
    public class MapFunctions : IMapFunctions
    {
       
        public Drink MapRowToDrink(IList<object> row)
        {
            return new Drink
            {
                CreatedDate = DateTime.Parse((string)row[0]),
                Units = Double.Parse((string)row[2])
            };
        }

        public Exercise MapRowToExerise(IList<object> row)
        {
            return new Exercise
            {
                CreatedDate = DateTime.Parse((string)row[0]),
                Metres = int.Parse((string)row[1]),
                TotalSeconds = int.Parse((string)row[4]),
                Description = (string)row[3]
            };
        }
    }
}