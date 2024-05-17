using Google.Apis.Sheets.v4.Data;
using NerdvanaCrawler.Code.Utils;

namespace NerdvanaCrawler.Code.GoogleApis
{
    public abstract class GoogleSheetResponse<TData>
    {
        public bool IsSuccess => !EqualityComparer<TData>.Default.Equals(Data, default);
        protected TData Data { get; }

        protected GoogleSheetResponse() : this(default) { }

        public GoogleSheetResponse(TData data)
        {
            Data = data;
        }
    }

    public class ValuesSheetResponse : GoogleSheetResponse<ValueRange>
    {
        public static ValuesSheetResponse Default { get; } = new ValuesSheetResponse();

        public IReadOnlyList<IReadOnlyList<object>> Rows { get; }

        private ValuesSheetResponse() : this(null) { }

        public ValuesSheetResponse(ValueRange values) : base(values) 
        {
            Rows = FetchRows(values);
        }

        private IReadOnlyList<IReadOnlyList<object>> FetchRows(ValueRange values)
        {
            object[][] rows;

            try
            {
                rows = new object[values.Values.Count][];
                for (int i = 0; i < values.Values.Count; ++i)
                    rows[i] = values.Values[i].ToArray();
            }
            catch (Exception ex)
            {
                this.LogError($"Failed to fetch rows from {nameof(ValueRange)}.\nException: {ex.Message}");
                rows = Array.Empty<object[]>();
            }

            return rows;
        }
    }

    public class DataSheetResponse : GoogleSheetResponse<Spreadsheet>
    {
        public static DataSheetResponse Default { get; } = new DataSheetResponse();

        public IReadOnlyList<RowData> Rows { get; }

        private DataSheetResponse() : this(null) { }

        public DataSheetResponse(Spreadsheet data) : base(data) 
        {
            Rows = FetchRows(data);
        }

        private IReadOnlyList<RowData> FetchRows(Spreadsheet data)
        {
            RowData[] rows;

            try
            {
                rows = data.Sheets[0].Data[0].RowData.ToArray();
            }
            catch (Exception ex)
            {
                this.LogError($"Failed to fetch rows from {nameof(Spreadsheet)}.\nException: {ex.Message}");
                rows = Array.Empty<RowData>();
            }

            return rows;
        }
    }
}
