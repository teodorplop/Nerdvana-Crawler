namespace NerdvanaCrawler.Code.GoogleApis
{
    public struct GoogleSheetSortRequest
    {
        public string SpreadsheetId { get; }
        public int SubsheetId { get; }
        public string SubsheetTitle { get; }
        public GoogleSheetRange Range { get; }
        public int SortColumn { get; }
        public bool Ascending { get; }

        public GoogleSheetSortRequest(string spreadsheetId, int subsheetId, string subsheetTitle, GoogleSheetRange range, int sortColumn, bool ascending)
        {
            SpreadsheetId = spreadsheetId;
            SubsheetId = subsheetId;
            SubsheetTitle = subsheetTitle;
            Range = range;
            SortColumn = sortColumn;
            Ascending = ascending;
        }

        public string RangeQuery => $"{SubsheetTitle}{Range}";

        public override string ToString() => RangeQuery;
    }
}
