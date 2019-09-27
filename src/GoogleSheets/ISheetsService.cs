﻿using System.Collections.Generic;
using Repositories.Health.Models;

namespace GoogleSheets
{
    public interface ISheetsService
    {
        List<Drink> GetDrinks();
        List<Exercise> GetExercises();

        void InsertExercises(Exercise exercise);

        List<Drink> GetHistoricDrinks();
    }
}