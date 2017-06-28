using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.Calendar.Domain;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IRepository<Session> _eventRepository;
        private readonly IRepository<OpenEvent> _openEventRepository;
        private readonly IEventPublisher _eventPublisher;

        public CalendarService(IRepository<Session> eventRepository, IRepository<OpenEvent> openEventRepository, IEventPublisher eventPublisher)
        {
            _eventRepository = eventRepository;
            _openEventRepository = openEventRepository;
            _eventPublisher = eventPublisher;
        }

        //events
        public virtual IList<Session> GetAllEventsByOpenEventId(OpenEvent openEvent)
        {
            var ee = openEvent.AppliedEvents;
            var query = _eventRepository.Table;
            query = query.Where(x => x.Active && !x.Deleted && x.StartsAtUtc == DateTime.UtcNow);
            return query.ToList();
        }

        public virtual Session GetEventById(int id)
        {
            var evnt = _eventRepository.Table.Where(x => x.Id == id).Single();
            if (evnt == null)
                evnt = new Session();
            return evnt;
        }

        public virtual IList<Session> GetAllEventsInCurrentDay(DateTime date = new DateTime())
        {
            if (date == null)//change this
                date = DateTime.UtcNow;
            var query = _eventRepository.Table;
            query = query.Where(x => x.Active && !x.Deleted && x.StartsAtUtc == date);
            return query.ToList();
        }

        public virtual void InsertEvent(Session evnt)
        {
            if (evnt == null)
                throw new ArgumentNullException("event");

            _eventRepository.Insert(evnt);

            //event notification
            _eventPublisher.EntityInserted(evnt);
        }

        public virtual void UpdateEvent(Session evnt)
        {
            if (evnt == null)
                throw new ArgumentNullException("event");

            _eventRepository.Update(evnt);

            //event notification
            _eventPublisher.EntityUpdated(evnt);
        }

        public virtual void DeleteEvent(Session evnt)
        {
            if (evnt == null)
                throw new ArgumentNullException("event");

            _eventRepository.Delete(evnt);

            //event notification
            _eventPublisher.EntityDeleted(evnt);
        }

        public virtual List<Session> GetEventsByOpenEventId(int id)
        {
            var openEvent = GetOpenEventById(id);
            if (openEvent == null)
            {

            }
            return _openEventRepository.Table.Where(x => x.Id == id).First().AppliedEvents.ToList();

        }

        //open events
        public virtual List<OpenEvent> GetAllOpenEvents(DateTime date = new DateTime())
        {
            if (date == null)//change this
                date = DateTime.UtcNow;
            var query = _openEventRepository.Table;
            query = query.Where(x => x.Active && !x.Deleted && x.StartsAt == date);
            return query.ToList();
        }

        public virtual OpenEvent GetOpenEventById(int id)
        {
            var openEvnt = _openEventRepository.Table.Where(x => x.Id == id).Single();
            if (openEvnt == null)
                openEvnt = new OpenEvent();
            return openEvnt;
        }

        public virtual OpenEvent GetOpenEventByEventId(int id)
        {
            var evnt = _eventRepository.Table.Where(x => x.Id == id).Single();
            var query = _openEventRepository.Table;
            query = query.Where(x => x.AppliedEvents.Count > 0);
            var queryList = query.Where(x => !x.Deleted).ToList();
            foreach (var openEvent in queryList)
            {
                if (openEvent.AppliedEvents.Contains(evnt))
                    return openEvent;
            }
            return null;
        }

        public virtual IList<OpenEvent> GetAllOpenEventsInCurrentDay(DateTime date = new DateTime())
        {
            if (date == null)//change this
                date = DateTime.UtcNow;
            var query = _openEventRepository.Table;
            query = query.Where(x => x.Active && !x.Deleted && x.StartsAt.Month == date.Month && x.StartsAt.Year == date.Year && x.StartsAt.Day == date.Day);
            return query.ToList();
        }

        //new
        public virtual IPagedList<OpenEvent> GetAllOpenEvents(
            int searchDateMonth = 0, int searchDateDay = 0, int searchDateYear = 0,
            int searchFromDateMonth = 0, int searchFromDateDay = 0, int searchFromDateYear = 0,
            int searchToDateMonth = 0, int searchToDateDay = 0, int searchToDateYear = 0,
            int monthPast = 0, int monthNext = 0,
            int pageIndex = 0, int pageSize = 2147483647)
        {
            var query = _openEventRepository.Table;
            if (searchDateMonth != 0 && searchDateDay != 0 && searchDateYear != 0)
                query = query.Where(c => c.StartsAt.Month == searchDateMonth && c.StartsAt.Day == searchDateDay && c.StartsAt.Year == searchDateYear);
            else if (searchFromDateMonth != 0 && searchFromDateDay != 0 && searchFromDateYear != 0 && searchToDateMonth == 0 && searchToDateDay == 0 && searchToDateYear == 0)
                query = query.Where(c => c.StartsAt.Month <= searchFromDateMonth && c.StartsAt.Day <= searchFromDateDay && c.StartsAt.Year <= searchFromDateYear);
            else if (searchFromDateMonth != 0 && searchFromDateDay != 0 && searchFromDateYear != 0 && searchToDateMonth != 0 && searchToDateDay != 0 && searchToDateYear != 0)
                query = query.Where(c => c.StartsAt.Month >= searchFromDateMonth && c.StartsAt.Day >= searchFromDateDay && c.StartsAt.Year >= searchFromDateYear && c.EndsAt.Month <= searchToDateMonth && c.EndsAt.Day <= searchToDateDay && c.EndsAt.Year <= searchToDateYear);

            if (monthPast > 0)
                query = query.Where(c => c.StartsAt.Month == monthPast);
            else if (monthNext > 0)
                query = query.Where(c => c.StartsAt.Month == monthNext);

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var openEvents = new PagedList<OpenEvent>(query, pageIndex, pageSize);
            return openEvents;
        }

        public virtual List<int> GetCustomerSessionsForCurrentOpenEvent(Customer customer, OpenEvent openEvent)
        {
            var sessionIds = new List<int>();
            foreach (var evt in openEvent.AppliedEvents)
            {
                if (evt.CustomerId == customer.Id)
                    sessionIds.Add(evt.IntC);
            }
            return sessionIds;
        }
        /// <summary>
        /// Insert an openEvent
        /// </summary>
        /// <param name="openEvent">openEvent</param>
        public virtual void InsertOpenEvent(OpenEvent openEvent)
        {
            if (openEvent == null)
                throw new ArgumentNullException("openEvent");

            _openEventRepository.Insert(openEvent);

            //event notification
            _eventPublisher.EntityInserted(openEvent);
        }

        /// <summary>
        /// Updates the openEvent
        /// </summary>
        /// <param name="openEvent">openEvent</param>
        public virtual void UpdateOpenEvent(OpenEvent openEvent)
        {
            if (openEvent == null)
                throw new ArgumentNullException("openEvent");

            _openEventRepository.Update(openEvent);

            //event notification
            _eventPublisher.EntityUpdated(openEvent);
        }

        /// <summary>
        /// Delete an openEvent
        /// </summary>
        /// <param name="openEvent">openEvent</param>
        public virtual void DeleteOpenEvent(OpenEvent openEvent)
        {
            if (openEvent == null)
                throw new ArgumentNullException("openEvent");

            openEvent.Deleted = true;

            UpdateOpenEvent(openEvent);
        }

        public bool EventCanBeAddedToOpenEvent(OpenEvent openEvent, TimeSpan from, TimeSpan to)
        {
            //validate time
            if (openEvent.StartsAt.TimeOfDay <= from && openEvent.EndsAt.TimeOfDay >= to)
                if (from < to)
                {
                    if (openEvent.AppliedEvents.Count == 0)
                        return true;
                    else
                    {
                        foreach (var evnt in openEvent.AppliedEvents)
                        {
                            if (evnt.StartsAtUtc.TimeOfDay <= from && evnt.EndsAtUtc.TimeOfDay >= from)
                                return false;
                            if (evnt.StartsAtUtc.TimeOfDay <= to && evnt.EndsAtUtc.TimeOfDay >= to)
                                return false;
                        }
                    }
                    return true;
                }
            return false;
        }
    }
}
