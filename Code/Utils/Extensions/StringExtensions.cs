namespace NerdvanaCrawler.Code.Utils.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of any
        /// string in a specified array of Unicode strings.
        /// </summary>
        public static int IndexOfAny(this string value, string[] anyOf)
        {
            if (string.IsNullOrEmpty(value) || anyOf == null)
                return -1;

            int index, i;

            index = -1;
            i = 0;
            while (i < anyOf.Length && index == -1)
                index = value.IndexOf(anyOf[i++]);

            return index;
        }
    }
}
