namespace Chato.Server.Infrastracture
{
    public static class StringAdditionalExtensionsClass
    {
        public static bool IsEmpty(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }

            return false;
        }

        public static bool IsNotEmpty(this string str)
        {
            return !str.IsEmpty();
        }

        public static bool SafeEqual(this string source, string target)
        {
            var str1 = source ?? string.Empty;
            var str2 = target ?? string.Empty;

            return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }
    }
}