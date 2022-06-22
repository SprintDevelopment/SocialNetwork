using SocialNetwork.Assets.Values.Enums;
using System;
using System.Globalization;

namespace SocialNetwork.Assets
{
    public class PersianUtil
    {
        public static string PersianDateTime(DateTime dateToConvert, bool isShortFormat = true)
        {
            if (dateToConvert == DateTime.MinValue)
                dateToConvert = DateTime.Now;

            var persianCalendar = new PersianCalendar();

            int day = persianCalendar.GetDayOfMonth(dateToConvert);
            int month = persianCalendar.GetMonth(dateToConvert);
            int year = persianCalendar.GetYear(dateToConvert);

            if (isShortFormat)
                return $"{year:0000}/{month:00}/{day:00}";
            //return $"{year:0000}/{month:00}/{day:00} - {dateToConvert.Hour:00}:{dateToConvert.Minute:00}";

            var persinaDayOfWeek = PersianDayOfWeek(dateToConvert);
            var persianMonth = (PersianMonth)month;

            //return $"{persinaDayOfWeek.ToString()}، {day} {persianMonth.ToString()} {year} - {dateToConvert.Hour:00}:{dateToConvert.Minute:00}";
            return $"{day} {persianMonth.ToString()} {year}";
        }

        public static PersianDayOfWeek PersianDayOfWeek(DateTime dateToConvert)
        {
            var persianCalendar = new PersianCalendar();

            return (PersianDayOfWeek)(((short)persianCalendar.GetDayOfWeek(dateToConvert) + 1) % 7 + 1);
        }

        public static bool GeorgianDateTime(string persianDate, out DateTime result)
        {
            try
            {
                var persianCalendar = new PersianCalendar();
                string[] dateParts = persianDate.Split('/');

                result = persianCalendar.ToDateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]), 0, 0, 0, 0);
                return true;
            }
            catch { result = DateTime.Now; }

            return false;
        }

    }
}
