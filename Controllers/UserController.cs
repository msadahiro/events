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
    public class UserController : Controller{
        private MyContext _context;
 
        public UserController(MyContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        // GET: /Register/
        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            ViewBag.errors = new List<string>();
            ViewBag.RegEmailError = "";
            return View();
        }
        [HttpPost]
        [RouteAttribute("create")]
        public IActionResult Create(RegisterViewModel model, User newUser){
            if(ModelState.IsValid){
                User RegCheckEmail = _context.users.Where(User => User.Email == newUser.Email).SingleOrDefault();
                if(RegCheckEmail == null){
                    _context.Add(newUser);
                    _context.SaveChanges();
                    var CurrentUserId = newUser.id;
                    HttpContext.Session.SetInt32("CurrentUser", (int)CurrentUserId);
                    return RedirectToAction ("dashboard", "Event");
                }
                else{
                    ViewBag.RegEmailError = "Email already used";
                }
            }
            else{
                ViewBag.RegEmailError = "";
            }
            ViewBag.errors = ModelState.Values;
            return View("Register");
        }
        [HttpGet]
        [RouteAttribute("login")]
        public IActionResult Login(){
            ViewBag.LogError = "";
            return View();
        }
        [HttpPost]
        [RouteAttribute("login")]
        public IActionResult LoginUser(string Email, string Password, LoginViewModel model){
            if(ModelState.IsValid){
                User SignInUser = _context.users.Where(User => User.Email == Email).SingleOrDefault();
                if(SignInUser != null && Password != null){
                    if(SignInUser.Password == Password){
                        HttpContext.Session.SetInt32("CurrentUser",(int)SignInUser.id);
                        return RedirectToAction("dashboard", "Event");
                    }
                }
            }
            ViewBag.LogError = "Invalid Login";
            return View("Login");
        }
        [HttpGet]
        [RouteAttribute("logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction ("Login");
        }
    }
}
