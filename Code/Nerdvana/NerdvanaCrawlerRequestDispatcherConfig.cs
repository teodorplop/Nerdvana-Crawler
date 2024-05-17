using NerdvanaCrawler.Code.Config;
using Newtonsoft.Json;

namespace NerdvanaCrawler.Code.Nerdvana
{
    [JsonConfigFile("Resources/nerdvanaCrawlerRequestDispatcherConfig.json")]
    public class NerdvanaCrawlerRequestDispatcherConfig : JsonConfigFile<NerdvanaCrawlerRequestDispatcherConfig>
    {
        [JsonProperty("min_delay")]
        public int MinDelay { get; private set; } = 1000;

        [JsonProperty("max_delay")]
        public int MaxDelay { get; private set; } = 1500;
    }
}
