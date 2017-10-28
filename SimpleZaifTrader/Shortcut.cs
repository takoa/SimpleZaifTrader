using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleZaifTrader
{
    public class Shortcut
    {
        [JsonProperty("Keys")]
        public Key[] Keys { get; set; }

        [JsonProperty("CommandName")]
        public string CommandName { get; set; }

        public bool IsHit
        {
            get
            {
                for (int i = 0; i < this.Keys.Length; i++)
                {
                    if (!Keyboard.IsKeyDown(this.Keys[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
