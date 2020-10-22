using WebServer.infrastructure;
using WebServer.infrastructure.Result;

namespace WebServer.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "Index page";
        }

        public string Details(int id)
        {
            return "Home page" + id;
        }
    }
}