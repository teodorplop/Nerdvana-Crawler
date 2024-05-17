namespace NerdvanaCrawler.Code.Utils
{
    public enum ELogLevel
    {
        Log, Warning, Error
    }

    public static class Logger
    {
        private static string GetTag(object caller)
        {
            return caller is Type callerType ? callerType.Name : caller.GetType().Name;
        }
        private static void Log(object caller, object message, ELogLevel logLevel)
        {
            Console.WriteLine($"[{logLevel}] [{GetTag(caller)}] {message}");
        }

        public static void Log(this object caller, object message) => Log(caller, message, ELogLevel.Log);

        public static void LogWarning(this object caller, object message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log(caller, message, ELogLevel.Warning);
            Console.ForegroundColor = previousColor;
        }

        public static void LogError(this object caller, object message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Log(caller, message, ELogLevel.Error);
            Console.ForegroundColor = previousColor;
        }
    }
}
