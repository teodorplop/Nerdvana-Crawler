using Google.Apis.Sheets.v4.Data;

namespace NerdvanaCrawler.Code.GoogleApis
{
    public interface IGoogleSheet
    {
        string Id { get; }
        string Title { get; }
        IReadOnlyList<string> Subsheets { get; }

        Task<ValuesSheetResponse> FetchValues(string subsheetName, GoogleSheetRange range);
        Task<DataSheetResponse> FetchData(string subsheetName, GoogleSheetRange range);

        Task UpdateData(List<RowData> data, string subsheetName, GoogleSheetRange range);
    }

    public sealed class NullGoogleSheet : IGoogleSheet
    {
        public static IGoogleSheet Instance { get; } = new NullGoogleSheet();

        public string Id => string.Empty;
        public string Title => "Null";
        public IReadOnlyList<string> Subsheets => Array.Empty<string>();

        private NullGoogleSheet() { }

        public Task<ValuesSheetResponse> FetchValues(string subsheetName, GoogleSheetRange range) => Task.FromResult(ValuesSheetResponse.Default);
        public Task<DataSheetResponse> FetchData(string subsheetName, GoogleSheetRange range) => Task.FromResult(DataSheetResponse.Default);

        public Task UpdateData(List<RowData> data, string subsheetName, GoogleSheetRange range) => Task.CompletedTask;
    }

    public static class IGoogleSheetExtensions 
    {
        public static Task<ValuesSheetResponse> FetchValues(this IGoogleSheet caller, string subsheetName)
            => caller.FetchValues(subsheetName, GoogleSheetRange.All);

        public static Task<DataSheetResponse> FetchData(this IGoogleSheet caller, string subsheetName)
            => caller.FetchData(subsheetName, GoogleSheetRange.All);
    }
}
