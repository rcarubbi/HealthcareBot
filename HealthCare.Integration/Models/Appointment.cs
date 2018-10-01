using System;

namespace HealthCareBot.Integration.Models
{
    public class Appointment
    {
        public DateTime DateTime { get; set; }

        public Hospital Hospital { get; set; }

        public string DoctorName { get; set; }

    }
}