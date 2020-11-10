using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace WebPortal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var routes = this.RouteData.Routers.OfType<RouteCollection>().FirstOrDefault();
            return View();
        }
    }
}

