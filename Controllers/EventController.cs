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
            return View();
        }
    }
}