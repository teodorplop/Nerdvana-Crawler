using NerdvanaCrawler.Code.Network;
using NerdvanaCrawler.Code.Utils;
using Newtonsoft.Json;

namespace NerdvanaCrawler.Code.Crawler
{
    public class KilonovaCrawler : NetworkClientCrawler
    {
        private class KilonovaResponse
        {
            [JsonProperty("status")]
            public EKilonovaResponseStatus Status { get; private set; }

            [JsonProperty("data")]
            public KilonovaData Data { get; private set; }
        }

        private enum EKilonovaResponseStatus
        {
            success,
            error
        }

        private enum EKilonovaSubmissionStatus
        {
            finished
        }

        private class KilonovaData
        {
            [JsonProperty("submissions")]
            public List<KilonovaSubmission> Submissions { get; private set; }
        }

        private class KilonovaSubmission : IComparable<KilonovaSubmission>
        {
            [JsonProperty("status")]
            public EKilonovaSubmissionStatus Status { get; private set; }

            [JsonProperty("compile_error")]
            public bool CompileError { get; private set; }

            [JsonProperty("score")]
            public int Score { get; private set; }

            public int CompareTo(KilonovaSubmission? other)
            {
                if (other == null)
                    return -1;

                return Status == other.Status
                     ? (CompileError == other.CompileError ? other.Score.CompareTo(Score) : CompileError.CompareTo(other.CompileError))
                     : (Status == EKilonovaSubmissionStatus.finished ? -1 : 1);
            }
        }

        private const string K_ROOT = "https://kilonova.ro/api/submissions/get";
        private const string K_NICE_ROOT = "https://kilonova.ro/submissions/";
        private const string K_PROBLEMS = "problems/";

        public KilonovaCrawler(INetworkClient networkClient) : base(networkClient) { }

        public override bool CanCrawl(string url) => url.Contains("kilonova");

        private string ExtractTaskName(string taskUrl)
        {
            string taskName = string.Empty;

            // Extract task name from URL
            try
            {
                var index = taskUrl.IndexOf(K_PROBLEMS);
                index += K_PROBLEMS.Length;

                while (index < taskUrl.Length && char.IsDigit(taskUrl[index]))
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

        protected override string BuildUrl(string username, string taskName, string taskUrl)
        {
            taskName = ExtractTaskName(taskUrl);
            return $"{K_ROOT}?problem_id={taskName}&user_id={username}";
        }

        protected override string BuildNiceUrl(string username, string taskName, string taskUrl)
        {
            taskName = ExtractTaskName(taskUrl);
            return $"{K_NICE_ROOT}?problem_id={taskName}&user_id={username}";
        }

        protected override FetchScoreResponse ParseScore(string content, string username, string task, string taskUrl)
        {
            FetchScoreResponse response;

            try
            {
                var kilonovaResponse = JsonConvert.DeserializeObject<KilonovaResponse>(content);
                kilonovaResponse.Data.Submissions.Sort();
                if (kilonovaResponse.Data.Submissions.Count == 0 || kilonovaResponse.Data.Submissions[0].Status != EKilonovaSubmissionStatus.finished)
                {
                    response = new FetchScoreResponse(EFetchScoreResult.NoSolutions, 0);
                }
                else if (kilonovaResponse.Data.Submissions[0].CompileError)
                {
                    response = new FetchScoreResponse(EFetchScoreResult.CompilationError, 0);
                }
                else
                {
                    response = new FetchScoreResponse(EFetchScoreResult.Success, kilonovaResponse.Data.Submissions[0].Score);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(this, $"Failed to deserialize kilonova response for username={username} task={task}. Exception: {ex.Message}");
                response = new FetchScoreResponse(EFetchScoreResult.ParserError, 0);
            }

            return response;
        }
    }
}
