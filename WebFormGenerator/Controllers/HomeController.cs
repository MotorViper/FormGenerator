using System.Web.Mvc;
using WebFormGenerator.Models;

namespace WebFormGenerator.Controllers
{
    /// <summary>
    /// Controller for the Home page.
    /// </summary>
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string dataName, string directory, string file, string staticDataFile, string optionsFile)
        {
            TempData["Data"] = new LoadData(dataName, directory, file, staticDataFile, optionsFile);
            return RedirectToAction("ParsedForm");
        }

        public ActionResult ParsedForm()
        {
            LoadData data = (LoadData)TempData["Data"];
            return View(data);
        }
    }
}
