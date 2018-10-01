using HealthCareBot.Factories;
using HealthCareBot.Integration.Interfaces;
using HealthCareBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace HealthCareBot.Dialogs
{
    [Serializable]

    public class BalanceSlipDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var unused = await result as IMessageActivity;

            var form = FormDialog.FromForm(BuildForm, FormOptions.PromptInStart);
            context.Call(form, AfterFormFilled);

        }

        private async Task AfterFormFilled(IDialogContext context, IAwaitable<BalanceSlipQuery> result)
        {
            var query = await result;

            var user = context.UserData.GetValue<User>("user");
            var repo = UserRepositoryFactory.CreateUserRepository();
            var fileUrl = repo.RequestBalanceSlip(user.Number, user.AccessKey, query.Year);

            var message = context.MakeMessage();
            message.Attachments.Add(new Attachment
            {
                ContentUrl = fileUrl,
                ContentType = "image/jpg"
            });

            await context.PostAsync(message);
            context.Done<object>(null);
        }

        private IForm<BalanceSlipQuery> BuildForm()
        {
            return new FormBuilder<BalanceSlipQuery>()
                .Field(nameof(BalanceSlipQuery.Year))
                .Build();
        }
    }
}