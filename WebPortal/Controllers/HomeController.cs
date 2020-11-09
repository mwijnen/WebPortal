using Glimpse.AspNet.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
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

