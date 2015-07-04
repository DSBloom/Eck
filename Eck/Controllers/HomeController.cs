using Eck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XMLStats;

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
            List<TestData> testList = DataAccess.GetTestData();
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
            //XMLStats.EventsRequest er = new XMLStats.EventsRequest("a36e424b-8c00-48c1-8b2d-d112f8ce61b0", "nadcraker@gmail.com");
            string visitingTeam, homeTeam;
            DateTime eventDate;

            MLBBoxScore mlbBox = null;
            List<Models.EckGame> listOfScores = new List<Models.EckGame>();

            EventsRequest er = new EventsRequest("a36e424b-8c00-48c1-8b2d-d112f8ce61b0", "nadcraker@gmail.com");
            EventList eList = er.get("mlb", new DateTime(2015, 7, 3));

            foreach (Event singleEvent in eList.@event)
            {
                EckTeam t = new EckTeam();
                Models.EckGame game = new Models.EckGame();

                if (singleEvent.home_team.full_name.Contains("Athletics") || singleEvent.away_team.full_name.Contains("Athletics"))
                {
                    visitingTeam = singleEvent.away_team.team_id;
                    homeTeam = singleEvent.home_team.team_id;

                    if (DateTime.TryParse(singleEvent.start_date_time, out eventDate))
                    {
                        MLBBoxScoreRequest mlbBoxRequest = new MLBBoxScoreRequest("a36e424b-8c00-48c1-8b2d-d112f8ce61b0", "nadcraker@gmail.com");
                        mlbBox = mlbBoxRequest.get(eventDate, visitingTeam, homeTeam);

                        int homeTeamScore = 0;
                        foreach(int s in mlbBox.home_period_scores)
                        {
                            homeTeamScore = homeTeamScore + s;
                        }

                        int awayTeamScore = 0;
                        foreach (int s in mlbBox.away_period_scores)
                        {
                            awayTeamScore = awayTeamScore + s;
                        }

                        //Make muh object
                        game.AwayScore = awayTeamScore;
                        game.AwayTeam = singleEvent.away_team.full_name;
                        game.HomeScore = homeTeamScore;
                        game.HomeTeam = singleEvent.home_team.full_name;
                        game.GameDate = eventDate;

                        listOfScores.Add(game);
                    }
                }
            }

            return View(listOfScores);
        }
    }
}