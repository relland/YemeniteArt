using Nop.Plugin.Widgets.Calendar.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar
{
    public static class Helper
    {
        public static List<Session> AddSessions(this string dayHours, DayOfWeek dayOfWeek, DateTime from, DateTime to, int sessionLengthMinutes, int sessionAvailablilityCustomerCount)
        {
            var list = new List<Session>();
            if (!dayHours.IsValidTimeStructure()) return list;
            while (from.Date <= to.Date)
            {
                if (from.DayOfWeek == dayOfWeek)
                {
                    //"10:00,15:30,09:00"
                    list.AddRange(dayHours.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => { return new Session(GetDate(from, x), sessionAvailablilityCustomerCount, sessionLengthMinutes) { }; }));
                }
                from.AddDays(1);
            }
            return list;
        }

        private static DateTime GetDate(DateTime date, string time)
        {
            var timeArr = time.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Select(x => { return Int32.Parse(x); }).ToArray();
            return new DateTime(date.Year, date.Month, date.Day, timeArr[0], timeArr[1], 0);
        }
        public static bool IsValidTimeStructure(this string dayHours)
        {
            var regex = "(1[0-9]:[0-5][0-9])|(2[0-4]:[0-5][0-9])";
            Regex r = new Regex(regex, RegexOptions.IgnoreCase);
            return dayHours.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .All(x => r.IsMatch(x));
        }
    }
}
