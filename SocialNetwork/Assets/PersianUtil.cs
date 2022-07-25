using SocialNetwork.Assets.Values.Enums;
using System;
using System.Globalization;

namespace SocialNetwork.Assets
{
    public static class PersianUtil
    {
        public static string ToPersianDateTime(this DateTime dateToConvert)
        {
            if (dateToConvert == DateTime.MinValue)
                dateToConvert = DateTime.Now;

            var persianCalendar = new PersianCalendar();

            int day = persianCalendar.GetDayOfMonth(dateToConvert);
            int month = persianCalendar.GetMonth(dateToConvert);
            int year = persianCalendar.GetYear(dateToConvert);

            return $"{year:0000}-{month:00}-{day:00}T" + dateToConvert.ToString("O").Split('T')[1];
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
