using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using NerdvanaCrawler.Code.Utils;

namespace NerdvanaCrawler.Code.GoogleApis
{
    public class GoogleSheetsService
    {
        private SheetsService m_SheetsService;
        private Dictionary<string, IGoogleSheet> m_Sheets;

        public GoogleSheetsService(GoogleApiConfig apiConfig)
        {
            var credential = GoogleCredential.FromFile(apiConfig.CredentialFilePath);
            var initializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = apiConfig.ApplicationName
            };

            m_SheetsService = new SheetsService(initializer);
            m_Sheets = new Dictionary<string, IGoogleSheet>();
        }

        public async Task<IGoogleSheet> GetSheet(string id)
        {
            if (!m_Sheets.TryGetValue(id, out var sheet))
                m_Sheets.Add(id, sheet = await LoadSheet(id));

            return sheet;
        }

        private async Task<IGoogleSheet> LoadSheet(string id)
        {
            var getRequest = m_SheetsService.Spreadsheets.Get(id);
            var sheet = await PerformGoogleSheetRequest(getRequest, $"Loading sheet with id={id}");
            return sheet != null ? new GoogleSheet(sheet, LoadValues, LoadData, SetData) : NullGoogleSheet.Instance;
        }

        private async Task<ValuesSheetResponse> LoadValues(GoogleSheetGetRequest request)
        {
            var getRequest = m_SheetsService.Spreadsheets.Values.Get(request.SpreadsheetId, request.RangeQuery);
            var values = await PerformGoogleSheetRequest(getRequest, $"Loading values from request={request}");
            return new ValuesSheetResponse(values);
        }

        private async Task<DataSheetResponse> LoadData(GoogleSheetGetRequest request)
        {
            var getRequest = m_SheetsService.Spreadsheets.Get(request.SpreadsheetId);
            getRequest.IncludeGridData = true;
            getRequest.Ranges = request.RangeQuery;

            var data = await PerformGoogleSheetRequest(getRequest, $"Loading data from request={request}");
            return new DataSheetResponse(data);
        }

        private async Task SetData(GoogleSheetPostRequest request)
        {
            var updateRequest = new Request()
            {
                UpdateCells = new UpdateCellsRequest()
                {
                    Range = new GridRange()
                    {
                        SheetId = request.SubsheetId,
                        StartRowIndex = request.Range.FirstRow,
                        EndRowIndex = request.Range.FirstRow + request.Range.NoRows,
                        StartColumnIndex = request.Range.FirstColumn,
                        EndColumnIndex = request.Range.FirstColumn + request.Range.NoColumns
                    },
                    Fields = "userEnteredValue,userEnteredFormat.backgroundColor",
                    Rows = request.Data
                }
            };
            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest() { Requests = new List<Request>() { updateRequest } };

            await PerformGoogleSheetRequest(m_SheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, request.SpreadsheetId), $"Writing values from request={request}");
        }

        private async Task<TData> PerformGoogleSheetRequest<TData>(SheetsBaseServiceRequest<TData> request, string message)
        {
            this.Log($"{message} - Started.");

            TData data = default;

            try
            {
                data = await request.ExecuteAsync();
                if (data != null)
                    this.Log($"{message} - Success.");
                else
                    this.LogError($"{message} - Failure.");
            }
            catch (Exception ex)
            {
                this.LogError($"{message} - Failure.\nException: {ex.Message}");
            }

            return data;
        }
    }
}
