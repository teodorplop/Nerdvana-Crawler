using Google.Apis.Sheets.v4.Data;
using NerdvanaCrawler.Code.Crawler;
using NerdvanaCrawler.Code.GoogleApis;
using NerdvanaCrawler.Code.Nerdvana.Homework;
using NerdvanaCrawler.Code.Utils;

namespace NerdvanaCrawler.Code.Nerdvana
{
    public class NerdvanaGoogleSheet
    {
        private static readonly string m_StudentsCollectionSubsheet = NerdvanaGoogleSheetConfig.Instance.StudentsCollectionSubsheet;
        private static readonly string m_HomeworkPrefix = NerdvanaGoogleSheetConfig.Instance.HomeworkPrefix;
        private static readonly string m_HomeworkOverallPrefix = NerdvanaGoogleSheetConfig.Instance.HomeworkOverallPrefix;

        private IGoogleSheet m_GoogleSheet;
        private NerdvanaStudentsCollection m_StudentsCollection;

        public IReadOnlyList<string> Homeworks { get; }
        public IReadOnlyList<string> HomeworkOveralls { get; }

        public NerdvanaGoogleSheet(IGoogleSheet googleSheet)
        {
            m_GoogleSheet = googleSheet;
            Homeworks = m_GoogleSheet.Subsheets.Where(s => s.StartsWith(m_HomeworkPrefix)).ToArray();
            HomeworkOveralls = m_GoogleSheet.Subsheets.Where(s => s.StartsWith(m_HomeworkOverallPrefix)).ToArray();
        }

        public async Task<NerdvanaStudentsCollection> GetStudents()
        {
            if (m_StudentsCollection == null)
            {
                var values = await m_GoogleSheet.FetchValues(m_StudentsCollectionSubsheet);
                m_StudentsCollection = new NerdvanaStudentsCollection(values);
            }

            return m_StudentsCollection;
        }

        public async Task<NerdvanaHomework> LoadHomework(string homework)
        {
            if (m_StudentsCollection == null)
            {
                await GetStudents();
                if (m_StudentsCollection == null)
                {
                    this.LogError("Failed to load homework.");
                    return default;
                }
            }

            var data = await m_GoogleSheet.FetchData(homework);
            return new NerdvanaHomework(homework, data, m_StudentsCollection);
        }

        public async Task<NerdvanaHomeworkOverall> LoadHomeworkOverall(string name)
        {
            if (m_StudentsCollection == null)
            {
                await GetStudents();
                if (m_StudentsCollection == null)
                {
                    this.LogError("Failed to load homework.");
                    return default;
                }
            }

            var data = await m_GoogleSheet.FetchData(name);
            return new NerdvanaHomeworkOverall(name, data);
        }

        public async Task UpdateHomework(NerdvanaHomework homework, IReadOnlyList<NerdvanaCrawlerResponse> results)
        {
            int firstRow, lastRow, firstColumn, lastColumn;

            firstRow = int.MaxValue;
            lastRow = int.MinValue;
            firstColumn = int.MaxValue;
            lastColumn = int.MinValue;
            foreach (var result in results)
            {
                firstRow = Math.Min(firstRow, result.Request.Student.SheetRowIndex);
                lastRow = Math.Max(lastRow, result.Request.Student.SheetRowIndex);

                firstColumn = Math.Min(firstColumn, result.Request.Task.SheetColumnIndex);
                lastColumn = Math.Max(lastColumn, result.Request.Task.SheetColumnIndex);
                if (result.Request.Task.TaskGroup != null)
                {
                    firstColumn = Math.Min(firstColumn, result.Request.Task.TaskGroup.SheetTotalColumnIndex);
                    lastColumn = Math.Max(lastColumn, result.Request.Task.TaskGroup.SheetTotalColumnIndex);
                }
            }

            var data = new List<RowData>(lastRow - firstRow + 1);
            for (int i = 0; i < data.Capacity; ++i)
                data.Add(new RowData() { Values = new CellData[lastColumn - firstColumn + 1] });

            foreach (var result in results)
            {
                if (result.Request.Task.TaskGroup != null) 
                {
                    data[result.Request.Student.SheetRowIndex - firstRow].Values[result.Request.Task.TaskGroup.SheetTotalColumnIndex - firstColumn] = 
                        UpdateTotalData(
                            data[result.Request.Student.SheetRowIndex - firstRow].Values[result.Request.Task.TaskGroup.SheetTotalColumnIndex - firstColumn], 
                            result.Response.Score, 
                            result.Request.Task.TaskGroup.Tasks.Count,
                            IsTaskGroupColored(homework, result.Request.Task.TaskGroup));
                }

                data[result.Request.Student.SheetRowIndex - firstRow].Values[result.Request.Task.SheetColumnIndex - firstColumn] = ResultToSheetData(result);
            }

            await m_GoogleSheet.UpdateData(data, homework.Name, GoogleSheetRange.Range(firstRow, firstColumn, lastRow - firstRow + 1, lastColumn - firstColumn + 1));
        }

        // Sorts the students (entire rows) by the Total of the first task group, highest score first.
        public async Task SortHomework(NerdvanaHomework homework)
        {
            if (homework.Students.Count == 0 || homework.TaskGroups.Count == 0)
                return;

            var totalColumnIndex = homework.TaskGroups[0].SheetTotalColumnIndex;
            if (totalColumnIndex < 0)
            {
                this.LogError($"Failed to sort homework={homework.Name}: first task group has no total column.");
                return;
            }

            int firstRow = homework.Students.Min(s => s.SheetRowIndex);
            int lastRow = homework.Students.Max(s => s.SheetRowIndex);

            await m_GoogleSheet.SortRange(homework.Name, GoogleSheetRange.Rows(firstRow, lastRow - firstRow + 1), totalColumnIndex, false);
        }

        public async Task UpdateHomeworkOverall()
        {
            if (HomeworkOveralls == null)
                return;

            var homeworkTasks = new List<Task<NerdvanaHomework>>();
            foreach (var homeworkName in Homeworks)
                homeworkTasks.Add(LoadHomework(homeworkName));

            await Task.WhenAll(homeworkTasks);

            foreach (var homeworkOverallName in HomeworkOveralls)
            {
                var taskGroupName = homeworkOverallName.Remove(0, m_HomeworkOverallPrefix.Length).Trim();
                var homeworkOverall = await LoadHomeworkOverall(homeworkOverallName);

                int minColumnIndex = Math.Min(Math.Min(homeworkOverall.MaxScoreColumnIndex, homeworkOverall.PercentageColumnIndex), homeworkOverall.ScoreColumnIndex);

                var data = new List<RowData>(homeworkOverall.Students.Count);
                for (int i = 0; i < data.Capacity; ++i)
                    data.Add(new RowData() { Values = new CellData[3] });

                int tasksCount = 0;
                foreach (var homeworkTask in homeworkTasks)
                {
                    var taskGroup = homeworkTask.Result.GetTaskGroup(taskGroupName);
                    if (taskGroup != null)
                        tasksCount += taskGroup.Tasks.Count;
                }

                for (int i = 0; i < homeworkOverall.Students.Count; ++i)
                {
                    var total = 0;
                    foreach (var homework in homeworkTasks)
                    {
                        total += homework.Result.GetOverallScore(homeworkOverall.Students[i], taskGroupName);
                    }

                    data[i].Values[homeworkOverall.ScoreColumnIndex - minColumnIndex] = new CellData
                    {
                        UserEnteredValue = new ExtendedValue
                        {
                            NumberValue = total
                        }
                    };

                    data[i].Values[homeworkOverall.MaxScoreColumnIndex - minColumnIndex] = new CellData()
                    {
                        UserEnteredValue = new ExtendedValue
                        {
                            NumberValue = tasksCount * 100
                        }
                    };

                    data[i].Values[homeworkOverall.PercentageColumnIndex - minColumnIndex] = UpdateTotalData(data[i].Values[2], total / tasksCount, 1, true);
                }

                await m_GoogleSheet.UpdateData(data, homeworkOverallName, GoogleSheetRange.Range(1, 1, homeworkOverall.Students.Count, 3));
            }
        }

        private CellData ResultToSheetData(NerdvanaCrawlerResponse response)
        {
            string text;
            switch (response.Response.Result)
            {
                case EFetchScoreResult.Success:
                    text = response.Response.Score.ToString();
                    break;
                case EFetchScoreResult.CompilationError:
                    text = $"{response.Response.Score} (CE)";
                    break;
                default:
                    text = response.Response.Result.ToString();
                    break;
            }

            return new CellData()
            {
                UserEnteredValue = new ExtendedValue()
                {
                    FormulaValue = $"=HYPERLINK(\"{response.Response.ResultUrl}\", \"{text}\")"
                }
            };
        }

        private static bool IsTaskGroupColored(NerdvanaHomework homework, NerdvanaHomeworkTaskGroup taskGroup)
        {
            return homework.TaskGroups.Count > 0 && homework.TaskGroups[0] == taskGroup;
        }

        private CellData UpdateTotalData(CellData cellData, int addScore, int tasksCount, bool colored)
        {
            if (cellData == null)
            {
                cellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue()
                    {
                        NumberValue = 0
                    }
                };
            }

            cellData.UserEnteredValue.NumberValue += addScore;

            // When not colored, the format stays null, which clears the background color in the sheet.
            if (colored)
            {
                cellData.UserEnteredFormat = new CellFormat()
                {
                    BackgroundColor = GoogleSheetsUtils.Lerp(new Color() { Red = 1, Alpha = 1 }, new Color() { Green = 1, Alpha = 1 }, (float)cellData.UserEnteredValue.NumberValue / (tasksCount * 100))
                };
            }

            return cellData;
        }
    }
}
