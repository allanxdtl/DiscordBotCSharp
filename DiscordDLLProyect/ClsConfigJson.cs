using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDLLProyect
{
    public struct ClsConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; private set; }

        [JsonProperty("host")]
        public string Host { get; private set; }

        [JsonProperty("port")]
        public int Port { get; private set; }

        [JsonProperty("password")]
        public string Password { get; private set; }

    }
}
