using System.Collections.Generic;
using HealthCareBot.Integration.Models;

namespace HealthCareBot.Integration.Interfaces
{
    public interface IHospitalSearchService
    {
        List<Hospital> SearchByPostcode(string postCode, MedicalSpecialities medicalSpecialty);

        List<Hospital> SearchByCoordinates(string latitude, string longitude, MedicalSpecialities medicalSpecialty);

        List<Hospital> SearchByDoctorName(string doctorName);
    }
}