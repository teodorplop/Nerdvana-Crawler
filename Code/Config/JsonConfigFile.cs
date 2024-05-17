using NerdvanaCrawler.Code.Utils;
using Newtonsoft.Json;
using System.Reflection;

namespace NerdvanaCrawler.Code.Config
{
    public abstract class JsonConfigFile<TJsonConfigFile>
    {
        private static TJsonConfigFile m_Instance;

        public static TJsonConfigFile Instance
        {
            get { return m_Instance ??= LoadOrCreateInstance(); }
        }

        private static TJsonConfigFile LoadOrCreateInstance()
        {
            var attribute = typeof(TJsonConfigFile).GetCustomAttribute<JsonConfigFileAttribute>();
            TJsonConfigFile configFile;

            if (attribute != null)
            {
                try
                {
                    string text = File.ReadAllText(attribute.FilePath);
                    configFile = JsonConvert.DeserializeObject<TJsonConfigFile>(text);
                    typeof(TJsonConfigFile).Log("Successfully loaded config file.");
                }
                catch (Exception ex)
                {
                    typeof(TJsonConfigFile).LogError($"Failed to deserialize config file. Will fallback to default one.\nException: {ex.Message}");
                    configFile = CreateInstance();
                }
            }
            else
            {
                typeof(TJsonConfigFile).LogError($"Missing {nameof(JsonConfigFileAttribute)}");
                configFile = CreateInstance();
            }

            return configFile;
        }

        private static TJsonConfigFile CreateInstance()
        {
            return Activator.CreateInstance<TJsonConfigFile>();
        }
    }
}
