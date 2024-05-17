
namespace NerdvanaCrawler.Code.Network
{
    public class NetworkClientCache : INetworkClient
    {
        private INetworkClient m_NetworkClient;
        private Dictionary<string, NetworkResponse> m_CachedResponses = new Dictionary<string, NetworkResponse>();

        public NetworkClientCache(INetworkClient networkClient)
        {
            m_NetworkClient = networkClient;
        }

        public async Task<NetworkResponse> Get(string url)
        {
            if (!m_CachedResponses.TryGetValue(url, out var response))
            {
                response = await m_NetworkClient.Get(url);
                if (response.IsSuccess)
                    m_CachedResponses.Add(url, response);
            }

            return response;
        }
    }
}
