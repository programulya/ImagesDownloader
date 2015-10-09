using System.Web.Mvc;

namespace ImagesDownloader.Controllers
{
    /// <summary>
    /// Home page controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Default action for display home page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}
