using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Data;
using Data.Providers;
using Models.AppSettings;
using Models.Domain;
using Models.Domain.Provider;
using Models.Requests.Payments;
using Services.Interfaces;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private static StripeKeys _stripeKeys;
        IDataProvider _data = null;

        public PaymentService(IDataProvider data, IOptions<StripeKeys> stripeKeys)
        {
            _data = data;
            _stripeKeys = stripeKeys.Value;
        }


        public StripeCharge CreateCharge(ChargeCreateRequest model, int userId)
        {
            StripeConfiguration.ApiKey = _stripeKeys.SecretKey;

            PaymentRequest accountType = null;
            StripeCharge chargeResponse = null;

            (string cardBrand, string tokenId) = GetTokenId(model);
            string customerId = GetCustomerId(model, tokenId);

            var chargeOptions = new ChargeCreateOptions
            {
                Amount = model.Amount,
                Currency = model.Currency,
                Customer = customerId,
                ReceiptEmail = model.Email
            };
            ChargeService service = new ChargeService();
            Charge charge = service.Create(chargeOptions);

            Console.WriteLine(charge);

            string chargeAsJson2 = JsonConvert.SerializeObject(charge);
            string chargeAsJson = JsonConvert.DeserializeObject(chargeAsJson2).ToString();

            if (accountType == null)
            {
                accountType = new PaymentRequest();
            }

            if (accountType != null)
            {
                accountType.CustomerId = charge.CustomerId;
                accountType.PaymentType = charge.PaymentMethodDetails.Card.Brand;
                accountType.ReceiptId = charge.ReceiptNumber;
                accountType.ChargeId = charge.Id;
                accountType.ReceiptUrl = charge.ReceiptUrl;
                accountType.ChargeResponse = chargeAsJson;
                accountType.ProviderId = model.ProviderId;
                accountType.Id = model.Id;

                chargeResponse = AddPayment(accountType, userId);
            }
            return chargeResponse;
        }


        public StripeCharge AddPayment(PaymentRequest model, int userId)
        {
            DataTable serviceMappedValues = MapBatchServices(model.Id);
            StripeCharge aCharge = new StripeCharge();

            string procName = "dbo.[PaymentAccounts_Insert_V2]";

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@CustomerId", model.CustomerId);
                col.AddWithValue("@ReceiptId", model.ReceiptId ??= "Available via Receipt Url");
                col.AddWithValue("@ChargeId", model.ChargeId);
                col.AddWithValue("@ReceiptUrl", model.ReceiptUrl);
                col.AddWithValue("@PaymentTypeId", FilterPaymentType(model.PaymentType));
                col.AddWithValue("@ChargeResponse", model.ChargeResponse);
                col.AddWithValue("@CreatedBy", userId);
                col.AddWithValue("@ProviderId", model.ProviderId);
                col.AddWithValue("@ChosenServicesBatch", serviceMappedValues);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);

            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                    aCharge = Mapper(reader);
            });
            return aCharge;
        }


        private static DataTable MapBatchServices(List<ProviderService> services)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProviderServiceId", typeof(int));

            foreach (ProviderService serviceInfo in services)
            {
                DataRow dataRow = dt.NewRow();
                dataRow.SetField(0, serviceInfo.Id);
                dt.Rows.Add(dataRow);
            }
            return dt;
        }


        private static int FilterPaymentType(string cardBrand)
        {
            string PaymentType = cardBrand;
            int PaymentTypeId = 0;

            switch (PaymentType.ToLower())
            {
                case "visa":
                    PaymentTypeId = 2;
                    break;
                case "mastercard":
                    PaymentTypeId = 3;
                    break;
                case "discover":
                    PaymentTypeId = 7;
                    break;
                case "american express":
                    PaymentTypeId = 8;
                    break;
            }
            return PaymentTypeId;
        }


        private static (string, string) GetTokenId(ChargeCreateRequest model)
        {
            var tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = model.CardNumber,
                    ExpMonth = model.ExpMonth,
                    ExpYear = model.ExpYear,
                    Cvc = model.Cvc
                },
            };

            var tokenService = new Stripe.TokenService();
            var stripeToken = tokenService.Create(tokenOptions);

            return (stripeToken.Card.Brand, stripeToken.Id);
        }


        private static string GetCustomerId(ChargeCreateRequest model, string tokenId)
        {
            var customerOptions = new CustomerCreateOptions
            {
                Name = model.Name,
                Email = model.Email,
                Source = tokenId,
            };
            var customerService = new CustomerService();
            Customer customer = customerService.Create(customerOptions);

            return customer.Id;
        }

        private static StripeCharge Mapper(IDataReader reader)
        {
            StripeCharge aCharge = new StripeCharge();
            int startingIndex = 0;

            aCharge.Id = reader.GetSafeInt32(startingIndex++);
            aCharge.CustomerId = reader.GetSafeString(startingIndex++);
            aCharge.ReceiptId = reader.GetSafeString(startingIndex++);
            aCharge.ChargeId = reader.GetSafeString(startingIndex++);
            aCharge.ReceiptUrl = reader.GetSafeString(startingIndex++);
            aCharge.CreatedBy = reader.GetSafeInt32(startingIndex++);
            aCharge.ProviderId = reader.GetSafeInt32(startingIndex++);
            aCharge.DateCreated = reader.GetSafeDateTime(startingIndex++);

            return aCharge;
        }
    }
}
