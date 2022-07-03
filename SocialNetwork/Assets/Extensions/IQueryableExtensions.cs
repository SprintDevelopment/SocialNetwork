using SocialNetwork.Assets.Values.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Assets.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int offset = 0, int limit = 0)
        {
            return query.Skip(offset).Take(limit > 0 ? limit : int.MaxValue);
        }
    }
}
