using NerdvanaCrawler.Code.GoogleApis;
using NerdvanaCrawler.Code.Utils;
using NerdvanaCrawler.Code.Utils.Extensions;

namespace NerdvanaCrawler.Code.Nerdvana
{
    // TODO: Shouldn't be aware of sheets specific things
    public class NerdvanaStudentsCollection
    {
        private static readonly string m_StudentNameColumn = NerdvanaGoogleSheetConfig.Instance.StudentNameColumn;
        private static readonly string m_StudentNerdarenaColumn = NerdvanaGoogleSheetConfig.Instance.StudentNerdarenaColumn;
        private static readonly string m_StudentInfoarenaColumn = NerdvanaGoogleSheetConfig.Instance.StudentInfoarenaColumn;
        private static readonly string m_StudentKilonovaColumn = NerdvanaGoogleSheetConfig.Instance.StudentKilonovaColumn;

        public IReadOnlyList<NerdvanaStudent> Students { get; }

        public NerdvanaStudentsCollection(ValuesSheetResponse values)
        {
            Students = ExtractStudents(values);
        }

        public bool TryGetStudentByName(string name, out NerdvanaStudent student)
        {
            int index = Students.FindIndex(s => s.Name == name);
            student = index != -1 ? Students[index] : default;
            if (index == -1)
                this.LogError($"Failed to find student with name={name}");
            return index != -1;
        }

        private IReadOnlyList<NerdvanaStudent> ExtractStudents(ValuesSheetResponse values)
        {
            List<NerdvanaStudent> students = new List<NerdvanaStudent>();

            if (values.IsSuccess)
            {
                try
                {
                    int nameColumnIndex = values.Rows[0].IndexOf(m_StudentNameColumn);
                    int nerdarenaColumnIndex = values.Rows[0].IndexOf(m_StudentNerdarenaColumn);
                    int infoarenaColumnIndex = values.Rows[0].IndexOf(m_StudentInfoarenaColumn);
                    int kilonovaColumnIndex = values.Rows[0].IndexOf(m_StudentKilonovaColumn);

                    for (int row = 1; row < values.Rows.Count; ++row)
                    {
                        try
                        {
                            students.Add(ExtractStudent(values.Rows[row], nameColumnIndex, nerdarenaColumnIndex, infoarenaColumnIndex, kilonovaColumnIndex));
                        }
                        catch (Exception ex)
                        {
                            this.LogError($"Failed to extract student from row index={row}.\nException: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.LogError($"Failed to extract students.\nException: {ex.Message}");
                }
            }
            else
            {
                this.LogError($"Values response wasn't successful, so we failed to extract students.");
            }

            return students;
        }

        private NerdvanaStudent ExtractStudent(IReadOnlyList<object> row, int nameColumnIndex, int nerdarenaColumnIndex, int infoarenaColumnIndex, int kilonovaColumnIndex)
        {
            var name = Convert.ToString(row[nameColumnIndex]);
            var nerdarenaUsername = nerdarenaColumnIndex < row.Count ? Convert.ToString(row[nerdarenaColumnIndex]) : null;
            var infoarenaUsername = infoarenaColumnIndex < row.Count ? Convert.ToString(row[infoarenaColumnIndex]) : null;
            var kilonovaUsername = kilonovaColumnIndex < row.Count ? Convert.ToString(row[kilonovaColumnIndex]) : null;

            return new NerdvanaStudent(name, nerdarenaUsername, infoarenaUsername, kilonovaUsername);
        }
    }
}
