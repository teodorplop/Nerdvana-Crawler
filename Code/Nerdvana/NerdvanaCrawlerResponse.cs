using NerdvanaCrawler.Code.Crawler;

namespace NerdvanaCrawler.Code.Nerdvana
{
    public struct NerdvanaCrawlerResponse
    {
        public NerdvanaCrawlerRequest Request { get; }
        public FetchScoreResponse Response { get; }

        public NerdvanaCrawlerResponse(NerdvanaCrawlerRequest request, FetchScoreResponse response)
        {
            Request = request;
            Response = response;
        }
    }
}
