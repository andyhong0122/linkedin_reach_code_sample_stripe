using Sabio.Models.Domain.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sabio.Models.Domain
{
    public class StripeCharge
    {
        public int AppointmentId { get; set; }
        public string StripeCustomerId { get; set; }
        public string ChargeId { get; set; }
        public string ReceiptUrl { get; set; }
        public string ReceiptId { get; set; }
    }
}