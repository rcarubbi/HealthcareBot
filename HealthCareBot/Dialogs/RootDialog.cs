using HealthCareBot.Factories;
using HealthCareBot.Integration.Interfaces;
using HealthCareBot.Models;
using HealthCareBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCareBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var message = await result;
            if (!Enum.TryParse<MenuOptions>(activity?.Text, out var selectedOption))
            {
                selectedOption = MenuOptions.Other;
            }

            switch (selectedOption)
            {
                case MenuOptions.BookAppointment:
                    await context.Forward(new BookAppointmentDialog(), ResumeAfterDialog, message, CancellationToken.None);
                    break;
                case MenuOptions.SearchHospital:
                    await context.Forward(new SearchHospitalDialog(), ResumeAfterDialog, message, CancellationToken.None);
                    break;
                case MenuOptions.BarcodeBillPayment:
                    await context.Forward(new BarcodeBillPaymentDialog(), ResumeAfterDialog, message, CancellationToken.None);
                    break;
                case MenuOptions.BalanceSlip:
                    await context.Forward(new BalanceSlipDialog(), ResumeAfterDialog, message, CancellationToken.None);
                    break;
                case MenuOptions.SearchRefund:
                    await context.Forward(new SearchRefundDialog(), ResumeAfterDialog, message, CancellationToken.None);
                    break;
                default:

                    if (!context.UserData.TryGetValue<User>("user", out var user))
                    {
                        Authenticate(context, false);
                    }
                    else
                    {
                        await SendGreetings(context, user);
                        context.Wait(MessageReceivedAsync);
                    }
                    break;
            }
        }

        private static async Task ResumeAfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("What do you think about my service?");
            context.Wait(SurveyReceivedAsync);
        }

        private static async Task SurveyReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var text = activity?.Text;
            var textAnalisysService = new TextAnalytics();
            var score = await textAnalisysService.MakeRequest(text);

            if (score < 50)
            {
                var message = context.MakeMessage();
                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                var card = new ThumbnailCard
                {
                    Title = "Sorry for not help you, but you can enter in contact through our other channels",
                    Subtitle = "Click on the buttons below to talk to our agents through phone or chat",
                    Buttons = new List<CardAction>
                    {
                        new CardAction("call", $"Call", null, $"tel:1149029052"),
                        new CardAction(ActionTypes.OpenUrl, $"Chat", null,
                            "http://www.gndi.com.br/web/guest/atendimento-em-saude")
                    }
                };
                message.Attachments.Add(card.ToAttachment());
                await context.PostAsync(message);
            }
            else
            {
                await context.PostAsync("Thanks for your answer, I'll be here if you need anything else");
            }

            InsightUtils.TrackEvent(MenuOptions.Survey);

            context.Done<object>(null);
        }

        private void Authenticate(IDialogStack context, bool retry)
        {
            var dialog = retry ? FormDialog.FromForm(BuildRetryForm, FormOptions.PromptInStart) : FormDialog.FromForm(BuildForm, FormOptions.PromptInStart);
            context.Call(dialog, ResumeAfterAutentication);
        }

        private IForm<Credentials> BuildForm()
        {
            var form = new FormBuilder<Credentials>().Message("Hi, I'm the HeathCare Bot. I need some info about you before help you.")
                .Field(nameof(Credentials.Number))
                .Field(nameof(Credentials.AccessKey))
                .Build();
            return form;
        }

        private IForm<Credentials> BuildRetryForm()
        {
            var form = new FormBuilder<Credentials>().Message("Right, so let's try again")
                .Field(nameof(Credentials.Number))
                .Field(nameof(Credentials.AccessKey))
                .Build();
            return form;
        }

        private async Task ResumeAfterAutentication(IDialogContext context, IAwaitable<Credentials> result)
        {
            var credencials = await result;
            var repo = UserRepositoryFactory.CreateUserRepository();
            var user = repo.Search(credencials.Number, credencials.AccessKey);
            if (user == null)
            {
                PromptDialog.Confirm(context, RetriesAfterAuthenticationFail,
                    "Unfortunately I couldn't find you in my database. Do you wanna try again?",
                    "It's not a valid option!");
            }
            else
            {
                context.UserData.SetValue("user", user);
                var message = context.MakeMessage();
                message.Text = $"Right {user.Name}, now tell me how I can help you today?";
                await context.PostAsync(message);
                ShowCards(context);
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task RetriesAfterAuthenticationFail(IDialogContext context, IAwaitable<bool> result)
        {
            try
            {
                var optionSelected = await result;

                if (optionSelected)
                {
                    Authenticate(context, true);
                }
                else
                {
                    await context.PostAsync("Ok, just let me know if you need more help");
                    context.Done<object>(null);

                }
            }
            catch (TooManyAttemptsException ex)
            {
                System.Diagnostics.Trace.TraceError($"Error: {ex.Message}");
                await context.PostAsync($"Ooops! Too many attempts. Don't worry, I'm trying to solve this issue, please try again.");
            }
        }

        public async Task SendGreetings(IDialogContext context, User user)
        {
            var message = context.MakeMessage();

            message.Text = $"Hi {user.Name}, how can I help you today?";
            await context.PostAsync(message);

            ShowCards(context);
        }


        private static async void ShowCards(IBotToUser context)
        {
            var message = context.MakeMessage();

            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            var list = new List<Attachment>();

            var card = new ThumbnailCard
            {
                Title = "Appointment Bookings",
                Subtitle = "Book an appointment or see your previous appointments",
                Images = new List<CardImage> { new CardImage($"{ConfigurationManager.AppSettings["BaseUrl"]}/img/BookingAppointments.png") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Appointment Bookings", null, MenuOptions.BookAppointment.ToString()) }
            };
            list.Add(card.ToAttachment());

            card = new ThumbnailCard
            {
                Title = "Hospitals Searching",
                Subtitle = "Find a hospital near you",
                Images = new List<CardImage> { new CardImage($"{ConfigurationManager.AppSettings["BaseUrl"]}/img/HospitalSearch.png") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Hospitals Searching", null, MenuOptions.SearchHospital.ToString()) }
            };
            list.Add(card.ToAttachment());

            card = new ThumbnailCard
            {
                Title = "Refund searching",
                Subtitle = "Check here your refund status",
                Images = new List<CardImage> { new CardImage($"{ConfigurationManager.AppSettings["BaseUrl"]}/img/Refund.png") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Pesquisa de reembolsos", null, MenuOptions.SearchRefund.ToString()) }
            };
            list.Add(card.ToAttachment());
            card = new ThumbnailCard
            {
                Title = "Balance Slip",
                Subtitle = "Request your balance slip here ",
                Images = new List<CardImage> { new CardImage($"{ConfigurationManager.AppSettings["BaseUrl"]}/img/Balance.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Balance Slip", null, MenuOptions.BalanceSlip.ToString()) }
            };
            list.Add(card.ToAttachment());
            card = new ThumbnailCard
            {
                Title = "Generate a barcode payment bill",
                Subtitle = "Request your bill here",
                Images = new List<CardImage> { new CardImage($"{ConfigurationManager.AppSettings["BaseUrl"]}/img/GenerateBarcodeSlip.png") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Generate a barcode payment bill", null, MenuOptions.BarcodeBillPayment.ToString()) }
            };
            list.Add(card.ToAttachment());

            message.Attachments = list;

            await context.PostAsync(message);
        }
    }
}