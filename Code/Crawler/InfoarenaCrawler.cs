using NerdvanaCrawler.Code.Network;

namespace NerdvanaCrawler.Code.Crawler
{
    public class InfoarenaCrawler : ArenaCrawler
    {
        protected override string ShortRootUrl => "infoarena.ro";
        protected override string RootUrl => "https://www.infoarena.ro";

        public InfoarenaCrawler(INetworkClient networkClient) : base(networkClient) { }
    }
}
