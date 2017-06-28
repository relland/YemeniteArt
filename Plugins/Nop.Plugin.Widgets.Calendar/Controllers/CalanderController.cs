using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Controllers
{
    public class CalanderController : BasePluginController
    {
        private readonly IProductService _productService;
        private readonly IViewTrackingService _viewTrackingService;
        private readonly IWorkContext _workContext;

        public CalanderController(IWorkContext workContext,
            IViewTrackingService viewTrackingService,
            IProductService productService,
            IPluginFinder pluginFinder)
        {
            _workContext = workContext;
            _viewTrackingService = viewTrackingService;
            _productService = productService;
        }
        //rel
        //OpenEvent
        //public int IntA { get; set; } //NumberOfSessions
        //public int IntB { get; set; } //EachSessionLength
        private MonthDayModel GetMonthDayModel(int year, int month, int day, Customer customer, int fromHour, int toHour)
        {
            var currentDate = new DateTime(year, month, day);
            var model = new MonthDayModel()
            {
                DayOfMonth = day,
                Month = month,
                Year = year,
                FromHour = fromHour,
                ToHour = toHour,
                MonthName = currentDate.ToString("MMMM"),
                DayName = currentDate.DayOfWeek.ToString(),
                Year14 = year - 2000
            };

            var openEventsList = _eventsService.GetAllOpenEventsInCurrentDay(currentDate);
            if (openEventsList.Count > 0)
            {
                model.AddEvent = new AddEventModel()
                {
                    AvailabeleHours = "",
                    Year = year.ToString(),
                    Month = month.ToString(),
                    Day = day.ToString(),
                    Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone)
                };
                model.HasOpenEvents = true;
                foreach (var openEvent in openEventsList)
                {
                    var oeModel = new OpenEventModel()
                    {
                        StartsAt = openEvent.StartsAt,
                        EachSessionLength = openEvent.IntB,
                        EndsAt = openEvent.EndsAt,
                        NumberOfSessions = openEvent.IntA,
                        Events = GetOpenEventsEvents(openEvent.Id, customer),
                        FrontId = openEvent.StartsAt.Hour >= fromHour && openEvent.StartsAt.Hour < toHour ? (openEvent.IntB * openEvent.IntA) / 15 : 0
                    };
                    model.OpenEvents.Add(oeModel);
                    if (model.AddEvent.FromHour == 0)
                    {
                        model.AddEvent.FromHour = oeModel.StartsAt.Hour;
                        model.AddEvent.FromMinute = oeModel.StartsAt.Minute;
                        model.AddEvent.ToHour = oeModel.EndsAt.Hour;
                        model.AddEvent.ToMinute = oeModel.EndsAt.Minute;
                        model.AddEvent.AvailabeleHours += oeModel.StartsAt.TimeOfDay + " - " + oeModel.EndsAt.TimeOfDay + ", ";

                        model.AddEvent.MinHour = oeModel.StartsAt.Hour;
                        model.AddEvent.MinMinute = oeModel.StartsAt.Minute;
                        model.AddEvent.MaxHour = oeModel.EndsAt.Hour;
                        model.AddEvent.MaxMinute = oeModel.EndsAt.Minute;
                    }
                    else
                    {
                        if (model.AddEvent.FromHour > oeModel.StartsAt.Hour)
                        {
                            model.AddEvent.FromHour = oeModel.StartsAt.Hour;
                            model.AddEvent.FromMinute = oeModel.StartsAt.Minute;

                            model.AddEvent.MinHour = oeModel.StartsAt.Hour;
                            model.AddEvent.MinMinute = oeModel.StartsAt.Minute;
                        }
                        else if (model.AddEvent.FromHour == oeModel.StartsAt.Hour && model.AddEvent.FromMinute > oeModel.StartsAt.Minute)
                        {
                            model.AddEvent.FromMinute = oeModel.StartsAt.Minute;

                            model.AddEvent.MinMinute = oeModel.StartsAt.Minute;
                        }
                        if (model.AddEvent.ToHour < oeModel.StartsAt.Hour)
                        {
                            model.AddEvent.ToHour = oeModel.EndsAt.Hour;
                            model.AddEvent.ToMinute = oeModel.EndsAt.Minute;

                            model.AddEvent.MaxHour = oeModel.EndsAt.Hour;
                            model.AddEvent.MaxMinute = oeModel.EndsAt.Minute;
                        }
                        else if (model.AddEvent.ToHour == oeModel.EndsAt.Hour && model.AddEvent.ToMinute < oeModel.EndsAt.Minute)
                        {
                            model.AddEvent.ToMinute = oeModel.EndsAt.Minute;

                            model.AddEvent.MaxMinute = oeModel.EndsAt.Minute;
                        }
                        model.AddEvent.AvailabeleHours += oeModel.StartsAt.TimeOfDay + " - " + oeModel.EndsAt.TimeOfDay + ", ";
                    }
                }
            }
            else
            {
                model.HasOpenEvents = false;
            }
            GetMiniMonthDayInfo(model, new DateTime(year, month, day, fromHour, 0, 0), fromHour, toHour);//(9am to 8pm)
            return model;
        }

        //rel
        private List<EventModel> GetOpenEventsEvents(int openEventId, Customer customer)
        {
            var list = new List<EventModel>();
            var events = _eventsService.GetEventsByOpenEventId(openEventId);
            foreach (var evnt in events)
            {
                var currentCustomerEvent = evnt.Customer.Id == customer.Id;
                var eventModel = new EventModel()
                {
                    CustomerComment = currentCustomerEvent ? evnt.CustomerComment : "",
                    Email = currentCustomerEvent ? evnt.ContactEmail : "",
                    Language = currentCustomerEvent ? evnt.Language : "",
                    PhoneNumber = currentCustomerEvent ? evnt.ContactPhoneNumbet : "",
                    //SessionLength = currentCustomerEvent ? evnt : "",
                    MyEvent = currentCustomerEvent,
                    StartsAt = evnt.StartsAtUtc
                };
                list.Add(eventModel);
            }
            return list;
        }

        //rel datetime -> open event -> event 
        private void GetMiniMonthDayInfo(MonthDayModel model, DateTime startTime, int fromHour, int toHour)
        {
            var block = 15;
            var numBlocks = 4 * (toHour - fromHour + 1);//quorte hour - 4 (hour) * 12 hours (9am to 8pm)
            var endTime = startTime.AddMinutes((double)block);
            for (int i = 0; i < numBlocks; i++)
            {
                var currentOpenEvent = model.OpenEvents.Where(x => x.StartsAt <= startTime && x.EndsAt >= endTime).ToList();
                var currentEvents = new List<EventModel>();
                var myEvent = false;
                var hasEvent = false;
                foreach (var oe in currentOpenEvent)
                {
                    if (oe.Events.Count > 0)
                    {
                        foreach (var e in oe.Events.Where(x => x.StartsAt <= startTime && startTime.AddMinutes(x.SessionLength) >= startTime).ToList())
                        {
                            hasEvent = true;
                            myEvent = myEvent ? myEvent : e.MyEvent;
                        }
                    }
                }
                var frontModel = new FrontDisplayOpenEventsModel()
                {
                    DisplayOrder = i + 1,
                    HasOpenEvent = currentOpenEvent.Count > 0,
                    HasEvent = hasEvent,
                    MyEvent = myEvent
                };
                model.FrontDisplayOpenEvents.Add(frontModel);
                startTime = startTime.AddMinutes((double)block);
                endTime = endTime.AddMinutes((double)block);
            }
            model.FrontDisplayOpenEvents = model.FrontDisplayOpenEvents.OrderBy(x => x.DisplayOrder).ToList();
        }

        //rel
        [HttpPost]
        public ActionResult AddEvent(AddEventModel model)
        {
            //tourguide role
            var toureguideRole = _customerService.GetCustomerRoleBySystemName("TourGuide");
            var customer = _workContext.CurrentCustomer;

            if (!IsCurrentUserRegistered() || !customer.CustomerRoles.Contains(toureguideRole))
                return new HttpUnauthorizedResult();

            var day = new DateTime(Int32.Parse(model.Year), Int32.Parse(model.Month), Int32.Parse(model.Day));
            //model.Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
            var openEvents = _eventsService.GetAllOpenEventsInCurrentDay(day);
            var addedEvent = false;
            var from = new TimeSpan(model.FromHour, model.FromMinute, 0);
            var to = new TimeSpan(model.ToHour, model.ToMinute, 0);
            if (openEvents != null && openEvents.Count > 0)
            {
                foreach (var oe in openEvents)
                {
                    if (_eventsService.EventCanBeAddedToOpenEvent(oe, from, to) && !addedEvent)
                    {
                        var evnt = new Event()
                        {
                            Active = true,
                            ContactEmail = customer.Email,
                            ContactPhoneNumbet = !string.IsNullOrWhiteSpace(model.Phone) ? model.Phone : customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                            Customer = customer,
                            CustomerComment = model.CustomerComment,
                            CustomerId = customer.Id,
                            Language = model.Language,
                            StartsAtUtc = new DateTime(day.Year, day.Month, day.Day, model.FromHour, model.FromMinute, 0),
                            EndsAtUtc = new DateTime(day.Year, day.Month, day.Day, model.ToHour, model.ToMinute, 0),
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow,
                        };
                        _eventsService.InsertEvent(evnt);
                        oe.AppliedEvents.Add(evnt);
                        _eventsService.UpdateOpenEvent(oe);
                        addedEvent = true;
                    }
                }
            }
            var eventMessage = addedEvent ? _localizationService.GetResource("rel.addedeventsuccess") : _localizationService.GetResource("rel.addedeventfail");
            return RedirectToAction("Events", new { eventMessage = eventMessage, success = addedEvent });
        }

        [NonAction]
        private void PrepareCustomerOpenEventsModel(CustomerEventsModel model, Customer customer, int month = 0, int year = 0)
        {
            if (month == 0 && year == 0)
            {
                month = DateTime.UtcNow.Month;
                year = DateTime.UtcNow.Year;
            }
            var date = new DateTime(year, month, 1);

            var totalMonthDays = DateTime.DaysInMonth(year, month);

            for (var day = 1; day <= totalMonthDays; day++)
            {
                var monthDay = new MonthDayModel()
                {
                    DayOfMonth = day,
                    Month = month,
                    Year = year,
                };
                var openEvents = _eventsService.GetAllOpenEvents(date);
                monthDay.HasOpenEvents = openEvents.Count > 0;
                foreach (var oe in openEvents)
                {
                    var oem = new OpenEventModel()
                    {
                        StartsAt = oe.StartsAt,
                        NumberOfSessions = oe.IntA,
                        EachSessionLength = oe.IntB,
                        MySessions = _eventsService.GetCustomerSessionsForCurrentOpenEvent(customer, oe)
                    };
                    monthDay.OpenEvents.Add(oem);
                }
                model.MonthDays.Add(monthDay);
            }
            //var allOpenEvents = _eventsService.GetAllOpenEvents(0, 0, 0, month, 1, year, month, 31, year);
            //foreach (var oe in allOpenEvents)
            //{
            //    var oem = new OpenEventModel()
            //    {
            //        EndsAt = oe.EndsAt,
            //        StartsAt = oe.StartsAt,
            //        Events = PrepareCustomerEventsModel(oe, customer),
            //        FrontId = oe.Id
            //    };
            //    model.MonthDays.Add(.Add(oem);
            //}
        }

        [NonAction]
        private List<EventModel> PrepareCustomerEventsModel(OpenEvent openEvent, Customer customer)
        {
            var events = new List<EventModel>();
            if (openEvent == null || customer == null)
                return events;
            foreach (var evnt in openEvent.AppliedEvents)
            {
                if (evnt.CustomerId == customer.Id)
                {
                    var em = new EventModel()
                    {
                        StartsAt = evnt.StartsAtUtc,
                        //EndsAt = evnt.EndsAtUtc,
                        CustomerComment = evnt.CustomerComment,
                        AdminComment = evnt.AdminComment,
                        Email = evnt.ContactEmail,
                        PhoneNumber = evnt.ContactPhoneNumbet,
                        Language = evnt.Language,
                        MyEvent = true,
                        FrontId = evnt.Id
                    };
                    events.Add(em);
                }
                else
                {
                    var em = new EventModel()
                    {
                        StartsAt = evnt.StartsAtUtc,
                        //EndsAt = evnt.EndsAtUtc,
                        MyEvent = false
                    };
                    events.Add(em);
                }
            }
            return events;
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Events(CustomerEventsModel model)
        //{

        //}

    }
}