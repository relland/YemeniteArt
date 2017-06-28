using Nop.Core;
using Nop.Plugin.Widgets.Calendar.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Services
{
    public interface ICalendarService
    {
        //events
        IList<Session> GetAllEventsByOpenEventId(OpenEvent openEvent);

        Session GetEventById(int id);

        IList<Session> GetAllEventsInCurrentDay(DateTime date = new DateTime());

        void InsertEvent(Session evnt);

        void UpdateEvent(Session evnt);

        void DeleteEvent(Session evnt);

        List<Session> GetEventsByOpenEventId(int id);

        //open events
        List<OpenEvent> GetAllOpenEvents(DateTime date = new DateTime());

        OpenEvent GetOpenEventById(int id);

        OpenEvent GetOpenEventByEventId(int id);

        IList<OpenEvent> GetAllOpenEventsInCurrentDay(DateTime date = new DateTime());

        //new
        IPagedList<OpenEvent> GetAllOpenEvents(
            int searchDateMonth = 0, int searchDateDay = 0, int searchDateYear = 0,
            int searchFromDateMonth = 0, int searchFromDateDay = 0, int searchFromDateYear = 0,
            int searchToDateMonth = 0, int searchToDateDay = 0, int searchToDateYear = 0,
            int monthPast = 0, int monthNext = 0,
            int pageIndex = 0, int pageSize = 2147483647);

        List<int> GetCustomerSessionsForCurrentOpenEvent(Customer customer, OpenEvent openEvent);
        /// <summary>
        /// Insert an openEvent
        /// </summary>
        /// <param name="openEvent">openEvent</param>
        void InsertOpenEvent(OpenEvent openEvent);

        /// <summary>
        /// Updates the openEvent
        /// </summary>
        /// <param name="openEvent">openEvent</param>
        void UpdateOpenEvent(OpenEvent openEvent);

        /// <summary>
        /// Delete an openEvent
        /// </summary>
        /// <param name="openEvent">openEvent</param>
        void DeleteOpenEvent(OpenEvent openEvent);

        bool EventCanBeAddedToOpenEvent(OpenEvent openEvent, TimeSpan from, TimeSpan to);
    }
}
