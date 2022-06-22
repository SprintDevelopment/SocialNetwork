using SocialNetwork.Assets.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace SocialNetwork.Assets.Extensions
{
    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue) where TAttribute : Attribute
        {
            return enumValue.GetType().GetMember(enumValue.ToString())
                            .First().GetCustomAttribute<TAttribute>();
        }

        public static IEnumerable<EnumDto> ToCollection(this Type enumType)
        {
            var enumValues = Enum.GetValues(enumType);

            return (from object enumValue in enumValues select new EnumDto(enumValue, ((Enum)enumValue).GetDescription()));
        }

        public static string GetDescription(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName();
        }

        public static T AddFlaggedMember<T>(this Enum enumValue, T enumValueToAdd)
        {
            return (T)(object)((int)(object)enumValue | (int)(object)enumValueToAdd);
        }

        public static T RemoveFlaggedMember<T>(this Enum enumValue, T enumValueToAdd)
        {
            return (T)(object)((int)(object)enumValue & ~(int)(object)enumValueToAdd);
        }
    }
}
