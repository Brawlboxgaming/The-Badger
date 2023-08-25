using Newtonsoft.Json;

namespace Badger.Class
{
    public class BotConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
    }
}
