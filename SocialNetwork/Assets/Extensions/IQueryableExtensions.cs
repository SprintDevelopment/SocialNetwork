using SocialNetwork.Assets.Values.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Assets.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber = 0)
        {
            return query.Skip(pageNumber * SizeConstants.PAGE_SIZE).Take(SizeConstants.PAGE_SIZE);
        }
    }
}
