using NerdvanaCrawler.Code.Utils;

namespace NerdvanaCrawler.Code.Network
{
    public class NetworkClient : INetworkClient
    {
        private const string K_USER_AGENT = "Infoarena-Monitor/1.0 (Contact: your-email@example.com)";
        
        private HttpClient m_HttpClient;

        public NetworkClient()
        {
            m_HttpClient = new HttpClient();
            m_HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(K_USER_AGENT);
        }

        public async Task<NetworkResponse> Get(string url)
        {
            this.Log($"Sending GET request to {url}");

            var response = new NetworkResponse();
            try
            {
                var networkResponse = await m_HttpClient.GetAsync(url);

                response.IsSuccess = networkResponse.IsSuccessStatusCode;
                if (networkResponse.IsSuccessStatusCode)
                {
                    this.Log($"GET request to {url} was successful!");
                    response.Content = await networkResponse.Content.ReadAsStringAsync();
                }
                else
                {
                    this.LogError($"GET request to {url} failed.");
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                this.LogError($"GET request to {url} failed. Exception: {ex.Message}");
            }

            return response;
        }
    }
}
