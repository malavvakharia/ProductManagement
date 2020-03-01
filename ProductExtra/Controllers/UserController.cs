using ProductExtra.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductExtra.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult UserRegistration()
        {
            Registration usermodel = new Registration();
            return View(usermodel);
        }
        [HttpPost]
        public ActionResult UserRegistration(Registration user)//Authenticate the user
        {
            using (UserEntities2 dbmodel = new UserEntities2())
            {

                var z = dbmodel.Registrations.Any(x => x.Email == user.Email);
                var m = dbmodel.Registrations.Any(x => x.Username == user.Username);
                if (m == true || z == true)
                {
                    ViewBag.DuplicateMessage = "true";
                    return View("UserRegistration", user);
                }
                dbmodel.Registrations.Add(user);
                dbmodel.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMessage = "true";
            return View("UserRegistration", new Registration());
        }
        // GET: Login

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Registration user)//Registration for new user
        {
            using (UserEntities2 db = new UserEntities2())
            {
                if (user.Email == null || user.Password == null || (user.Email == null && user.Password == null)) { return View(); }
                else
                {
                    var userd = db.Registrations.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();
                    if (db.Registrations.Where(x => x.Email == user.Email).FirstOrDefault() == null)
                    {
                        ViewBag.EmailErrorMessage = true;
                        return View("Login", user);
                    }
                    else if (userd == null)
                    {
                        ViewBag.LoginErrorMessage = "xyz";
                        return View("Login", user);
                    }
                    else
                    {
                        Session["userid"] = userd.Id;
                        Session["username"] = userd.Username;
                        return RedirectToAction("Index", "User");
                    }
                }
            }
        }     
        public ActionResult Logout()//Destroy the session 
        {
            int id = (int)Session["userid"];
            Session.Abandon();
            return RedirectToAction("Login", "User");
        }
        public ActionResult Index()//Welcome Method
        {
            return View();
        }
    }
}