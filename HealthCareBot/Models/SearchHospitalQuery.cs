using HealthCareBot.Integration.Models;
using Microsoft.Bot.Builder.FormFlow;
using System;

namespace HealthCareBot.Models
{
    [Serializable]

    public class SearchHospitalQuery
    {
        [Prompt("Okay, so now tell me the postcode")]
        public string Postcode { get; set; }
        public MedicalSpecialities MedicalSpecialty { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}