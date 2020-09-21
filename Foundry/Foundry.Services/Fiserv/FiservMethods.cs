using AutoMapper;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
   public class FiservMethods : IFiservMethods
    {
        private readonly IConfiguration _configuration;
        private readonly IFiservPaymentTransactionLogService _paymentTransactionLog;
        private readonly IMapper _mapper;

        public FiservMethods(IConfiguration configuration,
            IFiservPaymentTransactionLogService paymentTransactionLog, IMapper mapper
            )
        {
            _configuration = configuration;
            _paymentTransactionLog = paymentTransactionLog;
            _mapper = mapper;
    }
        public  async Task<string> fiservReverseTransaction( decimal Amount,string orderId)
        {

            using (var client = new HttpClient())
            {
                long timestamp, nonce;
                string apiKeyFD, apiSecretFD, clientRequestId;
                SettingsGenerateForReloadBalance(out timestamp, out nonce, out apiKeyFD, out apiSecretFD, out clientRequestId);

                string URLTransaction = string.Concat(_configuration["FiservReverseTransaction"], "/", orderId);
                var paymentPayload = new PayloadSecondaryTransaction()
                {
                    transactionAmount = new TransactionAmount()
                    {
                        total = Amount,
                        currency = "USD"
                    },
                    requestType = "ReturnTransaction",

                };

                string payloadJson = JsonConvert.SerializeObject(paymentPayload);

                string messageSignature;

                SetMessageHeadersSignature(timestamp, apiKeyFD, apiSecretFD, clientRequestId, payloadJson, out messageSignature);

                HttpContent stringContent = StringContentSetting(payloadJson);
                string FiservRequestCreate = string.Empty;
                RequestHeaderSetting(client, timestamp, nonce, apiKeyFD, clientRequestId, messageSignature);

                FiservRequestCreate = LogRequestCreateForTransaction(timestamp, nonce, apiKeyFD, clientRequestId, payloadJson, URLTransaction, messageSignature);
                var result = await client.PostAsync(URLTransaction, stringContent).ConfigureAwait(true);
                var transactionResult = await result.Content.ReadAsStringAsync().ConfigureAwait(true);

                FiservPaymentTransactionLogModel oModelLog = new FiservPaymentTransactionLogModel()
                {
                    //creditUserId = model.ReloadUserId,
                    //debitUserId = model.BenefactorUserId,
                    FiservRequestContent = FiservRequestCreate,
                    FiservRequestDate = DateTime.UtcNow,
                    FiservResponseContent = transactionResult,
                    FiservResponseDate = DateTime.UtcNow
                };
                var objToken = _mapper.Map<FiservPaymentTransactionLog>(oModelLog);
                await _paymentTransactionLog.AddAsync(objToken);

                return "ok";
            }

        }

        private static string LogRequestCreateForTransaction(long timestamp, long nonce, string apiKeyFD, string clientRequestId, string payloadJson, string URLTransaction, string messageSignature)
        {
            return string.Concat("Headers:{Api-key: ", apiKeyFD, ",Message-Signature: ", messageSignature, ",Timestamp: ", timestamp.ToString(), ",Client-Request-Id: ", clientRequestId, "},URL:{", URLTransaction, "}Body:{", payloadJson, "}");
        }
        private static void RequestHeaderSetting(HttpClient client, long timestamp, long nonce, string apiKeyFD, string clientRequestId, string messageSignature)
        {
            client.DefaultRequestHeaders.Add("Api-key", apiKeyFD);
            client.DefaultRequestHeaders.Add("Message-Signature", messageSignature);
          //  client.DefaultRequestHeaders.Add("Nonce", nonce.ToString());
            client.DefaultRequestHeaders.Add("Timestamp", timestamp.ToString());
            client.DefaultRequestHeaders.Add("Client-Request-Id", clientRequestId);
        }

        private static HttpContent StringContentSetting(string payloadJson)
        {
            HttpContent stringContent = new StringContent(payloadJson);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            stringContent.Headers.ContentLength = payloadJson.Length;
            return stringContent;
        }
        private void SettingsGenerateForReloadBalance(out long timestamp, out long nonce, out string apiKeyFD, out string apiSecretFD, out string clientRequestId)
        {
            Random generator = new Random();
            var num = generator.Next(0, 9999).ToString("D" + 4);
            timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            nonce = timestamp + Convert.ToInt16(num);
            apiKeyFD = _configuration["credentials:apiKey"];
            apiSecretFD = _configuration["credentials:apiSecret"];
            clientRequestId = Guid.NewGuid().ToString();
        }

        private void SetMessageHeadersSignature(long timestamp, string apiKeyFD, string apiSecretFD, string clientRequestId, string payloadJson, out string messageSignature)
        {
            var message = string.Concat(apiKeyFD, clientRequestId, timestamp, payloadJson);
            messageSignature = Sign(apiSecretFD, message);


        }

        private string Sign(string apiSecret, String payload = "")
        {
            UTF8Encoding encoder = new UTF8Encoding();

            // Create signature message
            string message = payload;
            byte[] secretKeyBytes = encoder.GetBytes(apiSecret);
            byte[] messageBytes = encoder.GetBytes(message);

            // Perform hashing
            HMACSHA256 hmac = new HMACSHA256(secretKeyBytes);
            byte[] hmacBytes = hmac.ComputeHash(messageBytes);
            String hexHmac = ByteArrayToString(hmacBytes);

            // Convert to Base64
            byte[] hexBytes = encoder.GetBytes(hexHmac);
            String signature = Convert.ToBase64String(hexBytes);
            return signature;
        }

        private string ByteArrayToString(byte[] input)
        {
            int i;
            StringBuilder output = new StringBuilder(input.Length);
            for (i = 0; i < input.Length; i++)
            {
                output.Append(input[i].ToString("x2"));
            }
            return output.ToString();
        }


    }
}
