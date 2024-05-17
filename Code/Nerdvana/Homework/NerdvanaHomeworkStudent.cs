namespace NerdvanaCrawler.Code.Nerdvana.Homework
{
    public struct NerdvanaHomeworkStudent : INerdvanaStudent
    {
        public NerdvanaStudent Student { get; }
        public int SheetRowIndex { get; }

        public string Name => Student.Name;

        public string NerdarenaUsername => Student.NerdarenaUsername;

        public string InfoarenaUsername => Student.InfoarenaUsername;

        public string KilonovaUsername => Student.KilonovaUsername;

        public NerdvanaHomeworkStudent(NerdvanaStudent student, int sheetRowIndex)
        {
            Student = student;
            SheetRowIndex = sheetRowIndex;
        }

        public override string ToString()
        {
            return Student.ToString();
        }
    }
}
