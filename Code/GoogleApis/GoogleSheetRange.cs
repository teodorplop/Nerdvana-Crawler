namespace NerdvanaCrawler.Code.GoogleApis
{
    public struct GoogleSheetRange
    {
        public int FirstRow { get; private set; }
        public int FirstColumn { get; private set; }
        public int NoRows { get; private set; }
        public int NoColumns { get; private set; }

        public string RangeQuery { get; private set; }

        public override string ToString() => RangeQuery;

        public static GoogleSheetRange All =>
            new GoogleSheetRange
            {
                FirstRow = -1,
                FirstColumn = -1,
                NoRows = -1,
                NoColumns = -1,

                RangeQuery = string.Empty
            };

        public static GoogleSheetRange Rows(int firstRow, int noRows) =>
            new GoogleSheetRange
            {
                FirstRow = firstRow,
                NoRows = noRows,

                FirstColumn = -1,
                NoColumns = -1,

                RangeQuery = $"!{firstRow}:{firstRow + noRows - 1}"
            };

        public static GoogleSheetRange Columns(int firstColumn, int noColumns) =>
            new GoogleSheetRange
            {
                FirstColumn = firstColumn,
                NoColumns = noColumns,

                FirstRow = -1,
                NoRows = -1,

                RangeQuery = $"!{firstColumn.ToColumnName()}:{(firstColumn + noColumns - 1).ToColumnName()}"
            };

        public static GoogleSheetRange Range(int firstRow, int firstColumn, int noRows, int noColumns) =>
            new GoogleSheetRange
            {
                FirstRow = firstRow,
                NoRows = noRows,

                FirstColumn = firstColumn,
                NoColumns = noColumns,

                RangeQuery = $"!{firstColumn.ToColumnName()}{firstRow}:{(firstColumn + noColumns - 1).ToColumnName()}{firstRow + noRows - 1}"
            };

        public static GoogleSheetRange RowRange(int firstRow, int firstColumn, int noRows) =>
            new GoogleSheetRange
            {
                FirstRow = firstRow,
                NoRows = noRows,

                FirstColumn = firstColumn,
                NoColumns = -1,

                RangeQuery = $"!{firstColumn.ToColumnName()}{firstRow}:{firstRow + noRows - 1}"
            };

        public static GoogleSheetRange ColumnRange(int firstRow, int firstColumn, int noColumns) =>
            new GoogleSheetRange
            {
                FirstRow = firstRow,
                NoRows = -1,

                FirstColumn = firstColumn,
                NoColumns = noColumns,

                RangeQuery = $"!{firstColumn.ToColumnName()}{firstRow}:{(firstColumn + noColumns - 1).ToColumnName()}"
            };
    }
}
