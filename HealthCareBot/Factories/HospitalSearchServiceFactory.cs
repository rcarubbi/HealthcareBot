using HealthCareBot.Integration.Interfaces;
using HealthCareBot.Integration.Interfaces.Fakes;
using HealthCareBot.Integration.Models;
using System;
using System.Collections.Generic;

namespace HealthCareBot.Factories
{
    public class HospitalSearchServiceFactory
    {
        internal static IHospitalSearchService Create()
        {
            return new StubIHospitalSearchService
            {
                SearchByDoctorNameString = (doctorName) =>
                {
                    if (doctorName == "Dr. Huang Yu Sheng")
                    {
                        return new List<Hospital>
                        {
                            new Hospital
                            {
                                Address = "Rua Schiling, 257 - Vila Hamburguesa, São Paulo - SP, 05302-003",
                                Doctors = new List<Doctor>
                                {
                                    new Doctor
                                    {
                                        Name = "Dr. Huang Yu Sheng",
                                        MedicalSpecialities = new List<MedicalSpecialities>
                                        {
                                            MedicalSpecialities.Acupuncture
                                        }
                                    }
                                },
                                Name = "Clinica de Acupuntura do Dr. Huang",
                                Latitude = -23.5296835,
                                Longitude = -46.7293928
                            }
                        };
                    }

                    if (doctorName == "Dr. Javier Carbajal")
                    {
                        return new List<Hospital> {
                            new Hospital()
                            {
                                Address = "R. Salete, 200 - Conjunto 43 - Santana, São Paulo - SP, 02016-001",
                                Doctors = new List<Doctor>
                                {
                                    new Doctor() { Name = "Dr. Javier Carbajal", MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Immunology }}
                                },
                                Name = "Clínica de Alergia e Imunologia Dr. Javier Carbajal",
                                Latitude = -23.502059,
                                Longitude = -46.6292571
                            }};
                    }

                    if (doctorName == "Dr. Matheus Castro")
                    {
                        return new List<Hospital> {
                            new Hospital()
                        {
                            Address = "R. Salete, 200 - Conjunto 43 - Santana, São Paulo - SP, 02016-001",
                            Doctors = new List<Doctor>
                            {
                                new Doctor() { Name = "Dr. Javier Carbajal", MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Immunology }},
                                new Doctor() { Name = "Dr. Matheus Castro", MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Immunology, MedicalSpecialities.Endocrinology }}
                            },
                            Name = "Clínica de Alergia e Imunologia Dr. Javier Carbajal",
                            Latitude = -23.502059,
                            Longitude = -46.6292571
                        }};
                    }

                    if (doctorName == "DR. JOSÉ CARLOS MORI")
                    {
                        return new List<Hospital> {
                            new Hospital()
                        {
                            Address = "R. Vespasiano, 32 - Vila Romana, São Paulo - SP, 05033-000",
                            Doctors = new List<Doctor>
                            {
                                new Doctor { Name = "DR. JOSÉ CARLOS MORI" , MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Immunology }}
                            },
                            Name = "DR. JOSÉ CARLOS MORI",
                            Latitude = -23.5225621,
                            Longitude = -46.6976603
                        }};
                    }

                    return new List<Hospital>();
                },
                SearchByPostcodeStringMedicalSpecialities = (postcode, medicalSpecialty) =>
                {
                    var hospitals = new List<Hospital>();
                    if (postcode != "05302030") return hospitals;
                    switch (medicalSpecialty)
                    {
                        case MedicalSpecialities.Acupuncture:
                            hospitals.Add(new Hospital
                            {
                                Address = "Rua Schiling, 257 - Vila Hamburguesa, São Paulo - SP, 05302-003",
                                Doctors = new List<Doctor>
                                {
                                    new Doctor { Name = "Dr. Huang Yu Sheng", MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Acupuncture }}
                                },
                                Name = "Clinica de Acupuntura do Dr. Huang",
                                Latitude = -23.5296835,
                                Longitude = -46.7293928
                            });
                            break;
                        case MedicalSpecialities.Immunology:
                            hospitals.Add(new Hospital
                            {
                                Address = "R. Salete, 200 - Conjunto 43 - Santana, São Paulo - SP, 02016-001",
                                Doctors = new List<Doctor>
                                {
                                    new Doctor { Name = "Dr. Javier Carbajal", MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Immunology }},
                                    new Doctor { Name = "Dr. Matheus Castro", MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Immunology, MedicalSpecialities.Endocrinology }}
                                },
                                Name = "Clínica de Alergia e Imunologia Dr. Javier Carbajal",
                                Latitude = -23.502059,
                                Longitude = -46.6292571
                            });
                            hospitals.Add(new Hospital
                            {
                                Address = "R. Vespasiano, 32 - Vila Romana, São Paulo - SP, 05033-000",
                                Doctors = new List<Doctor>
                                {
                                    new Doctor { Name = "DR. JOSÉ CARLOS MORI" , MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Immunology }}
                                },
                                Name = "DR. JOSÉ CARLOS MORI",
                                Latitude = -23.5225621,
                                Longitude = -46.6976603
                            });
                            break;
                        case MedicalSpecialities.Angiology:
                            break;
                        case MedicalSpecialities.Cardiology:
                            break;
                        case MedicalSpecialities.Coloproctology:
                            break;
                        case MedicalSpecialities.Dermatology:
                            break;
                        case MedicalSpecialities.Endocrinology:
                            break;
                        case MedicalSpecialities.Gastroenterology:
                            break;
                        case MedicalSpecialities.Geriatrics:
                            break;
                        case MedicalSpecialities.Gynecology:
                            break;
                        case MedicalSpecialities.Obstetrics:
                            break;
                        case MedicalSpecialities.Hematology:
                            break;
                        case MedicalSpecialities.Homeopathy:
                            break;
                        case MedicalSpecialities.Infectology:
                            break;
                        case MedicalSpecialities.Nephrology:
                            break;
                        case MedicalSpecialities.Neurology:
                            break;
                        case MedicalSpecialities.Ophthalmology:
                            break;
                        case MedicalSpecialities.Orthopedics:
                            break;
                        case MedicalSpecialities.Otolaryngology:
                            break;
                        case MedicalSpecialities.Pediatrics:
                            break;
                        case MedicalSpecialities.Pneumology:
                            break;
                        case MedicalSpecialities.Psychiatry:
                            break;
                        case MedicalSpecialities.Rheumatology:
                            break;
                        case MedicalSpecialities.Urology:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(medicalSpecialty), medicalSpecialty, null);
                    }
                    return hospitals;
                },
                SearchByCoordinatesStringStringMedicalSpecialities = (latitude, longitude, medicalSpecialty) =>
                {
                    var hospitals = new List<Hospital>();
                    switch (medicalSpecialty)
                    {
                        case MedicalSpecialities.Acupuncture:
                            hospitals.Add(new Hospital
                            {
                                Address = "Rua Schilling, 77 - Vila Leopoldina - São Paulo - SP, 050020-000",
                                Doctors = new List<Doctor>
                                    {
                                        new Doctor { Name = "Dr. Huang Yu Sheng", MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Acupuncture }}
                                    },
                                Name = "Clinica de Acupuntura do Dr. Huang",
                                Latitude = -23.5296835,
                                Longitude = -46.7293928
                            });

                            break;
                        case MedicalSpecialities.Immunology:
                            hospitals.Add(new Hospital
                            {
                                Address = "R. Salete, 200 - Conjunto 43 - Santana, São Paulo - SP, 02016-001",
                                Doctors = new List<Doctor>
                                    {
                                        new Doctor { Name = "Dr. Javier Carbajal", MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Immunology }},
                                        new Doctor { Name = "Dr. Matheus Castro", MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Immunology, MedicalSpecialities.Endocrinology }}
                                    },
                                Name = "Clínica de Alergia e Imunologia Dr. Javier Carbajal",
                                Latitude = -23.502059,
                                Longitude = -46.6292571
                            });
                            hospitals.Add(new Hospital
                            {
                                Address = "Rua Passos da Pátria, 334 - Alto da lapa - São Paulo - SP, 05040-000",
                                Doctors = new List<Doctor>
                                    {
                                        new Doctor { Name = "DR. JOSÉ CARLOS MORI" , MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Immunology }}
                                    },
                                Name = "DR. JOSÉ CARLOS MORI",
                                Latitude = -23.5225621,
                                Longitude = -46.6976603
                            });
                            break;
                        case MedicalSpecialities.Angiology:
                            break;
                        case MedicalSpecialities.Cardiology:
                            break;
                        case MedicalSpecialities.Coloproctology:
                            break;
                        case MedicalSpecialities.Dermatology:
                            break;
                        case MedicalSpecialities.Endocrinology:
                            break;
                        case MedicalSpecialities.Gastroenterology:
                            break;
                        case MedicalSpecialities.Geriatrics:
                            break;
                        case MedicalSpecialities.Gynecology:
                            break;
                        case MedicalSpecialities.Obstetrics:
                            break;
                        case MedicalSpecialities.Hematology:
                            break;
                        case MedicalSpecialities.Homeopathy:
                            break;
                        case MedicalSpecialities.Infectology:
                            break;
                        case MedicalSpecialities.Nephrology:
                            break;
                        case MedicalSpecialities.Neurology:
                            break;
                        case MedicalSpecialities.Ophthalmology:
                            break;
                        case MedicalSpecialities.Orthopedics:
                            break;
                        case MedicalSpecialities.Otolaryngology:
                            break;
                        case MedicalSpecialities.Pediatrics:
                            break;
                        case MedicalSpecialities.Pneumology:
                            break;
                        case MedicalSpecialities.Psychiatry:
                            break;
                        case MedicalSpecialities.Rheumatology:
                            break;
                        case MedicalSpecialities.Urology:
                            break;
                    }
                    return hospitals;
                }
            };
        }
    }
}