using System;

namespace SocialNetwork.Assets.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrWhitespace(this string stringToCheck)
        {
            return string.IsNullOrWhiteSpace(stringToCheck);
        }
        public static bool ContainsInsensitive(this string str, string value) => str?.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
