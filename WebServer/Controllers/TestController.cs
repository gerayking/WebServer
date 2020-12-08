using WebServer.infrastructure;
using WebServer.infrastructure.Result;

namespace WebServer.Controllers
{
    public class TestController : Controller
    {
        public ActionResult Test(string firstName ,string lastName="123")
        {
            var model = new {
                    firstname = firstName,
                    lastname = lastName,
                };
            return new ViewResult("test", "test", model);
        }
    }
}