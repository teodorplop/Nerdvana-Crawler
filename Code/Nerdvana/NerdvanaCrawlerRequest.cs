using NerdvanaCrawler.Code.Nerdvana.Homework;

namespace NerdvanaCrawler.Code.Nerdvana
{
    public struct NerdvanaCrawlerRequest
    {
        public NerdvanaHomeworkStudent Student { get; }
        public NerdvanaHomeworkTask Task { get; }

        public NerdvanaCrawlerRequest(NerdvanaHomeworkStudent student, NerdvanaHomeworkTask task)
        {
            Student = student;
            Task = task;
        }

        public override string ToString()
        {
            return $"{Student} - {Task}";
        }
    }
}
