using NerdvanaCrawler.Code.ConsoleUI;
using NerdvanaCrawler.Code.Nerdvana.Homework;

namespace NerdvanaCrawler.Code.Nerdvana.UI
{
    public class ChooseHomeworkStudentsMenu : MultipleChoiceObjectConsoleMenu<NerdvanaHomeworkStudent>
    {
        protected override string Title => "Choose students to evaluate";

        public ChooseHomeworkStudentsMenu(IReadOnlyList<NerdvanaHomeworkStudent> students, Arguments args) : base(students) { }
    }
}
