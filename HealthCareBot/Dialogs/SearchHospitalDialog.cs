using HealthCareBot.Factories;
using HealthCareBot.Integration.Models;
using HealthCareBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Builder.Location.Bing;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace HealthCareBot.Dialogs
{
    [Serializable]

    public class SearchHospitalDialog : IDialog<List<Hospital>>
    {
        private const string SHARE_LOCATION_OPTION = "Share location";
        private const string POST_CODE_OPTION = "Post code";

        public async Task StartAsync(IDialogContext context)
        {
            await ShowLocationOptions(context);
            context.Wait(MessageReceivedAsync);
        }

        private static async Task ShowLocationOptions(IDialogContext context)
        {
            var message = context.MakeMessage();
            message.Text = "I need you to inform your location in order to find a hospital neear you. How do you wanna do?";
            await context.PostAsync(message);
            ShowCards(context);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {

            var activity = await result as IMessageActivity;

            switch (activity.Text)
            {
                case SHARE_LOCATION_OPTION:
                    await ShareLocation(context);
                    context.Wait(LocationReceiveAsync);
                    break;
                case POST_CODE_OPTION:
                    var form = FormDialog.FromForm(BuildFormAskingMedicalSpecialtyAndPostCode, FormOptions.PromptInStart);
                    context.Call(form, AfterFormFilled);
                    break;
                default:
                    break;
            }



        }

        private static async Task ShareLocation(IBotToUser context)
        {
            await context.PostAsync("So please share your location and I will find some hospitals near you");
        }
        private async Task LocationReceiveAsync(IDialogContext context, IAwaitable<object> result)
        {
            if (await result is Activity activity && activity.Entities.Count > 0)
            {
                var latitude = activity.Entities[0].Properties.Root["geo"]["latitude"].ToString();
                var longitude = activity.Entities[0].Properties.Root["geo"]["longitude"].ToString();

                var query = new SearchHospitalQuery
                {
                    Latitude = latitude,
                    Longitude = longitude
                };

                var form = new FormDialog<SearchHospitalQuery>(query, BuildFormAskingMedicalSpecialty, FormOptions.PromptInStart);
                context.Call(form, AfterFormFilled);
            }
            else
            {
                PromptDialog.Confirm(context, RetryShareLocation,
                    "I couldn't get your location. Do you wanna try again?", "It's not a valid option");
            }
        }


        private IForm<SearchHospitalQuery> BuildFormAskingMedicalSpecialty()
        {
            var form = new FormBuilder<SearchHospitalQuery>()
                .Field(nameof(SearchHospitalQuery.MedicalSpecialty))
                       .Build();
            return form;
        }
        private IForm<SearchHospitalQuery> BuildFormAskingMedicalSpecialtyAndPostCode()
        {
            var form = new FormBuilder<SearchHospitalQuery>()
                .Field(nameof(SearchHospitalQuery.Postcode))
                .Field(nameof(SearchHospitalQuery.MedicalSpecialty))
                       .Build();
            return form;
        }


        private async Task RetryShareLocation(IDialogContext context, IAwaitable<bool> result)
        {
            try
            {
                var resposta = await result;
                if (resposta)
                {
                    await ShareLocation(context);
                    context.Wait(LocationReceiveAsync);
                }
                else
                {
                    await ShowLocationOptions(context);
                    context.Wait(MessageReceivedAsync);
                }
            }
            catch (TooManyAttemptsException ex)
            {
                System.Diagnostics.Trace.TraceError($"Erro: {ex.Message}");
                await context.PostAsync($"Ooops! Too many attempts. Don't worry, I'm trying to solve this issue, please try again.");

            }
        }

        private async Task AfterFormFilled(IDialogContext context, IAwaitable<SearchHospitalQuery> result)
        {
            var searchHospitalQuery = await result;
            var service = HospitalSearchServiceFactory.Create();
            List<Hospital> hospitals= null;
            hospitals = !string.IsNullOrWhiteSpace(searchHospitalQuery.Postcode) 
                ? service.SearchByPostcode(searchHospitalQuery.Postcode, searchHospitalQuery.MedicalSpecialty) 
                : service.SearchByCoordinates(searchHospitalQuery.Latitude, searchHospitalQuery.Longitude, searchHospitalQuery.MedicalSpecialty);

            if (hospitals.Count == 0)
            {
                await context.PostAsync("Unfortunately, I couldn't find any hospital near you");
            }
            else
            {
                await ShowLocations(context, hospitals);
            }
            context.Done(hospitals);
        }

        private async Task ShowLocations(IDialogContext context, List<Hospital> hospitals)
        {
            var message = context.MakeMessage();
            var apiKey = WebConfigurationManager.AppSettings["BingMapsApiKey"];
            var rm = new LocationResourceManager();
            var cardBuilder = new LocationCardBuilder(apiKey, rm);
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            var cards = cardBuilder.CreateHeroCards(ParseLocations(hospitals), locationNames: ParseHospitalNames(hospitals));

            foreach (var card in cards)
            {
                message.Attachments.Add(card.ToAttachment());
            }

            await context.PostAsync(message);
        }

        private IList<string> ParseHospitalNames(List<Hospital> hospitals)
        {
            var hospitalNames = new List<string>();
            foreach (var hospital in hospitals)
            {
                hospitalNames.Add(hospital.Name);
            }
            return hospitalNames;
        }

        private static IList<Location> ParseLocations(IEnumerable<Hospital> hospitals)
        {
            return hospitals.Select(hospital => new Location
                {
                    Address = new Microsoft.Bot.Builder.Location.Bing.Address
                    {
                        FormattedAddress = hospital.Address,
                        AddressLine = hospital.Address
                    },
                    GeocodePoints = new List<GeocodePoint>
                    {
                        new GeocodePoint {Coordinates = new List<double> {hospital.Latitude, hospital.Longitude}}
                    },
                    Name = hospital.Name,
                    Point = new GeocodePoint {Coordinates = new List<double> {hospital.Latitude, hospital.Longitude}}
                })
                .ToList();
        }

        private static async void ShowCards(IDialogContext context)
        {
            var message = context.MakeMessage();
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            ThumbnailCard card;
            var list = new List<Attachment>();
            switch (context.Activity.ChannelId)
            {
                case "facebook":
                    card = new ThumbnailCard
                    {
                        Title = "Share Location",
                        Subtitle = "Share your current location through facebook",
                        Images = new List<CardImage> { new CardImage($"{ConfigurationManager.AppSettings["BaseUrl"]}/img/Facebook.png") },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Share", null, "Share") }
                    };
                    list.Add(card.ToAttachment());
                    break;
                case "telegram":
                    card = new ThumbnailCard
                    {
                        Title = "Share Location",
                        Subtitle = "Share your current location through telegram",
                        Images = new List<CardImage> { new CardImage($"{ConfigurationManager.AppSettings["BaseUrl"]}/img/Telegram.jpg") },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Share", null, SHARE_LOCATION_OPTION) }
                    };
                    list.Add(card.ToAttachment());
                    break;
            }

            card = new ThumbnailCard
            {
                Title = "Postcode",
                Subtitle = "Inform the Postcode where you wanna find hospitals near",
                Images = new List<CardImage> { new CardImage($"{ConfigurationManager.AppSettings["BaseUrl"]}/img/Postcode.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Postcode", null, POST_CODE_OPTION) }
            };
            list.Add(card.ToAttachment());

            message.Attachments = list;

            await context.PostAsync(message);
        }
    }
}