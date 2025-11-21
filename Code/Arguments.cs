namespace NerdvanaCrawler.Code
{
    public class Arguments
    {
        private const string K_SHEET_ARG = "-sheet";
        
        private const string K_HOMEWORK_ARG = "-homework";
        private const string K_LAST_HOMEWORK = "last";
        
        private const string K_HOMEWORK_TASK_GROUP_ARG = "-taskgroup";
        private const string K_ALL_HOMEWORK_TASK_GROUPS = "all";
        
        private const string K_STUDENT_ARG = "-student";
        private const string K_ALL_STUDENTS = "all";
        
        public string SheetId { get; }
        
        public bool LastHomework => HomeworkName == K_LAST_HOMEWORK;
        public string HomeworkName { get; }
        
        public bool AllTaskGroups => TaskGroupName == K_ALL_HOMEWORK_TASK_GROUPS;
        public string TaskGroupName { get; }
        
        public bool AllStudents => StudentName == K_ALL_STUDENTS;
        public string StudentName { get; }
        
        public Arguments(string[] args)
        {
            int i = 0;
            while (i < args.Length)
            {
                if (args[i] == K_SHEET_ARG)
                    SheetId = args[++i];
                else if (args[i] == K_HOMEWORK_ARG)
                    HomeworkName = args[++i];
                else if (args[i] == K_HOMEWORK_TASK_GROUP_ARG)
                    TaskGroupName = args[++i];
                else if (args[i] == K_STUDENT_ARG)
                    StudentName = args[++i];

                ++i;
            }
        }
    }
}