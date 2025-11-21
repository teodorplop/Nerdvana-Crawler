namespace NerdvanaCrawler.Code.ConsoleUI
{
    public abstract class MultipleChoiceObjectConsoleMenu<TObject> : ConsoleMenu
    {
        private const string K_ALL_OBJECTS_STRING = "All";

        private IReadOnlyList<TObject> m_Objects;
        private TaskCompletionSource<IReadOnlyList<TObject>> m_TaskCompletion;

        public MultipleChoiceObjectConsoleMenu(params TObject[] objects) : base(false) =>
            AddObjects(objects);

        public MultipleChoiceObjectConsoleMenu(IReadOnlyList<TObject> objects) : base(false) =>
            AddObjects(objects);

        private void AddObjects(IReadOnlyList<TObject> objects)
        {
            m_Objects = objects;

            AddAction(new ConsoleInputAction(K_ALL_OBJECTS_STRING, ChooseAllObjects));
            if (objects != null)
            {
                foreach (TObject obj in objects)
                {
                    string message = obj?.ToString();
                    AddAction(new ConsoleInputAction(message, () => ChooseObject(obj)));
                }
            }
        }

        public override void Enable()
        {
            m_TaskCompletion = new TaskCompletionSource<IReadOnlyList<TObject>>();
            base.Enable();
        }

        private void ChooseAllObjects()
        {
            m_TaskCompletion.SetResult(m_Objects);
        }
        private void ChooseObject(TObject obj)
        {
            m_TaskCompletion.SetResult(new[] {obj});
        }

        public Task<IReadOnlyList<TObject>> WaitForSelection() => m_TaskCompletion.Task;
    }
}
