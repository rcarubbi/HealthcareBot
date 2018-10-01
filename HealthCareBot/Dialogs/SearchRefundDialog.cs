using HealthCareBot.Extensions;
using HealthCareBot.Factories;
using HealthCareBot.Integration.Interfaces;
using HealthCareBot.Integration.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthCareBot.Dialogs
{
    [Serializable]

    public class SearchRefundDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var unused = await result as IMessageActivity;

            var repo = UserRepositoryFactory.CreateUserRepository();
            var user = context.UserData.GetValue<User>("user");
            var refunds = repo.SearchRefunds(user.Number, user.AccessKey);
            await ShowRefunds(context, refunds);
            context.Done<object>(null);
        }

        private static async Task ShowRefunds(IBotContext context, List<Refund> refunds)
        {
            if (refunds.Count == 0)
            {
                await context.PostAsync("You don't have any pending refund");
            }
            else
            {
                var message = context.MakeMessage();
                var user = context.UserData.GetValue<User>("user");
                message.AddRefundCard(user.Name, refunds);
                await context.PostAsync(message);
            }
        }
    }
}
