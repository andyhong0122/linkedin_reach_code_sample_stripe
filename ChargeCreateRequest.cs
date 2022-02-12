using Models.Domain;
using Models.Domain.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Requests.Payments
{
    public class ChargeCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [CreditCard]
        public string CardNumber { get; set; }
        [Required]
        public int ExpMonth { get; set; }
        [Required]
        public int ExpYear { get; set; }
        [Required]
        public string Cvc { get; set; }
        [Required]
        public int Amount { get; set; } // Stripe API takes in a whole number ( 9999 for $99.99 ) so the Amount is set as integer
        [Required]
        public string Currency { get; set; }
        [Required]
        public int ProviderId { get; set; }
        [Required]
        public List<ProviderService> Id { get;set;} 
    }
}