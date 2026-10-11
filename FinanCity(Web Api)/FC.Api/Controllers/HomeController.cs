namespace FC.Api.Controllers
{
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }
    }
}