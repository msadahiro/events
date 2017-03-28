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
            User SignedInUser = _context.Users.Where(User => User.id == getUserId).SingleOrDefault();
            List<Event> getAllEvents = _context.Events
            .Include(reserve => reserve.Attendings)
            .ToList();
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
            if(ModelState.IsValid){
                newEvent.CreatedBy = (int)HttpContext.Session.GetInt32("CurrentUser");
                _context.Add(newEvent);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else{
                ViewBag.errors = ModelState.Values;
                return View("New");
            }
        }
    }
}