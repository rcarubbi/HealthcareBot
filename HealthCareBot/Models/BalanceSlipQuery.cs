using Microsoft.Bot.Builder.FormFlow;
using System;

namespace HealthCareBot.Models
{
    [Serializable]

    public class BalanceSlipQuery
    {
        [Prompt("Inform the reference year")]
        public int Year { get; set; }
    }
}