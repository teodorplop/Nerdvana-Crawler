namespace NerdvanaCrawler.Code.Network
{
    public interface INetworkClient
    {
        Task<NetworkResponse> Get(string url);
    }
}
