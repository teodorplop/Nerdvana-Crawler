namespace NerdvanaCrawler.Code.Config
{
    public class JsonConfigFileAttribute : Attribute
    {
        public string FilePath { get; }

        public JsonConfigFileAttribute(string filePath)
        {
            FilePath = filePath;
        }
    }
}
