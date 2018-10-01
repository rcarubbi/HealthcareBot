using HealthCareBot.Integration.Interfaces;
using HealthCareBot.Integration.Interfaces.Fakes;
using HealthCareBot.Integration.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace HealthCareBot.Factories
{
    public class UserRepositoryFactory
    {
        internal static IUserRepository CreateUserRepository()
        {

            var instance = new StubIUserRepository()
            {
                SearchAppointmentsStringString = (number, accessKey) =>
                {
                    if ((number == "123456789" && accessKey == "abc1234") || (number == "012345678" && accessKey == "abc4321"))
                    {
                        return new List<Appointment>()
                        {
                            new Appointment
                            {
                                DateTime = new DateTime(2017,10,10,10,0,0),
                                Hospital = new Hospital()
                                {
                                    Address = "Rua Schilling, 77 - Vila Leopoldina - São Paulo - SP, 050020-000",
                                    Doctors = new List<Doctor>
                                        {
                                            new Doctor() { Name = "Dr. Huang Yu Sheng", MedicalSpecialities = new List<MedicalSpecialities>() { MedicalSpecialities.Acupuncture }}
                                        },
                                    Name = "Clinica de Acupuntura do Dr. Huang",
                                    Latitude = -23.5296835,
                                    Longitude = -46.7293928
                                },
                                DoctorName = "Dr. Huang Yu Sheng"
                            },
                            new Appointment
                            {
                                DateTime = new DateTime(2017,10,15,16,0,0),
                                Hospital = new Hospital
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
                                },
                                DoctorName = "Dr. Javier Carbajal"
                            },
                        };
                    }

                    return new List<Appointment>();
                },
                SearchStringString = (number, accessKey) =>
                {
                    switch (number)
                    {
                        case "123456789" when accessKey == "abc1234":
                            return new User()
                            {
                                AccessKey = accessKey,
                                Number = number,
                                Name = "Raphael Carubbi Neto"
                            };
                        case "234567890" when accessKey == "def5678":
                            return new User()
                            {
                                AccessKey = accessKey,
                                Number = number,
                                Name = "Ed Carlos Carnieto"
                            };
                        case "012345678" when accessKey == "abc4321":
                            return new User()
                            {
                                AccessKey = accessKey,
                                Number = number,
                                Name = "João Carlos Basile"
                            };
                        default:
                            return null;
                    }
                },
                SearchRefundsStringString = (number, accessKey) => new List<Refund>()
                {
                    new Refund
                    {
                        Number = 1,
                        Amount = 120,
                        RequestDate = new DateTime(2017, 05, 10),
                        PaymentDate = new DateTime(2017, 06, 05)
                    },
                    new Refund
                    {
                        Number = 2,
                        Amount = 340,
                        RequestDate = new DateTime(2017, 06, 10),
                    },
                    new Refund
                    {
                        Number = 3,
                        Amount = 210,
                        RequestDate = new DateTime(2017, 06, 22),
                    }
                },
                RequestBalanceSlipStringStringInt32 = (number, accessKey, year) => $"{ConfigurationManager.AppSettings["BaseUrl"]}/img/BalanceDocument.jpg",
                GenerateBarcodeBillStringStringInt32Int32 = (numero, chave, ano, mes) => $"{ConfigurationManager.AppSettings["BaseUrl"]}/img/BarcodeSlip.jpg"
            };

            return instance;
        }
    }
}