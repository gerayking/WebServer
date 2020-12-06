using System;
using System.Diagnostics;
using WebServer.Entry;
using WebServer.infrastructure;
using WebServer.infrastructure.Result;

namespace WebServer.Controllers
{
    public class HomeController : Controller
    {
        private static Session Session = new Session();
        

        public ActionResult Index()
        {
            int counter = (Session["counter"] != null) ? (int)Session["counter"] : 0;
            counter++;
            Session["counter"] = counter;
            var model = new {title = "Homepage", counter = counter};
            return new ViewResult("Index", "index",model);
        }
        public string Details(int id = 15)
        {
            return "Home page" + id;
        }
    }
}