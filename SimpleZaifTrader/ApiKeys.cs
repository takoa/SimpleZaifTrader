using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleZaifTrader
{
    public class ApiKeys
    {
        [JsonProperty("ApiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("Secret")]
        public string SecretKey { get; set; }

        public ApiKeys()
        {
        }

        public static ApiKeys Read(string uri)
        {
            using (StreamReader reader = new StreamReader(uri))
            {
                string json = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<ApiKeys>(json);
            }
        }
    }
}
