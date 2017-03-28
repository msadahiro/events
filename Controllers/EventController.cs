using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using events.Models;
using Microsoft.AspNetCore.Mvc;
using events.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

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
            ViewBag.CurrentUser = SignedInUser;
            return View();
        }
        [HttpGetAttribute]
        [RouteAttribute("new")]
        public IActionResult New(){
            return View();
        }
        [HttpPost]
        [RouteAttribute("makeactivity")]
        public IActionResult MakeActivity(){
            
        }
    }
}