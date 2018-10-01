using HealthCareBot.Integration.Models;
using System;

namespace HealthCareBot.Integration.Interfaces
{
    public interface IAppointmentService
    {
         AppointmentBookingResult TryBook(int id, string doctorName, DateTime desiredDateTime);
    }
}
