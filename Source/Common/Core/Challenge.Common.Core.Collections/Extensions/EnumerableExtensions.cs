namespace Challenge.Common.Core.Collections.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns true if the sequence is null or empty
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            if (source is null) return true;

            if (source is ICollection<T> collT) return collT.Count == 0;
            if (source is IReadOnlyCollection<T> roColl) return roColl.Count == 0;
            if (source.TryGetNonEnumeratedCount(out var count)) return count == 0;

            using var e = source.GetEnumerator();
            return !e.MoveNext();
        }
    }
}
