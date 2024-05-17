using Google.Apis.Sheets.v4.Data;
using NerdvanaCrawler.Code.Utils;

namespace NerdvanaCrawler.Code.GoogleApis
{
    public class GoogleSheet : IGoogleSheet
    {
        private Spreadsheet m_Sheet;
        private Func<GoogleSheetGetRequest, Task<ValuesSheetResponse>> m_FetchValuesEvent;
        private Func<GoogleSheetGetRequest, Task<DataSheetResponse>> m_FetchDataEvent;
        private Func<GoogleSheetPostRequest, Task> m_PostDataEvent;

        public string Id => m_Sheet.SpreadsheetId;
        public string Title => m_Sheet.Properties.Title;
        public IReadOnlyList<string> Subsheets { get; }

        public GoogleSheet(
            Spreadsheet sheet, 
            Func<GoogleSheetGetRequest, Task<ValuesSheetResponse>> fetchValuesEvent,
            Func<GoogleSheetGetRequest, Task<DataSheetResponse>> fetchDataEvent,
            Func<GoogleSheetPostRequest, Task> postDataEvent)
        {
            m_Sheet = sheet;
            m_FetchValuesEvent = fetchValuesEvent;
            m_FetchDataEvent = fetchDataEvent;
            m_PostDataEvent = postDataEvent;

            Subsheets = m_Sheet.Sheets.Select(s => s.Properties.Title).ToArray();
        }

        public async Task<ValuesSheetResponse> FetchValues(string subsheetName, GoogleSheetRange range)
        {
            var subsheet = m_Sheet.Sheets.FirstOrDefault(s => s.Properties.Title == subsheetName);

            ValuesSheetResponse values;

            if (subsheet != default)
                values = await m_FetchValuesEvent.Invoke(CreateGetRequest(subsheet, range));
            else
            {
                this.LogError($"There's no subsheet with Title={subsheetName}.");
                values = ValuesSheetResponse.Default;
            }

            return values;
        }

        public async Task<DataSheetResponse> FetchData(string subsheetName, GoogleSheetRange range)
        {
            var subsheet = m_Sheet.Sheets.FirstOrDefault(s => s.Properties.Title == subsheetName);

            DataSheetResponse data;

            if (subsheet != default)
                data = await m_FetchDataEvent.Invoke(CreateGetRequest(subsheet, range));
            else
            {
                this.LogError($"There's no subsheet with Title={subsheetName}.");
                data = DataSheetResponse.Default;
            }

            return data;
        }

        public async Task UpdateData(List<RowData> data, string subsheetName, GoogleSheetRange range)
        {
            var subsheet = m_Sheet.Sheets.FirstOrDefault(s => s.Properties.Title == subsheetName);

            if (subsheet != default)
                await m_PostDataEvent.Invoke(CreatePostRequest(subsheet, range, data));
            else
                this.LogError($"There's not subsheet with Title={subsheetName}.");
        }

        private GoogleSheetGetRequest CreateGetRequest(Sheet subsheet, GoogleSheetRange range) =>
            new GoogleSheetGetRequest(Id, subsheet.Properties.Title, range);

        private GoogleSheetPostRequest CreatePostRequest(Sheet subsheet, GoogleSheetRange range, List<RowData> data) =>
            new GoogleSheetPostRequest(Id, subsheet.Properties.SheetId.GetValueOrDefault(), subsheet.Properties.Title, range, data);

        public override string ToString() => Title;
    }
}
