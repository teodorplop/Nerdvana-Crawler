namespace NerdvanaCrawler.Code.Nerdvana.Homework
{
    public class NerdvanaHomeworkTaskGroup
    {
        protected List<NerdvanaHomeworkTask> m_Tasks = new List<NerdvanaHomeworkTask>();

        public string Name { get; }
        public IReadOnlyList<NerdvanaHomeworkTask> Tasks => m_Tasks;
        public int SheetTotalColumnIndex { get; protected set; } = -1;

        public NerdvanaHomeworkTaskGroup(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }

    public class EditableNerdvanaTaskGroup : NerdvanaHomeworkTaskGroup
    {
        public EditableNerdvanaTaskGroup(string name) : base(name) { }

        public void AddTask(NerdvanaHomeworkTask task)
        {
            m_Tasks.Add(task);
        }

        public void SetSheetTotalColumnIndex(int totalColumnIndex)
        {
            SheetTotalColumnIndex = totalColumnIndex;
        }
    }
}
