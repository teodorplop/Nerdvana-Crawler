using NerdvanaCrawler.Code.ConsoleUI;
using NerdvanaCrawler.Code.Utils;
using NerdvanaCrawler.Code.Utils.Extensions;

namespace NerdvanaCrawler.Code.Nerdvana.UI
{
    public class ChooseHomeworkMenu : SingleChoiceObjectConsoleMenu<string>
    {
        protected override string Title => "Choose homework to evaluate";

        public ChooseHomeworkMenu(IReadOnlyList<string> homeworks, Arguments args) : base(FilterHomeworksFromArgs(homeworks, args), true) { }
        
        private static IReadOnlyList<string> FilterHomeworksFromArgs(IReadOnlyList<string> homeworks, Arguments args)
        {
            if (string.IsNullOrEmpty(args.HomeworkName))
                return homeworks;

            var homeworkIndex = homeworks.IndexOf(args.HomeworkName);
            if (homeworkIndex == -1)
            {
                typeof(ChooseHomeworkMenu).LogError("Invalid homework name argument.");
                return homeworks;
            }

            return [homeworks[homeworkIndex]];
        }
    }
}
