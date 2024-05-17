namespace NerdvanaCrawler.Code.Utils.Extensions
{
    internal static class IListExtensions
    {
        public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate)
        {
            if (list == null)
                return -1;

            int i = 0;
            while (i < list.Count && !predicate(list[i]))
                ++i;

            return i < list.Count ? i : -1;
        }
    }
}
