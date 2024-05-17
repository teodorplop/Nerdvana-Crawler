namespace NerdvanaCrawler.Code.Nerdvana
{
    public struct NerdvanaStudent : INerdvanaStudent
    {
        public string Name { get; }
        public string NerdarenaUsername { get; }
        public string InfoarenaUsername { get; }
        public string KilonovaUsername { get; }

        public NerdvanaStudent(string name, string nerdarenaUsername, string infoarenaUsername, string kilonovaUsername)
        {
            Name = name;
            NerdarenaUsername = nerdarenaUsername;
            InfoarenaUsername = infoarenaUsername;
            KilonovaUsername = kilonovaUsername;
        }

        public override string ToString() => Name;
    }
}
