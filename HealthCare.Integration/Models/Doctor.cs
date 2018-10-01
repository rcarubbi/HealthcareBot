using System.Collections.Generic;

namespace HealthCareBot.Integration.Models
{
    public class Doctor
    {
        public string Name { get; set; }

        public List<MedicalSpecialities> MedicalSpecialities { get; set; }
    }
}