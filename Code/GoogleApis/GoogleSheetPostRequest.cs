using Google.Apis.Sheets.v4.Data;

namespace NerdvanaCrawler.Code.GoogleApis
{
    public struct GoogleSheetPostRequest
    {
        public string SpreadsheetId { get; }
        public int SubsheetId { get; }
        public string SubsheetTitle { get; }
        public GoogleSheetRange Range { get; }
        public List<RowData> Data { get; }

        public GoogleSheetPostRequest(string spreadsheetId, int subsheetId, string subsheetTitle, GoogleSheetRange range, List<RowData> data)
        {
            SpreadsheetId = spreadsheetId;
            SubsheetId = subsheetId;
            SubsheetTitle = subsheetTitle;
            Range = range;
            Data = data;
        }

        public string RangeQuery => $"{SubsheetTitle}{Range}";

        public override string ToString() => RangeQuery;
    }
}
