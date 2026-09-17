using NerdvanaCrawler.Code.Utils;

namespace NerdvanaCrawler.Code.ConsoleUI
{
    public abstract class ConsoleMenu
    {
        private enum EAlignment { Left, Center, Right }

        private const int K_LINE_LENGTH = 100;

        private const char K_LINE_START = '|';
        private const char K_LINE_SEPARATOR = '-';
        private const char K_SPACE = ' ';

        private static readonly string K_SEPARATOR_LINE = new string(K_LINE_SEPARATOR, K_LINE_LENGTH);
        private static readonly string K_EMPTY_LINE = $"{K_LINE_START}{new string(K_LINE_SEPARATOR, K_LINE_LENGTH - 2)}{K_LINE_START}";

        private const string K_SELECT_OPTION_STRING = "Please input one of the following:";

        private bool m_AutoPlaySingleAction;
        private List<ConsoleInputAction> m_Actions;

        protected abstract string Title { get; }

        protected ConsoleMenu(bool autoPlaySingleAction, params ConsoleInputAction[] actions)
        {
            m_AutoPlaySingleAction = autoPlaySingleAction;
            SetActions(actions);
        }

        protected ConsoleMenu(IReadOnlyList<ConsoleInputAction> actions, bool autoPlaySingleAction)
        {
            m_AutoPlaySingleAction = autoPlaySingleAction;
            SetActions(actions);
        }

        private void SetActions(IReadOnlyList<ConsoleInputAction> actions)
        {
            m_Actions = actions != null ? new List<ConsoleInputAction>(actions) : new List<ConsoleInputAction>();
        }

        private void Separator() => Console.WriteLine(K_SEPARATOR_LINE);
        private void Space() => Console.WriteLine(K_EMPTY_LINE);
        private void Label(string text, EAlignment alignment = EAlignment.Center)
        {
            int spaceRemaining = K_LINE_LENGTH - 2 - text.Length;
            int spacesLeft = 0;
            int spacesRight = 0;

            switch (alignment)
            {
                case EAlignment.Left:
                    spacesLeft = 0;
                    spacesRight = spaceRemaining;
                    break;
                case EAlignment.Right:
                    spacesRight = 0;
                    spacesLeft = spaceRemaining;
                    break;
                case EAlignment.Center:
                    spacesLeft = (spaceRemaining + 1) / 2;
                    spacesRight = spaceRemaining / 2;
                    break;
            }

            Console.Write(K_LINE_START);
            if (spacesLeft > 0)
                Console.Write(new string(K_SPACE, spacesLeft));
            Console.Write(text);
            if (spacesRight > 0)
                Console.Write(new string(K_SPACE, spacesRight));
            Console.WriteLine(K_LINE_START);
        }

        private void DisplayTitle() => Label(Title);

        private void DisplayActions()
        {
            Label(K_SELECT_OPTION_STRING, EAlignment.Left);
            for (int i = 0; i < m_Actions.Count; ++i)
                Label($"{i}. {m_Actions[i].Message}", EAlignment.Left);
        }

        protected virtual void WaitForInput()
        {
            if (m_AutoPlaySingleAction && m_Actions.Count == 1)
            {
                m_Actions[0].Action?.Invoke();
                return;
            }
            
            int selectedIndex;
            do
            {
                var input = Console.ReadLine();
                if (input == null)
                    throw new InvalidOperationException($"{Title}: no console input available. Pass command line arguments to select an option when running without a console.");

                if (!int.TryParse(input, out selectedIndex) || selectedIndex < 0 || selectedIndex >= m_Actions.Count)
                {
                    this.LogError($"Failed to parse input: expected a number between 0 and {m_Actions.Count - 1}, got '{input}'.");
                    selectedIndex = -1;
                }
            } while (selectedIndex == -1);

            m_Actions[selectedIndex].Action?.Invoke();
        }

        protected void AddAction(ConsoleInputAction action)
        {
            m_Actions.Add(action);
        }

        public virtual void Enable()
        {
            Separator();
            DisplayTitle();
            Separator();
            DisplayActions();
            Separator();

            WaitForInput();
        }

        public virtual void Disable()
        {
        }
    }
}
