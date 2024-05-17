namespace NerdvanaCrawler.Code.Utils.Extensions
{
    public static class IReadOnlyListExtensions
    {
        public static int IndexOf<T>(this IReadOnlyList<T> list, T value)
        {
            if (list == null) return -1;

            int index = 0;
            while (index < list.Count && !EqualityComparer<T>.Default.Equals(list[index], value))
                ++index;

            return index == list.Count ? -1 : index;
        }

        public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
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
