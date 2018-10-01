using System;

namespace HealthCareBot.Integration.Models
{
    public class Refund
    {
        public int Number { get; set; }

        public decimal Amount { get; set; }

        public DateTime RequestDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        
    }
}