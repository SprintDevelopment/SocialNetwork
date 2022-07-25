using System;
using System.Web;

namespace SocialNetwork.Assets.Extensions
{
    public static class QueryExtensions
    {
        public static string ChangeParameter(this string url, params KeyValue[] keyValues)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var keyValue in keyValues)
                query[keyValue.Key] = keyValue.Value;
            
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.ToString();
        }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
