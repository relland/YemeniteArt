using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar
{
    public static class Helper
    {
        public static void AddSessions(this string dayHours, DayOfWeek dayOfWeek, DateTime from, DateTime to, int sessionLengthMinutes)
        {
            while (from.Date <= to.Date)
            {
                if (from.DayOfWeek == dayOfWeek)
                {
                    //dayHours.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    //    .Select(x => x.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries))

                }
            }
        }
    }
}
