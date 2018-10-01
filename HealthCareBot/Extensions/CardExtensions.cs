using AdaptiveCards;
using HealthCareBot.Integration.Models;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace HealthCareBot.Extensions
{
    public static class CardExtensions
    {
        public static void AddRefundCard(this IMessageActivity instance, string nome, List<Refund> refunds)
        {
            var card = new AdaptiveCard();
            card.Body.Add(
                new AdaptiveContainer
                {
                    Items = new List<AdaptiveElement> {

                        new AdaptiveColumnSet
                     {
                        Columns = new List<AdaptiveColumn> {
                            new AdaptiveColumn
                            {
                                Items = new List<AdaptiveElement> { new AdaptiveTextBlock
                                {
                                    Text = "Number",
                                    Weight = AdaptiveTextWeight.Bolder,
                                    Size = AdaptiveTextSize.Medium
                                }}
                            },
                             new AdaptiveColumn
                            {
                                Items = new List<AdaptiveElement> { new AdaptiveTextBlock
                                {
                                    Text = "Request Date",
                                    Weight = AdaptiveTextWeight.Bolder,
                                    Size = AdaptiveTextSize.Medium
                                 } }
                            },
                              new AdaptiveColumn
                            {
                                Items = new List<AdaptiveElement> { new AdaptiveTextBlock
                                {
                                    Text = "Amount",
                                    Weight = AdaptiveTextWeight.Bolder,
                                    Size = AdaptiveTextSize.Medium
                                } }
                            },
                                new AdaptiveColumn
                            {
                                Items = new List<AdaptiveElement> { new AdaptiveTextBlock
                                {
                                    Text = "Payment date",
                                    Weight = AdaptiveTextWeight.Bolder,
                                    Size = AdaptiveTextSize.Medium
                                } }
                            }
                        }
                    }
                }
                });


            foreach (var refund in refunds)
            {
                (card.Body[0] as AdaptiveContainer)?.Items.Add(AddRefundLine(refund));
            }

            var attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            instance.Attachments.Add(attachment);
        }

        private static AdaptiveElement AddRefundLine(Refund refund)
        {
            return new AdaptiveColumnSet
            {
                Columns = new List<AdaptiveColumn> {
                            new AdaptiveColumn
                            {
                                Items = new List<AdaptiveElement> { new AdaptiveTextBlock
                                {
                                    Text = refund.Number.ToString(),
                                    Size = AdaptiveTextSize.Medium
                                } }
                            },
                            new AdaptiveColumn
                            {
                                Items = new List<AdaptiveElement> { new AdaptiveTextBlock
                                {
                                    Text =  refund.RequestDate.ToShortDateString(),
                                    Size = AdaptiveTextSize.Medium
                                 } }
                            },
                              new AdaptiveColumn
                            {
                                Items = new List<AdaptiveElement> { new AdaptiveTextBlock
                                {
                                    Text =  refund.Amount.ToString("C2"),
                                    Size = AdaptiveTextSize.Medium
                                 } }
                            },
                            new AdaptiveColumn
                            {
                                Items = new List<AdaptiveElement> { new AdaptiveTextBlock
                                {
                                   Text =  refund.PaymentDate.HasValue? refund.PaymentDate.Value.ToShortDateString() : "Pending",
                                    Size = AdaptiveTextSize.Medium
                                } }
                            }
                        }
            };
        }

        public static void AddAppointmentsCard(this IMessageActivity instance, string nome, List<Appointment> appointments)
        {
            var facts = new List<AdaptiveFact>();

            foreach (var item in appointments)
            {
                facts.Add(new AdaptiveFact
                {
                    Title = item.DoctorName,
                    Value = $"{item.DateTime.ToShortDateString()} at {item.DateTime.ToShortTimeString()}"
                });
            }

            var card = new AdaptiveCard
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = "Pending Appointments",
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Medium
                    },
                    new AdaptiveTextBlock {Text = $"**{nome}**", Wrap = true},
                    new AdaptiveContainer
                    {
                        Items = new List<AdaptiveElement>()
                        {
                            new AdaptiveTextBlock {Text = "You have the following appointments:", Wrap = true},
                            new AdaptiveFactSet {Facts = facts}
                        }
                    }
                }
            };

            var attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            instance.Attachments.Add(attachment);
        }
    }
}