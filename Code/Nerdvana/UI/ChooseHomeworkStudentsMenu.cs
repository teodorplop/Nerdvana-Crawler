using NerdvanaCrawler.Code.ConsoleUI;
using NerdvanaCrawler.Code.Nerdvana.Homework;
using NerdvanaCrawler.Code.Utils.Extensions;

namespace NerdvanaCrawler.Code.Nerdvana.UI
{
    public class ChooseHomeworkStudentsMenu : MultipleChoiceObjectConsoleMenu<NerdvanaHomeworkStudent>
    {
        private readonly bool m_AllStudents;
        private readonly NerdvanaHomeworkStudent? m_ArgsStudent;
        
        protected override string Title => "Choose students to evaluate";

        public ChooseHomeworkStudentsMenu(IReadOnlyList<NerdvanaHomeworkStudent> students, Arguments args) : base(students)
        {
            m_AllStudents = args.AllStudents;
            if (!string.IsNullOrEmpty(args.StudentName))
            {
                var studentIndex = students.FindIndex(student => student.Name == args.StudentName);
                if (studentIndex != -1)
                    m_ArgsStudent = students[studentIndex];
            }
        }

        public override void Enable()
        {
            base.Enable();
            
            if (m_AllStudents)
                ChooseAllObjects();
            else if (m_ArgsStudent.HasValue)
                ChooseObject(m_ArgsStudent.Value);
        }
    }
}
