namespace NerdvanaCrawler.Code.ConsoleUI
{
    public class ConsoleInputAction
    {
        public string Message { get; }
        public Action Action { get; }

        public ConsoleInputAction(Action action) : this(action.Method.Name, action) { }

        public ConsoleInputAction(string message, Action action)
        {
            Message = message;
            Action = action;
        }
    }
}
