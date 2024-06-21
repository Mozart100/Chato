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

        public static List<T> SafeToList<T>(this IEnumerable<T> collection)
        {
            if (collection.IsNullOrEmpty())
            {
                return new List<T>();
            }

            return collection.ToList();
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

        public static TSource[] SafeToArray<TSource>(this IEnumerable<TSource> collection)
        {
            var result = new TSource[] { };

            if (!collection.IsNullOrEmpty())
            {
                var list = new List<TSource>();
                foreach (var item in collection)
                {
                    list.Add(item);
                }

                result = list.ToArray();
            }

            return result;
        }



        public static IEnumerable<TTarget> SafeSelect<TSource, TTarget>(this IEnumerable<TSource> collection, Func<TSource, TTarget> func)
        {
            if (!collection.IsNullOrEmpty())
            {
                foreach (var item in collection)
                {
                    if (item != null)
                    {
                        yield return func(item);
                    }
                }
            }
        }
    }
}