namespace NerdvanaCrawler.Code.GoogleApis
{
    public struct GoogleSheetGetRequest
    {
        public string SpreadsheetId { get; }
        public string SubsheetTitle { get; }
        public GoogleSheetRange Range { get; }

        public GoogleSheetGetRequest(string spreadsheetId, string subsheetTitle, GoogleSheetRange range)
        {
            SpreadsheetId = spreadsheetId;
            SubsheetTitle = subsheetTitle;
            Range = range;
        }

        public string RangeQuery => $"{SubsheetTitle}{Range}";

        public override string ToString() => RangeQuery;
    }
}
