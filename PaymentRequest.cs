using Models.Domain;
using Models.Domain.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Requests.Payments
{
    public class PaymentRequest
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public string PaymentType { get; set; }
        [Required]
        public string ReceiptUrl { get; set; }
        [Required]
        public string ChargeId { get; set; }
        [Required]
        public string ReceiptId { get; set; }
        [Required]
        public string ChargeResponse { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public int ProviderId { get; set; }
        [Required]
        public List<ProviderService> Id { get; set; }
    }
}