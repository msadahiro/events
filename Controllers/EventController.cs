using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using events.Models;
using Microsoft.AspNetCore.Mvc;
using events.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace events.Controllers
{
    public class EventController : Controller
    {
        private MyContext _context;

        public EventController(MyContext context)
        {
            _context = context;
        }
        [HttpGet]
        [RouteAttribute("dashboard")]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("CurrentUser") == null)
            {
                return RedirectToAction("Login", "User");
            }

            int? getUserId = HttpContext.Session.GetInt32("CurrentUser");
            User SignedInUser = _context.users.Where(User => User.id == getUserId).SingleOrDefault();
            List<Event> getAllEvents = _context.events
                .OrderBy(time => time.EventDate)

                .Include(reserve => reserve.Attendings)
                    .ThenInclude(Reserve => Reserve.User)

                .ToList();
            ViewBag.dashboard = HttpContext.Session.GetString("notValid");
            ViewBag.Today = DateTime.Today;
            ViewBag.CurrentUser = SignedInUser;
            ViewBag.AllEvents = getAllEvents;
            return View();
        }


        //If compare start date is after the event start date and the event bleeds into compare
        //If event start date is after the compare start date and the compare bleeds into the event
        public bool checkingReservationTime(Event compareEvent)
        {
            int? getUserId = HttpContext.Session.GetInt32("CurrentUser");
            User SignedInUser = _context.users.Where(User => User.id == getUserId).SingleOrDefault();
            List<Reserve> signedUp = _context.reserves.Where(res => res.UserId == SignedInUser.id)
            // .Include(even => even.Event)
            .Where((eve) => ((eve.Event.EventDate < compareEvent.EventDate && eve.Event.EventDate.AddMinutes(eve.Event.Duration) > compareEvent.EventDate
            )
             || ((eve.Event.EventDate < compareEvent.EventDate && eve.Event.EventDate > compareEvent.EventDate.AddMinutes(eve.Event.Duration))
             || ((eve.Event.EventDate > compareEvent.EventDate && eve.Event.EventDate.AddMinutes(eve.Event.Duration) > compareEvent.EventDate))))
             )
            .ToList();
            // List<Event> getEvents = _context.events
            // .Where((eve) => ((eve.EventDate < compareEvent.EventDate && eve.EventDate.AddMinutes(eve.Duration) < compareEvent.EventDate
            // ) || ((eve.EventDate > compareEvent.EventDate && eve.EventDate > compareEvent.EventDate.AddMinutes(eve.Duration)))))
            // .ToList();
            if (signedUp.Count == 0)
            {
                return true;
            }
            return false;
        }

        [HttpGetAttribute]
        [RouteAttribute("new")]
        public IActionResult New()
        {
            var today = DateTime.Today;
            var todayString = today.ToString("yyyy-MM-dd");
            ViewBag.errors = new List<string>();
            ViewBag.Today = todayString;
            return View("New");
        }
        public List<Event> checkCreatedEvent(Event newEvent)
        {
            int? getUserId = HttpContext.Session.GetInt32("CurrentUser");
            List<Event> EventforUser = _context.events
                .Where(u => u.CreatedBy == getUserId)
                .Include(res => res.Attendings)
                    .ThenInclude(User => User.User)
                    
                .ToList();

            return EventforUser;
        }

        [HttpPost]
        [RouteAttribute("makeactivity")]
        public IActionResult MakeActivity(NewViewModel model, Event newEvent)
        {
            if (HttpContext.Session.GetInt32("CurrentUser") == null)
            {
                return RedirectToAction("Login", "User");
            }
            if (newEvent.Length == "Hours")
            {
                newEvent.Duration *= 60;
            }
            if (newEvent.Length == "Days")
            {
                newEvent.Duration *= 1440;
            }
            // Event makeEvent = _context.events.SingleOrDefault(o => o.EventDate == newEvent.EventDate);
            List<Event> userEvents = checkCreatedEvent(newEvent);
            bool isOkay = true;
            foreach (var entry in userEvents)
            {
                if ((entry.EventDate < newEvent.EventDate && entry.EventDate.AddMinutes(entry.Duration) > newEvent.EventDate) || (entry.EventDate < newEvent.EventDate && entry.EventDate > newEvent.EventDate.AddMinutes(newEvent.Duration)))
                {
                    isOkay = false;
                }
            }
            if (isOkay)
            {
                int? getUserId = HttpContext.Session.GetInt32("CurrentUser");
                if (ModelState.IsValid)
                {
                    newEvent.CreatedBy = (int)HttpContext.Session.GetInt32("CurrentUser");
                    _context.Add(newEvent);
                    _context.SaveChanges();
                    Reserve newRsvp = new Reserve()
                    {
                        UserId = (int)getUserId,
                        EventId = newEvent.id
                    };
                    _context.Add(newRsvp);
                    _context.SaveChanges();
                    return RedirectToAction("Activity", new { id = newEvent.id });
                }
            }
            ViewBag.errors = ModelState.Values;
            return View("New");

        }
        [HttpGet]
        [RouteAttribute("activity/{id}")]
        public IActionResult Activity(int id)
        {
            if (HttpContext.Session.GetInt32("CurrentUser") == null)
            {
                return RedirectToAction("Login", "User");
            }
            List<Event> showActivity = _context.events.Where(activity => activity.id == id)
                .Include(users => users.Attendings)
                    .ThenInclude(u => u.User)
                .ToList();
            int? getUserId = HttpContext.Session.GetInt32("CurrentUser");
            User SignedInUser = _context.users.Where(User => User.id == getUserId).SingleOrDefault();
            List<Event> getAllEvents = _context.events
                .OrderBy(time => time.EventDate)
                .Include(reserve => reserve.Attendings)
                    .ThenInclude(Reserve => Reserve.User)
                .ToList();
            ViewBag.CurrentUser = SignedInUser;
            ViewBag.AllEvents = getAllEvents;
            ViewBag.Activity = showActivity;
            return View("Activity");
        }
        [HttpGet]
        [RouteAttribute("reserve/{id}")]
        public IActionResult Reserve(int id)
        {
            Event joinThisEvent = _context.events.SingleOrDefault(activity => activity.id == id);
            if (checkingReservationTime(joinThisEvent) == true)
            {
                int? getUserId = HttpContext.Session.GetInt32("CurrentUser");
                Reserve newRsvp = new Reserve()
                {
                    UserId = (int)getUserId,
                    EventId = id
                };
                _context.Add(newRsvp);
                _context.SaveChanges();
                HttpContext.Session.SetString("notValid", "");
                return RedirectToAction("Dashboard");
            }
            HttpContext.Session.SetString("notValid", "Conflicting RSVP times");
            return RedirectToAction("dashboard");
        }
        [HttpGet]
        [RouteAttribute("removeRSVP/{id}")]
        public IActionResult RemoveRSVP(int id)
        {
            int? getUserId = HttpContext.Session.GetInt32("CurrentUser");
            Reserve remove = _context.reserves.Where(user => user.UserId == getUserId).Where(activity => activity.EventId == id).SingleOrDefault();
            _context.Remove(remove);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        [RouteAttribute("delete/{id}")]
        public IActionResult Delete(int id)
        {
            List<Reserve> deleteAll = _context.reserves.Where(activity => activity.EventId == id).ToList();
            foreach (var user in deleteAll)
            {
                _context.Remove(user);
                _context.SaveChanges();
            }
            Event deleteEvent = _context.events.Where(activity => activity.id == id).SingleOrDefault();
            _context.Remove(deleteEvent);
            _context.SaveChanges();
            return RedirectToAction("dashboard");
        }
    }
}