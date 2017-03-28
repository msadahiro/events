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
    public class EventController : Controller{
        private MyContext _context;
 
        public EventController(MyContext context)
        {
            _context = context;
        }
        [HttpGet]
        [RouteAttribute("dashboard")]
        public IActionResult Dashboard(){
            if(HttpContext.Session.GetInt32("CurrentUser")==null){
                return RedirectToAction("Login","User");
            }
            
            int? getUserId = HttpContext.Session.GetInt32("CurrentUser");
            User SignedInUser = _context.users.Where(User => User.id == getUserId).SingleOrDefault();
            List<Event> getAllEvents = _context.events
                .OrderBy(time => time.EventDate)
                .Include(reserve => reserve.Attendings)
                    .ThenInclude(Reserve => Reserve.User)
                .ToList();
            ViewBag.Today = DateTime.Today;
            ViewBag.CurrentUser = SignedInUser;
            ViewBag.AllEvents = getAllEvents;
            return View();
        }
        [HttpGetAttribute]
        [RouteAttribute("new")]
        public IActionResult New(){
            var today = DateTime.Today;
            var todayString = today.ToString("yyyy-MM-dd");
            ViewBag.errors = new List<string>();
            ViewBag.Today = todayString;
            return View("New");
        }
        [HttpPost]
        [RouteAttribute("makeactivity")]
        public IActionResult MakeActivity(NewViewModel model, Event newEvent){
            if(HttpContext.Session.GetInt32("CurrentUser")==null){
                return RedirectToAction("Login","User");
            }
            int? getUserId = HttpContext.Session.GetInt32("CurrentUser");
            if(ModelState.IsValid){
                newEvent.CreatedBy = (int)HttpContext.Session.GetInt32("CurrentUser");
                _context.Add(newEvent);
                _context.SaveChanges();
                Reserve newRsvp = new Reserve(){
                    UserId = (int)getUserId,
                    EventId = newEvent.id
                };
                _context.Add(newRsvp);
                _context.SaveChanges();
                return RedirectToAction("Activity",new {id = newEvent.id});
            }
            else{
                ViewBag.errors = ModelState.Values;
                return View("New");
            }
        }
        [HttpGet]
        [RouteAttribute("activity/{id}")]
        public IActionResult Activity(int id){
            if(HttpContext.Session.GetInt32("CurrentUser")==null){
                return RedirectToAction("Login","User");
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
        public IActionResult Reserve(int id){
            int? getUserId = HttpContext.Session.GetInt32("CurrentUser");
            Reserve newRsvp = new Reserve(){
                UserId = (int)getUserId,
                EventId = id
            };
            _context.Add(newRsvp);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        [RouteAttribute("removeRSVP/{id}")]
        public IActionResult RemoveRSVP (int id){
            int? getUserId = HttpContext.Session.GetInt32("CurrentUser");
            Reserve remove = _context.reserves.Where(user => user.UserId == getUserId).Where(activity => activity.EventId == id).SingleOrDefault();
            _context.Remove(remove);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        [RouteAttribute("delete/{id}")]
        public IActionResult Delete(int id){
            List<Reserve> deleteAll = _context.reserves.Where(activity => activity.EventId == id).ToList();
            foreach(var user in deleteAll){
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