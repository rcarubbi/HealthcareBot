using Microsoft.Bot.Builder.FormFlow;
using System;

namespace HealthCareBot.Models
{
    [Serializable]
    public class Credentials
    {
        [Describe("Member's card number")]
        [Prompt("Please, inform your member's card number")]
        public string Number { get; set; }


        [Describe("Access Key")]
        [Prompt("Now inform your access key")]
        public string AccessKey { get; set; }
    }
}