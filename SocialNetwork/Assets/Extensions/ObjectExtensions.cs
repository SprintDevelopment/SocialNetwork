using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SocialNetwork.Assets.Extensions
{
    public static class ObjectExtensions
    {
        public static List<PropertyInfo> GetNavigationProperties<T>(this T obj)
        {
            var type = obj.GetType();
            var navigationProperties = new List<PropertyInfo>();
            foreach (var property in type.GetProperties())
            {
                var foreignAttribute = (ForeignKeyAttribute)property.GetCustomAttribute(typeof(ForeignKeyAttribute));
                if (foreignAttribute != null || property.GetMethod.IsVirtual)
                    navigationProperties.Add(property);
            }

            return navigationProperties;
        }

        public static PropertyInfo GetKeyProperty<T>(this T obj)
        {
            var type = obj.GetType();
            return type.GetProperties().SingleOrDefault(p => (KeyAttribute)p.GetCustomAttribute(typeof(KeyAttribute)) != null);
        }
        public static string GetPropertyValue<T>(this T item, string propertyName)
        {
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        }

        public static T LightClone<T>(this T obj) where T : BaseModel
        {
            var deepClone = obj.Clone();
            
            foreach (var property in GetNavigationProperties(obj))
                property.SetValue(deepClone, null);

            return (T)deepClone;
        }

        public static bool ExistsIn<T>(this T source, params T[] others)
        {
            return others.Length == 0 || others.Any(x => EqualityComparer<T>.Default.Equals(source, x));
        }
    }
}
