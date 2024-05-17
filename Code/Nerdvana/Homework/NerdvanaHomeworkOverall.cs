using Google.Apis.Sheets.v4.Data;
using NerdvanaCrawler.Code.GoogleApis;
using NerdvanaCrawler.Code.Utils;
using NerdvanaCrawler.Code.Utils.Extensions;

namespace NerdvanaCrawler.Code.Nerdvana.Homework
{
    public class NerdvanaHomeworkOverall
    {
        private static readonly string m_StudentNameColumnString = NerdvanaGoogleSheetConfig.Instance.StudentNameColumn;
        private static readonly string m_HomeworkOverallScoreColumnString = NerdvanaGoogleSheetConfig.Instance.HomeworkOverallScoreColumnString;
        private static readonly string m_HomeworkOverallMaxScoreColumnString = NerdvanaGoogleSheetConfig.Instance.HomeworkOverallMaxScoreColumnString;
        private static readonly string m_HomeworkOverallPercentageColumnString = NerdvanaGoogleSheetConfig.Instance.HomeworkOverallPercentageColumnString;

        public string Name { get; }
        public IReadOnlyList<string> Students { get; }
        public int ScoreColumnIndex { get; private set; }
        public int MaxScoreColumnIndex { get; private set; }
        public int PercentageColumnIndex { get; private set; }

        public NerdvanaHomeworkOverall(string name, DataSheetResponse data)
        {
            Name = name;
            Students = ExtractStudents(data);
        }

        private IReadOnlyList<string> ExtractStudents(DataSheetResponse data)
        {
            IReadOnlyList<string> students;

            if (data.IsSuccess)
            {
                try
                {
                    students = ExtractStudents(data.Rows);
                    this.Log("Successfully extracted students.");
                }
                catch (Exception ex)
                {
                    this.LogError($"Failed to extract students.\nException: {ex.Message}");
                    students = new List<string>();
                }
            }
            else
            {
                this.LogError($"Data response wasn't successful, so we failed to extract students.");
                students = new List<string>();
            }

            return students;
        }

        private IReadOnlyList<string> ExtractStudents(IReadOnlyList<RowData> rows)
        {
            ScoreColumnIndex = rows[0].Values.FindIndex(cell => cell.FormattedValue == m_HomeworkOverallScoreColumnString);
            MaxScoreColumnIndex = rows[0].Values.FindIndex(cell => cell.FormattedValue == m_HomeworkOverallMaxScoreColumnString);
            PercentageColumnIndex = rows[0].Values.FindIndex(cell => cell.FormattedValue == m_HomeworkOverallPercentageColumnString);

            var students = new List<string>();
            var nameColumnIndex = rows[0].Values.FindIndex(cell => cell.FormattedValue == m_StudentNameColumnString);

            for (int row = 1; row < rows.Count; ++row)
            {
                string studentName = rows[row].Values[nameColumnIndex].FormattedValue;

                if (!string.IsNullOrEmpty(studentName))
                {
                    students.Add(studentName);
                }
            }

            return students;
        }
    }
}
