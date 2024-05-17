using NerdvanaCrawler.Code.Config;
using Newtonsoft.Json;

namespace NerdvanaCrawler.Code.Nerdvana
{
    [JsonConfigFile("Resources/nerdvanaSheetsCollectionConfig.json")]
    public class NerdvanaSheetsCollectionConfig : JsonConfigFile<NerdvanaSheetsCollectionConfig>
    {
        [JsonProperty("sheets")]
        public string[] Sheets { get; private set; }
    }
}
