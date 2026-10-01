using NerdvanaCrawler.Code.Network;
using NerdvanaCrawler.Code.Utils;
using NerdvanaCrawler.Code.Utils.Extensions;

namespace NerdvanaCrawler.Code.Crawler
{
    public abstract class ArenaCrawler : NetworkClientCrawler
    {
        private const string K_PROBLEMA = "problema/";

        protected abstract string ShortRootUrl { get; }
        protected abstract string RootUrl { get; }

        protected virtual string[] NoSolutionStrings { get; } = new[] { "nici o solutie", "nici o soluție" };
        protected virtual string[] CompileErrorStrings { get; } = new[] { "eroare de compilare" };
        protected virtual string[] CompileOkayStrings { get; } = new[] { "evaluare completa:", "evaluare completă:" };

        public ArenaCrawler(INetworkClient networkClient) : base(networkClient) { }

        public override bool CanCrawl(string url) => url.Contains(ShortRootUrl);

        private string ExtractTaskName(string taskUrl)
        {
            string taskName = string.Empty;

            // Extract task name from URL
            try
            {
                var index = taskUrl.IndexOf(K_PROBLEMA);
                index += K_PROBLEMA.Length;

                while (index < taskUrl.Length && char.IsLetterOrDigit(taskUrl[index]))
                {
                    taskName += taskUrl[index];
                    ++index;
                }
            }
            catch (Exception ex)
            {
                this.LogError($"Failed to extract taskName from taskUrl={taskUrl}. Exception: {ex.Message}");
            }

            return taskName;
        }

        protected override string BuildUrl(string username, string task, string taskUrl)
        {
            return $"{RootUrl}/monitor?task={ExtractTaskName(taskUrl)}&user={username}";
        }

        protected override FetchScoreResponse ParseScore(string content, string username, string task, string taskUrl)
        {
            var response = new FetchScoreResponse();

            content = content.ToLower();

            int compileOkayIndex = content.IndexOfAny(CompileOkayStrings);
            if (compileOkayIndex != -1)
                response = new FetchScoreResponse(EFetchScoreResult.Success, ExtractScore(content, compileOkayIndex));
            else if (content.IndexOfAny(NoSolutionStrings) != -1)
                response = new FetchScoreResponse(EFetchScoreResult.NoSolutions, 0);
            else if (content.IndexOfAny(CompileErrorStrings) != -1)
                response = new FetchScoreResponse(EFetchScoreResult.CompilationError, 0);
            else
                response = new FetchScoreResponse(EFetchScoreResult.ParserError, 0);

            return response;
        }

        private int ExtractScore(string content, int startIndex)
        {
            int score = 0;

            while (startIndex < content.Length && !char.IsDigit(content[startIndex]))
                ++startIndex;

            while (startIndex < content.Length && char.IsDigit(content[startIndex]))
                score = score * 10 + content[startIndex++] - '0';

            return score;
        }
    }
}
