using Microsoft.AspNetCore.Mvc;

namespace FUNSAR.Areas.Identity
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
