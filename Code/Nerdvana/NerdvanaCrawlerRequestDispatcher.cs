using NerdvanaCrawler.Code.Crawler;
using NerdvanaCrawler.Code.Nerdvana.Homework;
using NerdvanaCrawler.Code.Network;
using NerdvanaCrawler.Code.Utils;

namespace NerdvanaCrawler.Code.Nerdvana
{
    public class NerdvanaCrawlerRequestDispatcher
    {
        private static int MinDelay = NerdvanaCrawlerRequestDispatcherConfig.Instance.MinDelay;
        private static int MaxDelay = NerdvanaCrawlerRequestDispatcherConfig.Instance.MinDelay;

        private class CrawlerWrapper
        {
            private ICrawler m_Crawler;
            private TaskQueue m_CrawlerQueue;

            private Func<INerdvanaStudent, string> m_ExtractUsername;

            public CrawlerWrapper(ICrawler crawler, Func<INerdvanaStudent, string> extractUsername)
            {
                m_Crawler = crawler;
                m_CrawlerQueue = new TaskQueue();

                m_ExtractUsername = extractUsername;
            }

            public bool CanCrawl(string url) => m_Crawler.CanCrawl(url);

            public async Task<FetchScoreResponse> EnqueueCrawlRequest(INerdvanaStudent student, NerdvanaHomeworkTask task)
            {
                FetchScoreResponse scoreResponse = default;

                await m_CrawlerQueue.Enqueue(async () => 
                {
                    scoreResponse = await m_Crawler.TryFetchScore(m_ExtractUsername.Invoke(student), task.Name, task.Url);
                    if (scoreResponse.Result != EFetchScoreResult.InvalidInput)
                        await Task.Delay(Random.Shared.Next(MinDelay, MaxDelay));
                });

                return scoreResponse;
            }
        }

        private CrawlerWrapper[] m_Crawlers;

        public NerdvanaCrawlerRequestDispatcher(INetworkClient networkClient)
        {
            m_Crawlers = new CrawlerWrapper[] 
            { 
                new CrawlerWrapper(new NerdarenaCrawler(networkClient), student => student.NerdarenaUsername),
                new CrawlerWrapper(new InfoarenaCrawler(networkClient), student => student.InfoarenaUsername),
                new CrawlerWrapper(new KilonovaCrawler(networkClient), student => student.KilonovaUsername)
            };
        }

        public async Task<NerdvanaCrawlerResponse> EnqueueCrawlRequest(NerdvanaCrawlerRequest request)
        {
            var crawler = FindCrawlerFor(request);
            if (crawler == null)
            {
                Logger.LogError(this, $"Failed to find crawler for {request}");
                return new NerdvanaCrawlerResponse(request, new FetchScoreResponse(EFetchScoreResult.NoCrawler, 0));
            }

            var fetchScoreResponse = await crawler.EnqueueCrawlRequest(request.Student, request.Task);
            return new NerdvanaCrawlerResponse(request, fetchScoreResponse);
        }

        private CrawlerWrapper FindCrawlerFor(NerdvanaCrawlerRequest request)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return Array.Find(m_Crawlers, crawler => crawler.CanCrawl(request.Task.Url));
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
