using NerdvanaCrawler.Code.ConsoleUI;
using NerdvanaCrawler.Code.Nerdvana.Homework;
using NerdvanaCrawler.Code.Utils.Extensions;

namespace NerdvanaCrawler.Code.Nerdvana.UI
{
    public class ChooseHomeworkTaskGroupMenu : MultipleChoiceObjectConsoleMenu<NerdvanaHomeworkTaskGroup>
    {
        private readonly bool m_AllTaskGroups;
        private readonly NerdvanaHomeworkTaskGroup m_ArgsTaskGroup;
        
        protected override string Title => "Choose task group to evaluate";

        public ChooseHomeworkTaskGroupMenu(IReadOnlyList<NerdvanaHomeworkTaskGroup> homeworks, Arguments args) : base(homeworks)
        {
            m_AllTaskGroups = args.AllTaskGroups;
            if (!string.IsNullOrEmpty(args.TaskGroupName))
            {
                var taskGroupIndex = homeworks.FindIndex(taskGroup => taskGroup.Name == args.TaskGroupName);
                if (taskGroupIndex != -1)
                    m_ArgsTaskGroup = homeworks[taskGroupIndex];
            }
        }

        public override void Enable()
        {
            base.Enable();
                
            if (m_AllTaskGroups)
                ChooseAllObjects();
            else if (m_ArgsTaskGroup != null)
                ChooseObject(m_ArgsTaskGroup);
        }

        protected override void WaitForInput() { }
    }
}
