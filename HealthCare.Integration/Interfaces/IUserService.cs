using HealthCareBot.Integration.Models;
using System.Collections.Generic;

namespace HealthCareBot.Integration.Interfaces
{
    public interface IUserRepository
    {
        User Search(string number, string accessKey);
        List<Appointment> SearchAppointments(string number, string accessKey);
        List<Refund> SearchRefunds(string number, string accessKey);
        string RequestBalanceSlip(string number, string accessKey, int year);
        string GenerateBarcodeBill(string number, string accessKey, int year, int month);
    }
}
