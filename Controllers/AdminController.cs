using Microsoft.AspNetCore.Mvc;

namespace EchoBot.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        // GET /admin
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();    // Va a buscar Views/Admin/Index.cshtml
        }
    }
}
