namespace Chato.Server.Infrastracture
{
    public static class CollectionAdditionalExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null || !collection.Any())
            {
                return true;
            }

            return false;
        }

        public static int SafeCount<T>(this IEnumerable<T> collection)
        {
            if (collection == null || !collection.Any())
            {
                return 0;
            }

            return collection.Count();
        }

        public static bool SafeAny<T>(this IEnumerable<T> collection)
        {
            if (collection == null || !collection.Any())
            {
                return false;
            }

            return true;
        }
    }
}