using Microsoft.AspNetCore.Mvc;

namespace SecretSantaApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Secret Santa App";
            return View();
        }
    }
}
