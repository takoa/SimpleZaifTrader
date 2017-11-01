using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZaifNet.Trade
{
    public class TradeApiUtility
    {
        private static readonly string baseUri = "https://api.zaif.jp/tapi";

        private HttpClient client;
        private string apiKey;
        private string secretKey;
        private HMACSHA512 authenticator;

        public TradeApiUtility(string apiKey, string secretKey)
        {
            this.client = new HttpClient() { BaseAddress = new Uri("https://api.zaif.jp") };
            this.apiKey = apiKey;
            this.secretKey = secretKey;
            this.authenticator = new HMACSHA512(Encoding.UTF8.GetBytes(apiKey));
        }

        public async Task<TradeResponse> PostTrade(string currencyPair, string action, decimal price, decimal amount, decimal? limit, string comment)
        {
            if (currencyPair == null)
            {
                throw new ArgumentNullException("currencyPair");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "currency_pair", currencyPair },
                { "action", action },
                { "price", price.ToString() },
                { "amount", amount.ToString() }
            };

            if (limit != null)
            {
                parameters.Add("limit", limit.ToString());
            }
            if (comment != null)
            {
                parameters.Add("comment", comment);
            }

            return JsonConvert.DeserializeObject<TradeResponse>(await this.Post("trade", parameters));
        }

        public async Task<CancelOrderResponse> PostCancelOrder(string currencyPair, long id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "order_id", id.ToString() },
                { "currency_pair", currencyPair }
            };

            string str = await this.Post("cancel_order", parameters);

            return JsonConvert.DeserializeObject<CancelOrderResponse>(str);
        }

        public async Task<GetInfo2Response> PostGetInfo2() => JsonConvert.DeserializeObject<GetInfo2Response>(await this.Post("get_info2", null));

        public async Task<ActiveOrdersResponse> PostActiveOrders(string currencyPair)
        {
            if (currencyPair == null)
            {
                throw new ArgumentNullException("currencyPair");
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "currency_pair", currencyPair }
            };
            string receivedJson = await this.Post("active_orders", parameters);
            string json;

            if (receivedJson.IndexOf(@"""return"": {}") < 0)
            {
                json = Regex.Replace(receivedJson, @"""(?<id>\d+)"": {", @"{ ""id"": ${id}, ", RegexOptions.IgnoreCase | RegexOptions.Singleline)
                        .Replace(@"""return"": {", @"""return"": [")
                        .Replace(@"}}}", @"}]}");

                return JsonConvert.DeserializeObject<ActiveOrdersResponse>(json);
            }
            else
            {
                return new ActiveOrdersResponse() { Success = 1, Return = null };
            }
        }

        private async Task<string> Post(string methodName, Dictionary<string, string> parameters)
        {
            FormUrlEncodedContent content;
            HttpResponseMessage response;

            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }

            parameters.Add("nonce", (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString());
            parameters.Add("method", methodName);
            content = new FormUrlEncodedContent(parameters);

            this.client.DefaultRequestHeaders.Clear();
            this.client.DefaultRequestHeaders.Add("key", this.apiKey);
            this.client.DefaultRequestHeaders.Add("sign", 
                BitConverter.ToString(
                    new HMACSHA512(Encoding.UTF8.GetBytes(this.secretKey)).ComputeHash(
                        Encoding.UTF8.GetBytes(await content.ReadAsStringAsync())))
                    .ToLower().Replace("-", ""));

            response = await this.client.PostAsync(TradeApiUtility.baseUri, content);

            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : string.Empty;
        }
    }
}
