using Microsoft.Bot.Builder.FormFlow;
using System;

namespace HealthCareBot.Models
{
    public class AppointmentBookingQuery
    {
        [Prompt("tell me the name of the doctor you are looking for")]
        public string DoctorName { get; set; }

        [Prompt("now inform the desired date and time to the appointment (format: dd/mm/aaaa hh:mm)")]
        public DateTime DesiredDateTime { get; set; }
    }
}