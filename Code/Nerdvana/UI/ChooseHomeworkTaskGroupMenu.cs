using NerdvanaCrawler.Code.ConsoleUI;
using NerdvanaCrawler.Code.Nerdvana.Homework;

namespace NerdvanaCrawler.Code.Nerdvana.UI
{
    public class ChooseHomeworkTaskGroupMenu : MultipleChoiceObjectConsoleMenu<NerdvanaHomeworkTaskGroup>
    {
        protected override string Title => "Choose task group to evaluate";

        public ChooseHomeworkTaskGroupMenu(IReadOnlyList<NerdvanaHomeworkTaskGroup> homeworks) : base(homeworks) { }
    }
}
