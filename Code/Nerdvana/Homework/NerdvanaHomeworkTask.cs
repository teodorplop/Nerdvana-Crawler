namespace NerdvanaCrawler.Code.Nerdvana.Homework
{
    public struct NerdvanaHomeworkTask
    {
        public string Name { get; }
        public string Url { get; }
        public int SheetColumnIndex { get; }
        public NerdvanaHomeworkTaskGroup TaskGroup { get; }

        public NerdvanaHomeworkTask(string name, string url, int sheetColumnIndex, NerdvanaHomeworkTaskGroup taskGroup)
        {
            Name = name;
            Url = url;
            SheetColumnIndex = sheetColumnIndex;
            TaskGroup = taskGroup;
        }

        public override string ToString() => Name;
    }
}
