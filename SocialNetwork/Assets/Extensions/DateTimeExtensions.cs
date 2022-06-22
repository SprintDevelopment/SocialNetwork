using System;

namespace SocialNetwork.Assets.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToTimesAgo(this DateTime date)
        {
            var days = DateTime.Now.Subtract(date).Days;
            switch (days)
            {
                case 0:
                    return "امروز";
                case 1:
                    return "دیروز";
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    return $"{days} روز پیش";
            }

            var weeks = DateTime.Now.Subtract(date).Days / 7;
            if (weeks < 5)
                return $"{weeks} هفته پیش";

            var months = DateTime.Now.Subtract(date).Days / 30;
            if (weeks < 12)
                return $"{weeks} ماه پیش";

            var years = DateTime.Now.Subtract(date).Days / 365;
            return $"{years} سال پیش";
        }

        public static string ToShortDate(this DateTime date)
        {
            var persianDate = PersianUtil.PersianDateTime(date);
            return persianDate.Substring(5);
        }
    }
}
