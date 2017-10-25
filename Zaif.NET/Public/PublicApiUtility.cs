using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZaifNet.Public
{
    public class PublicApiUtility
    {
        private HttpClient client;

        public PublicApiUtility()
        {
            this.client = new HttpClient();
        }

        public async Task<DepthResponse> GetDepth(string currencyPair) => JsonConvert.DeserializeObject<DepthResponse>(await this.client.GetStringAsync("https://api.zaif.jp/api/1/depth/" + currencyPair));

        public async Task<TradeResponse[]> GetTrades(string currencyPair) => JsonConvert.DeserializeObject<TradeResponse[]>(await this.client.GetStringAsync("https://api.zaif.jp/api/1/trades/" + currencyPair));
    }
}
