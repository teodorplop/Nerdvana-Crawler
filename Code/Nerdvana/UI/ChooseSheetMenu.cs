using NerdvanaCrawler.Code.ConsoleUI;
using NerdvanaCrawler.Code.GoogleApis;

namespace NerdvanaCrawler.Code.Nerdvana.UI
{
    public class ChooseSheetMenu : SingleChoiceObjectConsoleMenu<IGoogleSheet>
    {
        protected override string Title => "Choose sheet to work on";

        public ChooseSheetMenu(IReadOnlyList<IGoogleSheet> sheets, Arguments args) : base(sheets) { }
    }
}
