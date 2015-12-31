using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebFormGenerator.Models;

namespace WebFormGenerator.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string dataName, string directory, string file, string staticDataFile)
        {
            TempData["Data"] = new LoadData(dataName, directory, file, staticDataFile);
            return RedirectToAction("ParsedForm");
        }

        public ActionResult ParsedForm()
        {
            LoadData data = (LoadData)TempData["Data"];
            return View(data);
        }
    }
}