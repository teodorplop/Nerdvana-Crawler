namespace NerdvanaCrawler.Code.Crawler
{
    public interface ICrawler
    {
        bool CanCrawl(string url);
        Task<FetchScoreResponse> TryFetchScore(string username, string taskName, string taskUrl);
    }
}
