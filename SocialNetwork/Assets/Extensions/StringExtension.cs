namespace SocialNetwork.Assets.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrWhitespace(this string stringToCheck)
        {
            return string.IsNullOrWhiteSpace(stringToCheck);
        }
    }
}
