namespace NerdvanaCrawler.Code.ConsoleUI
{
    public abstract class SingleChoiceObjectConsoleMenu<TObject> : ConsoleMenu
    {
        private TaskCompletionSource<TObject> m_TaskCompletion;

        public SingleChoiceObjectConsoleMenu(params TObject[] objects) : base() =>
            AddObjects(objects);

        public SingleChoiceObjectConsoleMenu(IReadOnlyList<TObject> objects) : base() =>
            AddObjects(objects);

        private void AddObjects(IReadOnlyList<TObject> objects)
        {
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
            m_TaskCompletion = new TaskCompletionSource<TObject>();
            base.Enable();
        }

        private void ChooseObject(TObject obj)
        {
            m_TaskCompletion.SetResult(obj);
        }

        public Task<TObject> WaitForSelection() => m_TaskCompletion.Task;
    }
}
