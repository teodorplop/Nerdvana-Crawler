using NerdvanaCrawler.Code.ConsoleUI;

namespace NerdvanaCrawler.Code.Nerdvana.UI
{
    public class ChooseHomeworkMenu : SingleChoiceObjectConsoleMenu<string>
    {
        protected override string Title => "Choose homework to evaluate";

        public ChooseHomeworkMenu(IReadOnlyList<string> homeworks, Arguments args) : base(homeworks) { }
    }
}
