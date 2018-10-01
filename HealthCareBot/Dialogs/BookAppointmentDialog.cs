using HealthCareBot.Extensions;
using HealthCareBot.Factories;
using HealthCareBot.Integration.Interfaces;
using HealthCareBot.Integration.Models;
using HealthCareBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCareBot.Dialogs
{
    [Serializable]

    public class BookAppointmentDialog : IDialog<object>
    {
        private const string NewAppointment = "New";
        private const string MyAppointments = "My appointments";
        private const string ByDoctorsNameOption = "By doctors name";
        private const string FindNearHospitalsOption = "Find near hospitals";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            PromptDialog.Choice(context, OnSchedulingOptionSelected, new List<string> {NewAppointment, MyAppointments},
                "Choose an option for appointments:", "It's not a valid option!");
            return Task.CompletedTask;
        }

        private async Task OnSchedulingOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var optionSelected = await result;

                switch (optionSelected)
                {
                    case NewAppointment:
                        PromptDialog.Choice(context, ResumeAfterHospitalSearchTypeSelected,
                            new[] {ByDoctorsNameOption, FindNearHospitalsOption},
                            "How do you want to inform your hospital?");
                        break;
                    case MyAppointments:
                        var repo = UserRepositoryFactory.CreateUserRepository();
                        var user = context.UserData.GetValue<User>("user");
                        var appointments = repo.SearchAppointments(user.Number, user.AccessKey);
                        await ShowAppointments(context, appointments);
                        context.Done<object>(null);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                System.Diagnostics.Trace.TraceError($"Error: {ex.Message}");
                await context.PostAsync($"Ooops! Too many attempts. Don't worry, I'm trying to solve this issue, please try again.");
            }
        }

        private static async Task ShowAppointments(IBotContext context, List<Appointment> appointments)
        {
            if (appointments.Count == 0)
            {
                await context.PostAsync("You don't have any pending appointment");
            }
            else
            {
                var usuario = context.UserData.GetValue<User>("user");
                var message = context.MakeMessage();
                message.AddAppointmentsCard(usuario.Name, appointments);
                await context.PostAsync(message);
            }
        }

        private async Task ResumeAfterHospitalSearchTypeSelected(IDialogContext context, IAwaitable<string> result)
        {
            var resposta = await result;

            if (resposta == ByDoctorsNameOption)
            {
                var form = FormDialog.FromForm(BuildAppointmentBookingForm, FormOptions.PromptInStart);
                context.Call(form, AfterAppointmentScheduleFormFilled);
            }
            else
            {
                context.Call(new SearchHospitalDialog(), AfterSearchHospitals);
            }
        }

        private async Task AfterAppointmentScheduleFormFilled(IDialogContext context, IAwaitable<AppointmentBookingQuery> result)
        {
            var appointmentData = await result;

            await BookAppointment(context, appointmentData);

        }

        private async Task BookAppointment(IDialogContext context, AppointmentBookingQuery appointmentData)
        {
            var hospitalSearchService = HospitalSearchServiceFactory.Create();
            var hospitals = hospitalSearchService.SearchByDoctorName(appointmentData.DoctorName);
            if (hospitals.Count == 0)
            {
                PromptDialog.Confirm(context, RetryAgendamento,
                    "I couldn't find this Doctor in my database. Do you wanna try again?",
                    "It's not a valid option!");

            }
            else
            {
                var appointmentService = AppointmentServiceFactory.Create();  
                var appointmentBookingResult = appointmentService.TryBook(hospitals[0].Id, appointmentData.DoctorName, appointmentData.DesiredDateTime);
                if (appointmentBookingResult.Success)
                {
                    await ShowAppointmentConfirmation(context, appointmentData);
                    context.Done<object>(null);
                }
                else
                {
                    context.PrivateConversationData.SetValue("appointment", appointmentData);
                    ShowSuggestedDateTimes(context, appointmentBookingResult.SuggestedDateTimes);
                }
            }
        }

        private static async Task ShowAppointmentConfirmation(IBotToUser context, AppointmentBookingQuery appointmentData)
        {
            await context.PostAsync(
                $@"Okay! Your appointment has been booked to {appointmentData.DesiredDateTime.ToShortDateString()}
                    at {appointmentData.DesiredDateTime.ToShortTimeString()} with {appointmentData.DoctorName}");
        }

        private void ShowSuggestedDateTimes(IDialogContext context, IEnumerable<DateTime> suggesteDateTimes)
        {
            PromptDialog.Choice(context, AfterSelectDateTime, suggesteDateTimes,
                "Unfortunately the desired time is unavailable however we have the following times that maybe are suitable for you...");
        }

        private async Task AfterSelectDateTime(IDialogContext context, IAwaitable<DateTime> result)
        {
            var appointmentData = context.PrivateConversationData.GetValue<AppointmentBookingQuery>("appointment");
            appointmentData.DesiredDateTime = await result;
            await BookAppointment(context, appointmentData);
        }

        private async Task RetryAgendamento(IDialogContext context, IAwaitable<bool> result)
        {
            var resposta = await result;
            if (resposta)
            {
                PromptDialog.Choice(context,
                    ResumeAfterHospitalSearchTypeSelected,
                    new[] {ByDoctorsNameOption, FindNearHospitalsOption}, "How do you want inform the hospital?");
            }
            else
            {
                context.Done<object>(null);
            }
        }

        private IForm<AppointmentBookingQuery> BuildAppointmentBookingForm()
        {
            var form = new FormBuilder<AppointmentBookingQuery>()
                .Message("Okay, now I'll ask you some information required to book the appointment")
                .Field(nameof(AppointmentBookingQuery.DoctorName))
                .Field(nameof(AppointmentBookingQuery.DesiredDateTime))
                .Build();
            return form;
        }

        private async Task AfterSearchHospitals(IDialogContext context, IAwaitable<List<Hospital>> result)
        {
            var hospitals = await result;
            context.PrivateConversationData.SetValue("hospitals", hospitals);
            PromptDialog.Choice(context, AfterSelectHospital, hospitals.Select(h => h.Name).ToArray(), "Select a Hospital");
        }

        private async Task AfterSelectHospital(IDialogContext context, IAwaitable<string> result)
        {
            var hospitalName = await result;
            var locais = context.PrivateConversationData.GetValue<List<Hospital>>("hospitals");
            var doctors = locais.First(x => x.Name == hospitalName).Doctors;
            if (doctors.Count > 1)
            {
                context.PrivateConversationData.SetValue("doctors", doctors);
                PromptDialog.Choice(context, AfterSelectDoctor, doctors.Select(x => x.Name).ToArray(), "Select a doctor");
            }
            else
            {
                var doctor = doctors.First();
                var appointment = new AppointmentBookingQuery { DoctorName = doctor.Name };
                var form = new FormDialog<AppointmentBookingQuery>(appointment, BuildAppointmentBookingForm, FormOptions.PromptInStart);
                context.Call(form, AfterAppointmentScheduleFormFilled);

            }
        }

        private async Task AfterSelectDoctor(IDialogContext context, IAwaitable<string> result)
        {
            var doctorName = await result;
            var appoitmentData = new AppointmentBookingQuery { DoctorName = doctorName };
            var form = new FormDialog<AppointmentBookingQuery>(appoitmentData, BuildAppointmentBookingForm, FormOptions.PromptInStart);
            context.Call(form, AfterAppointmentScheduleFormFilled);
        }
    }
}