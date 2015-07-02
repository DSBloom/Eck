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

        public ActionResult MLBToday()
        {
            XMLStats.EventsRequest er = new XMLStats.EventsRequest("a36e424b-8c00-48c1-8b2d-d112f8ce61b0", "nadcraker@gmail.com");
            XMLStats.EventList evtList = er.get("mlb", DateTime.Today);

            XMLStats.MLBBoxScoreRequest mbsr = new XMLStats.MLBBoxScoreRequest("a36e424b-8c00-48c1-8b2d-d112f8ce61b0", "nadcraker@gmail.com");
            foreach(XMLStats.Event evt in evtList)



            return View(list);
        }
    }
}