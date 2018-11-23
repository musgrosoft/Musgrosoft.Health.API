﻿namespace Utils
{
    public interface IConfig
    {
        //Database
        string HealthDbConnectionString { get; }

        //Logging Logz.io 
        string LogzIoToken { get; }

        //Fitbit
        string FitbitClientId { get; }
        string FitbitClientSecret { get; }
        string FitbitUserId { get; }
        string FitbitVerificationCode { get; }
        string FitbitOAuthRedirectUrl { get; }

        //Google Sheets
        string GoogleClientId { get; }
        string GoogleClientSecret { get; }
        string AlcoholSpreadsheetId { get; }
        string ExerciseSpreadsheetId { get; }

        //Nokia Health
        string NokiaClientId { get; }
        string NokiaClientSecret { get; }

    }
}