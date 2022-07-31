using SocialNetwork.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Assets.Dtos;

namespace SocialNetwork.Assets.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> list, int chunkSize)
        {
            return list.IsNullOrEmpty() ? null : 
                list.Select((item, i) => new { item, i })
                    .GroupBy(g => g.i / chunkSize)
                    .Select(g => g.Select(i => i.item));
        }

        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, string idPropertyName, string titlePropertyName, int? selectedValue)
        {
            if (items == null)
                return new List<SelectListItem>();

            return from item in items
                   select new SelectListItem
                   {
                       Value = item.GetPropertyValue(idPropertyName),
                       Text = item.GetPropertyValue(titlePropertyName),
                       Selected = selectedValue != null && item.GetPropertyValue(idPropertyName).Equals(selectedValue.ToString())
                   };
        }
        public static PaginationDto<T> Paginate<T>(this IEnumerable<T> query, string url, int offset = 0, int limit = 0)
        {
            return new PaginationDto<T> 
            {
                Results = query.Skip(offset).Take(limit > 0 ? limit : int.MaxValue),
                Previous = offset == 0 || limit == 0? "" : url.ChangeParameter(new KeyValue { Key = "offset", Value = Math.Max(0, offset - limit).ToString() }),
                Next = offset + limit >= query.Count() || limit == 0 ? "" : url.ChangeParameter(new KeyValue { Key = "offset", Value = (offset + limit).ToString() }),
            };
        }

        public static string Join(this IEnumerable<string> items, string separatotr)
        {
            if (items.IsNullOrEmpty())
                return "";

            return string.Join(separatotr, items);
        }

        public static bool HasAnyItemIn<T>(this IEnumerable<T> items, params T[] list)
        {
            return items.Any(item => list.Contains(item));
        }
    }
}
