using NerdvanaCrawler.Code;
using NerdvanaCrawler.Code.ConsoleUI;
using NerdvanaCrawler.Code.GoogleApis;
using NerdvanaCrawler.Code.Nerdvana;
using NerdvanaCrawler.Code.Nerdvana.Homework;
using NerdvanaCrawler.Code.Nerdvana.UI;
using NerdvanaCrawler.Code.Network;

namespace NerdvanaCrawler
{
    class App
    {
        private INetworkClient m_NetworkClient;
        private NerdvanaCrawlerRequestDispatcher m_NerdvanaCrawler;
        private GoogleSheetsService m_GoogleSheetsService;
        private IGoogleSheet[] m_Sheets;

        private static Task Main(string[] args) => new App().Run(args);

        private async Task Run(string[] args)
        {
            m_NetworkClient = CreateNetworkClient();
            m_NerdvanaCrawler = CreateNerdvanaCrawler();
            m_GoogleSheetsService = CreateGoogleSheetsService();
            m_Sheets = await GetSheets();

            await RunFlow(args);
        }

        private async Task RunFlow(string[] args)
        {
            var arguments = new Arguments(args);
            
            var menuStack = new ConsoleMenuStack();

            // Choose which sheet to work on
            var chooseSheetMenu = new ChooseSheetMenu(m_Sheets, arguments);
            menuStack.Push(chooseSheetMenu);
            var chosenSheet = await chooseSheetMenu.WaitForSelection();

            // Prepare sheet
            var nerdvanaSheet = new NerdvanaGoogleSheet(chosenSheet);

            // Choose which homework to evaluate
            var chooseHomeworkMenu = new ChooseHomeworkMenu(nerdvanaSheet.Homeworks, arguments);
            menuStack.Push(chooseHomeworkMenu);
            var chosenHomeworkName = await chooseHomeworkMenu.WaitForSelection();
            var chosenHomework = await nerdvanaSheet.LoadHomework(chosenHomeworkName);

            // Choose which task group to evaluate
            var chooseHomeworkTaskGroupMenu = new ChooseHomeworkTaskGroupMenu(chosenHomework.TaskGroups, arguments);
            menuStack.Push(chooseHomeworkTaskGroupMenu);
            var chosenTaskGroups = await chooseHomeworkTaskGroupMenu.WaitForSelection();

            // Choose which students to evaluate
            var chooseStudentsMenu = new ChooseHomeworkStudentsMenu(chosenHomework.Students, arguments);
            menuStack.Push(chooseStudentsMenu);
            var chosenStudents = await chooseStudentsMenu.WaitForSelection();

            var evaluation = await Evaluate(chosenStudents, chosenTaskGroups);
            await nerdvanaSheet.UpdateHomework(chosenHomework, evaluation);
            await nerdvanaSheet.SortHomework(chosenHomework);

            await nerdvanaSheet.UpdateHomeworkOverall();
        }

        private async Task<List<NerdvanaCrawlerResponse>> Evaluate(IReadOnlyList<NerdvanaHomeworkStudent> students, IReadOnlyList<NerdvanaHomeworkTaskGroup> taskGroups)
        {
            var tasks = new List<Task<NerdvanaCrawlerResponse>>();

            foreach (var student in students)
                foreach (var taskGroup in taskGroups)
                    foreach (var task in taskGroup.Tasks)
                        tasks.Add(m_NerdvanaCrawler.EnqueueCrawlRequest(new NerdvanaCrawlerRequest(student, task)));

            await Task.WhenAll(tasks);

            var responses = new List<NerdvanaCrawlerResponse>(tasks.Count);
            for (int i = 0; i < tasks.Count; ++i)
                responses.Add(tasks[i].Result);

            return responses;
        }

        private async Task<IGoogleSheet[]> GetSheets()
        {
            var sheetsIds = NerdvanaSheetsCollectionConfig.Instance.Sheets;
            var sheets = new IGoogleSheet[sheetsIds.Length];
            for (int i = 0; i < sheetsIds.Length; ++i)
                sheets[i] = await m_GoogleSheetsService.GetSheet(sheetsIds[i]);

            return sheets;
        }

        private NerdvanaCrawlerRequestDispatcher CreateNerdvanaCrawler() => new NerdvanaCrawlerRequestDispatcher(m_NetworkClient);

        private INetworkClient CreateNetworkClient() => new NetworkClientCache(new NetworkClient());

        private GoogleSheetsService CreateGoogleSheetsService()
        {
            var googleApiConfig = new GoogleApiConfig()
            {
                ApplicationName = AppInfo.Name,
                CredentialFilePath = "Resources/credentials.json"
            };
            return new GoogleSheetsService(googleApiConfig);
        }
    }
}
