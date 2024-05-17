using Google.Apis.Sheets.v4.Data;

namespace NerdvanaCrawler.Code.GoogleApis
{
    public static class GoogleSheetsUtils
    {
        public static string ToColumnName(this int columnNumber)
        {
            string columnName = string.Empty;

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }

        public static Color Lerp(Color from, Color to, float factor)
        {
            return new Color()
            {
                Red = from.Red.GetValueOrDefault() + (to.Red.GetValueOrDefault() - from.Red.GetValueOrDefault()) * factor,
                Green = from.Green.GetValueOrDefault() + (to.Green.GetValueOrDefault() - from.Green.GetValueOrDefault()) * factor,
                Blue = from.Blue.GetValueOrDefault() + (to.Blue.GetValueOrDefault() - from.Blue.GetValueOrDefault()) * factor,
                Alpha = from.Alpha.GetValueOrDefault() + (to.Alpha.GetValueOrDefault() - from.Alpha.GetValueOrDefault()) * factor,
            };
        }
    }
}
