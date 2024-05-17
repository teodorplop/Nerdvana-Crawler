using NerdvanaCrawler.Code.Network;
using NerdvanaCrawler.Code.Utils;

namespace NerdvanaCrawler.Code.Crawler
{
    public abstract class NetworkClientCrawler : ICrawler
    {
        private INetworkClient m_NetworkClient;

        public NetworkClientCrawler(INetworkClient networkClient)
        {
            m_NetworkClient = networkClient;
        }

        public async Task<FetchScoreResponse> TryFetchScore(string username, string taskName, string taskUrl)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(taskName) || string.IsNullOrEmpty(taskUrl))
                return new FetchScoreResponse(EFetchScoreResult.InvalidInput, 0);

            var url = BuildUrl(username, taskName, taskUrl);
            var networkResponse = await m_NetworkClient.Get(url);
            var response = new FetchScoreResponse();

            this.Log($"Fetching score for username={username} and task={taskName}.");

            if (networkResponse.IsSuccess)
            {
                if (string.IsNullOrEmpty(networkResponse.Content))
                {
                    this.LogError($"Received empty response for username={username} and task={taskName}.");
                    response = new FetchScoreResponse(EFetchScoreResult.ParserError, 0);
                }
                else
                    response = ParseScore(networkResponse.Content, username, taskName);

                if (response.Result == EFetchScoreResult.ParserError)
                    this.LogError($"Failed to parse score for username={username} and task={taskName}.");
            }
            else
                response = new FetchScoreResponse(EFetchScoreResult.NetworkError, 0);

            if (!response.Result.IsError())
                this.Log($"Successfully fetched score for username={username} and task={taskName}. Score={response.Score}.");

            response = response.WithResultUrl(BuildNiceUrl(username, taskName, taskUrl));

            return response;
        }

        public abstract bool CanCrawl(string url);

        protected abstract string BuildUrl(string username, string task, string taskUrl);

        protected virtual string BuildNiceUrl(string username, string task, string taskUrl) => BuildUrl(username, task, taskUrl);

        protected abstract FetchScoreResponse ParseScore(string content, string username, string task);
    }
}
