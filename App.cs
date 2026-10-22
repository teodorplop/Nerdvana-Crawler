using NerdvanaCrawler.Code;
using NerdvanaCrawler.Code.ConsoleUI;
using NerdvanaCrawler.Code.GoogleApis;
using NerdvanaCrawler.Code.Nerdvana;
using NerdvanaCrawler.Code.Nerdvana.Homework;
using NerdvanaCrawler.Code.Nerdvana.UI;
using NerdvanaCrawler.Code.Network;
using NerdvanaCrawler.Code.Utils;
using System.Text.RegularExpressions;

namespace NerdvanaCrawler
{
    class App
    {
        private INetworkClient m_NetworkClient;
        private NerdvanaCrawlerRequestDispatcher m_NerdvanaCrawler;
        private GoogleSheetsService m_GoogleSheetsService;

        private static Task Main(string[] args) => new App().Run(args);

        private async Task Run(string[] args)
        {
            m_NetworkClient = CreateNetworkClient();
            m_NerdvanaCrawler = CreateNerdvanaCrawler();
            m_GoogleSheetsService = CreateGoogleSheetsService();

            var arguments = new Arguments(args);
            var sheet = await GetSheet(arguments);
            if (sheet == NullGoogleSheet.Instance)
            {
                this.LogError("Failed to load sheet. Make sure it exists and is shared with the service account as Editor.");
                return;
            }

            await RunFlow(sheet, arguments);
        }

        private async Task RunFlow(IGoogleSheet sheet, Arguments arguments)
        {
            var menuStack = new ConsoleMenuStack();

            // Prepare sheet
            var nerdvanaSheet = new NerdvanaGoogleSheet(sheet);

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

        private async Task<IGoogleSheet> GetSheet(Arguments arguments)
        {
            // Sheet id priority: -sheet argument, SHEET_ID environment variable, console input
            var sheetId = arguments.SheetId;
            if (string.IsNullOrEmpty(sheetId))
                sheetId = Environment.GetEnvironmentVariable("SHEET_ID");

            if (string.IsNullOrEmpty(sheetId))
            {
                Console.Write("Enter sheet id or URL: ");
                sheetId = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(sheetId))
                return NullGoogleSheet.Instance;

            return await m_GoogleSheetsService.GetSheet(ParseSheetId(sheetId.Trim()));
        }

        // Accepts either a raw sheet id or a full URL like https://docs.google.com/spreadsheets/d/<id>/edit
        private static string ParseSheetId(string sheetIdOrUrl)
        {
            var match = Regex.Match(sheetIdOrUrl, @"/spreadsheets/d/([a-zA-Z0-9_-]+)");
            return match.Success ? match.Groups[1].Value : sheetIdOrUrl;
        }

        private NerdvanaCrawlerRequestDispatcher CreateNerdvanaCrawler() => new NerdvanaCrawlerRequestDispatcher(m_NetworkClient);

        private INetworkClient CreateNetworkClient() => new NetworkClientCache(new NetworkClient());

        private GoogleSheetsService CreateGoogleSheetsService()
        {
            var googleApiConfig = new GoogleApiConfig()
            {
                ApplicationName = AppInfo.Name,
                CredentialJson = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIALS"),
                CredentialFilePath = "Resources/credentials.json"
            };
            return new GoogleSheetsService(googleApiConfig);
        }
    }
}
