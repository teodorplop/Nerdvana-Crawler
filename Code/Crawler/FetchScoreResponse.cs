namespace NerdvanaCrawler.Code.Crawler
{
    public enum EFetchScoreResult
    {
        NoCrawler, InvalidInput, NetworkError, ParserError, NoSolutions, CompilationError, Success
    }

    public static class EFetchScoreResultExtensions
    {
        public static bool IsError(this EFetchScoreResult fetchScoreResult)
        {
            return fetchScoreResult switch
            {
                EFetchScoreResult.NetworkError => true,
                EFetchScoreResult.ParserError => true,
                EFetchScoreResult.NoCrawler => true,
                EFetchScoreResult.InvalidInput => true,
                _ => false,
            };
        }
    }

    public struct FetchScoreResponse
    {
        public EFetchScoreResult Result { get; }
        public int Score { get; }
        public string ResultUrl { get; private set; }

        public FetchScoreResponse(EFetchScoreResult result, int score)
        {
            Result = result;
            Score = score;
            ResultUrl = string.Empty;
        }

        public FetchScoreResponse WithResultUrl(string resultUrl)
        {
            ResultUrl = resultUrl;
            return this;
        }
    }
}
