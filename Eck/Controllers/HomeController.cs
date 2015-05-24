using Eck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eck.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Create()
        {
            ViewBag.Message = "Test insertion page.";

            return View();
        }

        public ActionResult List()
        {
            List < TestData > testList = DataAccess.GetTestData();
            ViewBag.Message = "Showing database records.";

            return View(testList);
        }

        [HttpPost]
        public ActionResult Create(TestData model)
        {
            if (ModelState.IsValid)
            {
                DataAccess.InsertTestData(model);
                return RedirectToAction("List");
            }

            return View(model);
        }
    }
}