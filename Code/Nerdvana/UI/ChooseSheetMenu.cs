using NerdvanaCrawler.Code.ConsoleUI;
using NerdvanaCrawler.Code.GoogleApis;
using NerdvanaCrawler.Code.Utils;
using NerdvanaCrawler.Code.Utils.Extensions;

namespace NerdvanaCrawler.Code.Nerdvana.UI
{
    public class ChooseSheetMenu : SingleChoiceObjectConsoleMenu<IGoogleSheet>
    {
        protected override string Title => "Choose sheet to work on";

        public ChooseSheetMenu(IReadOnlyList<IGoogleSheet> sheets, Arguments args) : base(FilterSheetsFromArgs(sheets, args), true) { }

        private static IReadOnlyList<IGoogleSheet> FilterSheetsFromArgs(IReadOnlyList<IGoogleSheet> sheets, Arguments args)
        {
            if (string.IsNullOrEmpty(args.SheetId))
                return sheets;

            var sheetIndex = sheets.FindIndex(sheet => sheet.Id == args.SheetId || sheet.Title == args.SheetId);
            if (sheetIndex == -1)
            {
                typeof(ChooseSheetMenu).LogError("Invalid sheet id argument.");
                return sheets;
            }

            return [sheets[sheetIndex]];
        }
    }
}
