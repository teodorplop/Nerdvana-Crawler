using System.Text.RegularExpressions;
using NerdvanaCrawler.Code.Network;
using NerdvanaCrawler.Code.Utils;
using Newtonsoft.Json;

namespace NerdvanaCrawler.Code.Crawler
{
    public class CodeforcesCrawler : NetworkClientCrawler
    {
        private class CodeforcesResponse
        {
            [JsonProperty("status")]
            public string Status { get; private set; }

            [JsonProperty("result")]
            public List<CodeforcesSubmission> Result { get; private set; }
        }

        private class CodeforcesSubmission
        {
            [JsonProperty("problem")]
            public CodeforcesProblem Problem { get; private set; }

            [JsonProperty("verdict")]
            public string Verdict { get; private set; }
        }

        private class CodeforcesProblem
        {
            [JsonProperty("contestId")]
            public int ContestId { get; private set; }

            [JsonProperty("index")]
            public string Index { get; private set; }
        }

        private const string K_ROOT = "https://codeforces.com/api/user.status";
        private const string K_NICE_ROOT = "https://codeforces.com/submissions/";
        private const string K_STATUS_OK = "OK";
        private const string K_VERDICT_OK = "OK";
        private const string K_VERDICT_COMPILATION_ERROR = "COMPILATION_ERROR";
        private const int K_MAX_SCORE = 100;

        // Matches both /problemset/problem/{contestId}/{index} and /contest/{contestId}/problem/{index}
        private static readonly Regex m_ProblemRegex = new Regex(@"/(?:problemset/problem/(\d+)/|contest/(\d+)/problem/)([A-Za-z0-9]+)");

        public CodeforcesCrawler(INetworkClient networkClient) : base(networkClient) { }

        public override bool CanCrawl(string url) => url.Contains("codeforces.com");

        private bool TryExtractProblem(string taskUrl, out int contestId, out string index)
        {
            contestId = 0;
            index = string.Empty;

            var match = m_ProblemRegex.Match(taskUrl);
            if (!match.Success)
            {
                this.LogError($"Failed to extract problem from taskUrl={taskUrl}.");
                return false;
            }

            var contestGroup = match.Groups[1].Success ? match.Groups[1] : match.Groups[2];
            contestId = int.Parse(contestGroup.Value);
            index = match.Groups[3].Value.ToUpper();
            return true;
        }

        // All submissions of a user come in a single request, so the network cache makes this one call per student.
        protected override string BuildUrl(string username, string taskName, string taskUrl)
        {
            return $"{K_ROOT}?handle={username}";
        }

        protected override string BuildNiceUrl(string username, string taskName, string taskUrl)
        {
            return TryExtractProblem(taskUrl, out int contestId, out _)
                 ? $"{K_NICE_ROOT}{username}/contest/{contestId}"
                 : $"{K_NICE_ROOT}{username}";
        }

        protected override FetchScoreResponse ParseScore(string content, string username, string task, string taskUrl)
        {
            if (!TryExtractProblem(taskUrl, out int contestId, out string index))
                return new FetchScoreResponse(EFetchScoreResult.InvalidInput, 0);

            FetchScoreResponse response;

            try
            {
                var codeforcesResponse = JsonConvert.DeserializeObject<CodeforcesResponse>(content);
                if (codeforcesResponse.Status != K_STATUS_OK)
                    return new FetchScoreResponse(EFetchScoreResult.ParserError, 0);

                var submissions = codeforcesResponse.Result.FindAll(s => s.Problem.ContestId == contestId && s.Problem.Index == index);

                if (submissions.Count == 0)
                    response = new FetchScoreResponse(EFetchScoreResult.NoSolutions, 0);
                else if (submissions.Exists(s => s.Verdict == K_VERDICT_OK))
                    response = new FetchScoreResponse(EFetchScoreResult.Success, K_MAX_SCORE);
                else if (submissions.TrueForAll(s => s.Verdict == K_VERDICT_COMPILATION_ERROR))
                    response = new FetchScoreResponse(EFetchScoreResult.CompilationError, 0);
                else
                    response = new FetchScoreResponse(EFetchScoreResult.Success, 0);
            }
            catch (Exception ex)
            {
                Logger.LogError(this, $"Failed to deserialize codeforces response for username={username} task={task}. Exception: {ex.Message}");
                response = new FetchScoreResponse(EFetchScoreResult.ParserError, 0);
            }

            return response;
        }
    }
}
