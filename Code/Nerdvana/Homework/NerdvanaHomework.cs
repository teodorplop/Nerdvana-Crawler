using Google.Apis.Sheets.v4.Data;
using NerdvanaCrawler.Code.GoogleApis;
using NerdvanaCrawler.Code.Utils;
using NerdvanaCrawler.Code.Utils.Extensions;

namespace NerdvanaCrawler.Code.Nerdvana.Homework
{
    // TODO: Shouldn't be aware of sheets specific things
    public class NerdvanaHomework
    {
        private const string K_DEFAULT_TASK_GROUP_NAME = "Default";

        private static readonly string m_StudentNameColumnString = NerdvanaGoogleSheetConfig.Instance.StudentNameColumn;
        private static readonly string m_HomeworkTotalColumnString = NerdvanaGoogleSheetConfig.Instance.HomeworkTotalColumnString;

        private DataSheetResponse m_Data;

        public string Name { get; }
        public IReadOnlyList<NerdvanaHomeworkStudent> Students { get; }
        public IReadOnlyList<NerdvanaHomeworkTaskGroup> TaskGroups { get; }

        public NerdvanaHomework(string name, DataSheetResponse data, NerdvanaStudentsCollection studentsCollection)
        {
            m_Data = data;

            Name = name;
            Students = ExtractStudents(data, studentsCollection);
            TaskGroups = ExtractTasks(data);
        }

        public NerdvanaHomeworkStudent? GetStudent(string studentName)
        {
            var studentIndex = Students.FindIndex(s => s.Name == studentName);
            if (studentIndex == -1)
            {
                this.LogWarning($"Failed to find student={studentName} in homework={Name}");
                return default;
            }

            return Students[studentIndex];
        }

        public NerdvanaHomeworkTaskGroup GetTaskGroup(string taskGroupName)
        {
            var taskIndex = TaskGroups.FindIndex(t => t.Name == taskGroupName);
            if (taskIndex == -1)
            {
                this.LogWarning($"Failed to find taskGroup={taskGroupName} in homework={Name}");
                return default;
            }

            return TaskGroups[taskIndex];
        }

        public int GetOverallScore(string studentName, string taskGroupName)
        {
            var student = GetStudent(studentName);
            if (student == null)
                return 0;

            var taskGroup = GetTaskGroup(taskGroupName);
            if (taskGroup == null)
                return 0;

            int score;
            try
            {
                var value = m_Data.Rows[student.Value.SheetRowIndex].Values[taskGroup.SheetTotalColumnIndex].FormattedValue;
                score = int.Parse(value);
            }
            catch (Exception ex)
            {
                score = 0;
                this.LogError($"Failed to parse total for student={studentName} and taskGroup={taskGroupName}. Exception: {ex.Message}");
            }

            return score;
        }

        private IReadOnlyList<NerdvanaHomeworkStudent> ExtractStudents(DataSheetResponse data, NerdvanaStudentsCollection studentsCollection)
        {
            IReadOnlyList<NerdvanaHomeworkStudent> students;

            if (data.IsSuccess)
            {
                try
                {
                    students = ExtractStudents(data.Rows, studentsCollection);
                    this.Log("Successfully extracted students.");
                }
                catch (Exception ex)
                {
                    this.LogError($"Failed to extract students.\nException: {ex.Message}");
                    students = new List<NerdvanaHomeworkStudent>();
                }
            }
            else
            {
                this.LogError($"Data response wasn't successful, so we failed to extract students.");
                students = new List<NerdvanaHomeworkStudent>();
            }

            return students;
        }

        private IReadOnlyList<NerdvanaHomeworkStudent> ExtractStudents(IReadOnlyList<RowData> rows, NerdvanaStudentsCollection studentsCollection)
        {
            List<NerdvanaHomeworkStudent> students = new List<NerdvanaHomeworkStudent>();
            int nameColumnIndex = rows[0].Values.FindIndex(cell => cell.FormattedValue == m_StudentNameColumnString);

            for (int row = 1; row < rows.Count; ++row)
            {
                string studentName = rows[row].Values[nameColumnIndex].FormattedValue;

                if (!string.IsNullOrEmpty(studentName))
                {
                    studentsCollection.TryGetStudentByName(studentName, out var nerdvanaStudent);
                    students.Add(new NerdvanaHomeworkStudent(nerdvanaStudent, row));
                }
            }

            return students;
        }

        private IReadOnlyList<NerdvanaHomeworkTaskGroup> ExtractTasks(DataSheetResponse data)
        {
            IReadOnlyList<NerdvanaHomeworkTaskGroup> tasks;

            if (data.IsSuccess)
            {
                try
                {
                    tasks = ExtractTaskGroups(data.Rows);
                    this.Log("Successfully extracted tasks.");
                }
                catch (Exception ex)
                {
                    this.LogError($"Failed to extract tasks.\nException: {ex.Message}");
                    tasks = Array.Empty<NerdvanaHomeworkTaskGroup>();
                }
            }
            else
            {
                this.LogError($"Data response wasn't successful, so we failed to extract tasks.");
                tasks = Array.Empty<NerdvanaHomeworkTaskGroup>();
            }

            return tasks;
        }

        private IReadOnlyList<NerdvanaHomeworkTaskGroup> ExtractTaskGroups(IReadOnlyList<RowData> rows)
        {
            var taskGroups = new List<NerdvanaHomeworkTaskGroup>();

            var taskGroup = new EditableNerdvanaTaskGroup(K_DEFAULT_TASK_GROUP_NAME);
            for (int column = 0; column < rows[0].Values.Count; ++column)
            {
                var taskGroupName = rows[0].Values[column].FormattedValue;

                if (!string.IsNullOrEmpty(taskGroupName))
                {
                    AddTaskGroup(taskGroups, taskGroup);
                    taskGroup = new EditableNerdvanaTaskGroup(taskGroupName);
                }

                var taskName = rows[1].Values[column].FormattedValue;
                var taskUrl = rows[1].Values[column].Hyperlink;

                if (!string.IsNullOrEmpty(taskName))
                {
                    if (m_HomeworkTotalColumnString != taskName)
                        taskGroup.AddTask(new NerdvanaHomeworkTask(taskName, taskUrl, column, taskGroup));
                    else
                        taskGroup.SetSheetTotalColumnIndex(column);
                }
                    
            }

            AddTaskGroup(taskGroups, taskGroup);

            return taskGroups;
        }

        private void AddTaskGroup(List<NerdvanaHomeworkTaskGroup> taskGroups, NerdvanaHomeworkTaskGroup taskGroup)
        {
            if (taskGroup.Tasks.Count > 0 && !taskGroups.Contains(taskGroup))
                taskGroups.Add(taskGroup);
        }
    }
}
