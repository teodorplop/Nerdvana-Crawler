using NerdvanaCrawler.Code.Network;

namespace NerdvanaCrawler.Code.Crawler
{
    public class NerdarenaCrawler : ArenaCrawler
    {
        protected override string ShortRootUrl => "nerdarena.ro";
        protected override string RootUrl => "https://www.nerdarena.ro";

        public NerdarenaCrawler(INetworkClient networkClient) : base(networkClient) { }
    }
}
