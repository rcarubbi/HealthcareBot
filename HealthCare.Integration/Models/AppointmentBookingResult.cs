using System;
using System.Collections.Generic;

namespace HealthCareBot.Integration.Models
{
    public class AppointmentBookingResult
    {
        public bool Success { get; set; }
        public List<DateTime> SuggestedDateTimes { get; set; }
    }
}
