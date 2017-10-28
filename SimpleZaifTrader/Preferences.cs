using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleZaifTrader
{
    public class Preferences
    {
        [JsonProperty("Shortcuts")]
        public Shortcut[] Shortcuts { get; set; }
    }
}
