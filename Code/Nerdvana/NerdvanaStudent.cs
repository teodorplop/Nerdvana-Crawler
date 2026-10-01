namespace NerdvanaCrawler.Code.Nerdvana
{
    public struct NerdvanaStudent : INerdvanaStudent
    {
        public string Name { get; }
        public string NerdarenaUsername { get; }
        public string InfoarenaUsername { get; }
        public string KilonovaUsername { get; }
        public string CodeforcesUsername { get; }

        public NerdvanaStudent(string name, string nerdarenaUsername, string infoarenaUsername, string kilonovaUsername, string codeforcesUsername)
        {
            Name = name;
            NerdarenaUsername = nerdarenaUsername;
            InfoarenaUsername = infoarenaUsername;
            KilonovaUsername = kilonovaUsername;
            CodeforcesUsername = codeforcesUsername;
        }

        public override string ToString() => Name;
    }
}
