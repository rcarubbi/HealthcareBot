using System.Collections.Generic;

namespace HealthCareBot.Integration.Models
{
    public class Hospital
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address  { get; set; }

        public List<Doctor> Doctors { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}