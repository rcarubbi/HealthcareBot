using Microsoft.Bot.Builder.FormFlow;
using System;

namespace HealthCareBot.Models
{
    [Serializable]

    public class BillPaymentQuery
    {
        [Prompt("now tell me the year")]
        public int Year { get; set; }

        [Prompt("Right, to generate the bill I need you to tell me what's the reference month")]
        public int Month { get; set; }
    }
}