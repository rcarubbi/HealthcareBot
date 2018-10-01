using HealthCareBot.Factories;
using HealthCareBot.Integration.Interfaces;
using HealthCareBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace HealthCareBot.Dialogs
{
    public class BarcodeBillPaymentDialog : IDialog<object>
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

        private static async Task AfterFormFilled(IDialogContext context, IAwaitable<BillPaymentQuery> result)
        {
            var query = await result;

            var user = context.UserData.GetValue<User>("user");
            var repo = UserRepositoryFactory.CreateUserRepository();
            var urlArquivo = repo.GenerateBarcodeBill(user.Number, user.AccessKey, query.Year, query.Month);

            var message = context.MakeMessage();
            message.Attachments.Add(new Attachment
            {
                ContentUrl = urlArquivo,
                ContentType = "image/jpg"
            });

            await context.PostAsync(message);
            context.Done<object>(null);
        }

        private IForm<BillPaymentQuery> BuildForm()
        {
            return new FormBuilder<BillPaymentQuery>()
                .Field(nameof(BillPaymentQuery.Month))
                .Field(nameof(BillPaymentQuery.Year))
                .Build();
        }
    }
}