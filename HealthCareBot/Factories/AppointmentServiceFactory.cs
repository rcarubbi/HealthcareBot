using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HealthCareBot.Integration.Interfaces;
using HealthCareBot.Integration.Interfaces.Fakes;
using HealthCareBot.Integration.Models;

namespace HealthCareBot.Factories
{
    public class AppointmentServiceFactory
    {
        public static IAppointmentService Create()
        {
            var service = new StubIAppointmentService()
            {
                TryBookInt32StringDateTime = (id, doctorName, desiredDateTime) =>
                {
                    if (desiredDateTime == new DateTime(2018, 10, 10, 10, 0, 0))
                    {
                        return new AppointmentBookingResult()
                        {
                            Success= false,
                            SuggestedDateTimes= new List<DateTime>
                            {
                                new DateTime(2017, 10, 09, 17, 0, 0),
                                new DateTime(2017, 10, 10, 13, 0, 0),
                                new DateTime(2017, 10, 10, 15, 0, 0),
                                new DateTime(2017, 10, 10, 16, 0, 0),
                                new DateTime(2017, 10, 11, 09, 0, 0)
                            }
                        };
                    }
                    else
                    {
                        return new AppointmentBookingResult
                        {
                            Success = true
                        };
                    }
                }
            };

            return service;
        }
    }
}