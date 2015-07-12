using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetScores;

namespace SessionBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            //Check to see if there are sessions in the db
            List<Session> sessions = DataAccess.GetSessions();

            if(sessions.Count > 0)
            {
                //Get the latest session
                Session latestSess = sessions[sessions.Count];

                if (latestSess.SessionStatus == "In progress")
                {
                    //What is the latest date in that session
                    //Get all the sessionDays associated with that session
                    List<SessionDay> sessDays = DataAccess.GetSessionDays(latestSess.SessionID);

                    //Get the latest one
                    SessionDay latestSessDay = sessDays[sessDays.Count];

                    //If the latest session in the database is before or on today, proceed
                    if (latestSessDay.SessionDayDate <= DateTime.Now)
                    {
                        //Start getting games on that day, make sure we have all the games accounted for that day (no partial records)
                        //and start inserting sessionDays from that day on
                        List<Game> games = DataAccess.GetGames(latestSessDay.SessionDayDate);

                        if (games.Count == latestSessDay.GamesThisSessionDay)
                        {
                            //Ok, this isn't a partial record
                        }
                        else
                        {
                            //Need to update this partial record
                        }

                        //Go through the games and make sessiondays
                        //Also update the session associated with those days
                        //SessionDay sd = new SessionDay();

                        //Insert new SessionDay from the list of Game objects
                        DataAccess.InsertSessionDay(games);

                        //DataAccess.UpdateSession(???)
                    }
                    else
                    {
                        //We're all caught up!
                        Environment.Exit(0);
                    }
                }
                else
                {
                    //No sessions in progress?
                    //So we need to start a new one?
                    //If there are ZERO sessions in the database, then the pool hasn't even started. We're starting from scratch
                    //Get MLB opening day from the config file. Seems like a good place to put something like that
                }
            }

























            ////Global
            //List<Session> sessions = new List<GetScores.Session>();
            //DateTime startingDate = new DateTime(2015, 4, 5).Date;

            ////Check where the latest session is
            //Session latestSession = DataAccess.GetLatestSession();
            
            ////Create the initial Session object
            //Session session = new Session();
            //session.SessionID = Guid.NewGuid();            
            //session.SessionStatus = "In Progress";
            //session.StartDate = startingDate.Date;
            
            //while (startingDate < DateTime.Now.AddDays(1))
            //{
            //    if (session.SessionStatus == "In Progress")
            //    {
            //        //Get all of the games from the database for that day
            //        List<Game> games = DataAccess.GetGames(startingDate);

            //        //New sessionDay
            //        SessionDay sd = new SessionDay();
            //        sd.SessionDayID = Guid.NewGuid();
            //        sd.SessionDate = startingDate;

            //        foreach (Game g in games)
            //        {
            //            //switch g.HomeTeam //put the digit in the right column in the sessionDAy
            //            switch (g.HomeTeam)
            //            {
            //                case "Arizona Diamondbacks":
            //                    sd.arizonaDiamondbacksDigit = g.HomeLastDigit;
            //                    break;
            //                case "Atlanta Braves":
            //                    sd.atlantaBravesDigit = g.HomeLastDigit;
            //                    break;
            //                case "Baltimore Orioles":
            //                    sd.baltimoreoriolesDigit = g.HomeLastDigit;
            //                    break;
            //                case "Boston Red Sox":
            //                    sd.bostonRedSoxsDigit = g.HomeLastDigit;
            //                    break;
            //                case "Chicago Cubs":
            //                    sd.chicagoCubsDigit = g.HomeLastDigit;
            //                    break;
            //                case "Chicago White Sox":
            //                    sd.chicagoWhiteSoxsDigit = g.HomeLastDigit;
            //                    break;
            //                case "Cincinnati Reds":
            //                    sd.cincinnatiRedsDigit = g.HomeLastDigit;
            //                    break;
            //                case "Cleveland Indians":
            //                    sd.clevelandIndiansDigit = g.HomeLastDigit;
            //                    break;
            //                case "Colorado Rockies":
            //                    sd.coloradoRockiesDigit = g.HomeLastDigit;
            //                    break;
            //                case "Detroit Tigers":
            //                    sd.detroitTigersDigit = g.HomeLastDigit;
            //                    break;
            //                case "Houston Astros":
            //                    sd.houstonAstrosDigit = g.HomeLastDigit;
            //                    break;
            //                case "Kansas City Royals":
            //                    sd.kansasCityRoyalssDigit = g.HomeLastDigit;
            //                    break;
            //                case "Los Angeles Angels":
            //                    sd.losAngelesAngelsDigit = g.HomeLastDigit;
            //                    break;
            //                case "Los Angeles Dodgers":
            //                    sd.losAngelesDodgersDigit = g.HomeLastDigit;
            //                    break;
            //                case "Miami Marlins":
            //                    sd.miamiMarlinsDigit = g.HomeLastDigit;
            //                    break;
            //                case "Milwaukee Brewers":
            //                    sd.milwaukeeBrewersDigit = g.HomeLastDigit;
            //                    break;
            //                case "Minnesota Twins":
            //                    sd.minnesotaTwinsDigit = g.HomeLastDigit;
            //                    break;
            //                case "New York Mets":
            //                    sd.newYorkMetsDigit = g.HomeLastDigit;
            //                    break;
            //                case "New York Yankees":
            //                    sd.newYorkYankeesDigit = g.HomeLastDigit;
            //                    break;
            //                case "Oakland Athletics":
            //                    sd.oaklandAthleticsDigit = g.HomeLastDigit;
            //                    break;
            //                case "Philadelphia Phillies":
            //                    sd.philadelphiaPhilliesDigit = g.HomeLastDigit;
            //                    break;
            //                case "Pittsburgh Pirates":
            //                    sd.pittsburghPiratesDigit = g.HomeLastDigit;
            //                    break;
            //                case "San Diego Padres":
            //                    sd.sanDiegoPadresDigit = g.HomeLastDigit;
            //                    break;
            //                case "San Francisco Giants":
            //                    sd.sanFranciscoGiantsDigit = g.HomeLastDigit;
            //                    break;
            //                case "Seattle Mariners":
            //                    sd.seattleMarinersDigit = g.HomeLastDigit;
            //                    break;
            //                case "St. Louis Cardinals":
            //                    sd.stLouisCardinalsDigit = g.HomeLastDigit;
            //                    break;
            //                case "Tampa Bay Rays":
            //                    sd.tampaBayRaysDigit = g.HomeLastDigit;
            //                    break;
            //                case "Texas Rangers":
            //                    sd.texasRangersDigit = g.HomeLastDigit;
            //                    break;
            //                case "Toronto Blue Jays":
            //                    sd.torontoBlueJaysDigit = g.HomeLastDigit;
            //                    break;
            //                case "Washington Nationals":
            //                    sd.washingtonNationalsDigit = g.HomeLastDigit;
            //                    break;
            //            }

            //            //switch g.AwayTeam
            //            switch (g.AwayTeam)
            //            {
            //                case "Arizona Diamondbacks":
            //                    sd.arizonaDiamondbacksDigit = g.AwayLastDigit;
            //                    break;
            //                case "Atlanta Braves":
            //                    sd.atlantaBravesDigit = g.AwayLastDigit;
            //                    break;
            //                case "Baltimore Orioles":
            //                    sd.baltimoreoriolesDigit = g.AwayLastDigit;
            //                    break;
            //                case "Boston Red Sox":
            //                    sd.bostonRedSoxsDigit = g.AwayLastDigit;
            //                    break;
            //                case "Chicago Cubs":
            //                    sd.chicagoCubsDigit = g.AwayLastDigit;
            //                    break;
            //                case "Chicago White Sox":
            //                    sd.chicagoWhiteSoxsDigit = g.AwayLastDigit;
            //                    break;
            //                case "Cincinnati Reds":
            //                    sd.cincinnatiRedsDigit = g.AwayLastDigit;
            //                    break;
            //                case "Cleveland Indians":
            //                    sd.clevelandIndiansDigit = g.AwayLastDigit;
            //                    break;
            //                case "Colorado Rockies":
            //                    sd.coloradoRockiesDigit = g.AwayLastDigit;
            //                    break;
            //                case "Detroit Tigers":
            //                    sd.detroitTigersDigit = g.AwayLastDigit;
            //                    break;
            //                case "Houston Astros":
            //                    sd.houstonAstrosDigit = g.AwayLastDigit;
            //                    break;
            //                case "Kansas City Royals":
            //                    sd.kansasCityRoyalssDigit = g.AwayLastDigit;
            //                    break;
            //                case "Los Angeles Angels":
            //                    sd.losAngelesAngelsDigit = g.AwayLastDigit;
            //                    break;
            //                case "Los Angeles Dodgers":
            //                    sd.losAngelesDodgersDigit = g.AwayLastDigit;
            //                    break;
            //                case "Miami Marlins":
            //                    sd.miamiMarlinsDigit = g.AwayLastDigit;
            //                    break;
            //                case "Milwaukee Brewers":
            //                    sd.milwaukeeBrewersDigit = g.AwayLastDigit;
            //                    break;
            //                case "Minnesota Twins":
            //                    sd.minnesotaTwinsDigit = g.AwayLastDigit;
            //                    break;
            //                case "New York Mets":
            //                    sd.newYorkMetsDigit = g.AwayLastDigit;
            //                    break;
            //                case "New York Yankees":
            //                    sd.newYorkYankeesDigit = g.AwayLastDigit;
            //                    break;
            //                case "Oakland Athletics":
            //                    sd.oaklandAthleticsDigit = g.AwayLastDigit;
            //                    break;
            //                case "Philadelphia Phillies":
            //                    sd.philadelphiaPhilliesDigit = g.AwayLastDigit;
            //                    break;
            //                case "Pittsburgh Pirates":
            //                    sd.pittsburghPiratesDigit = g.AwayLastDigit;
            //                    break;
            //                case "San Diego Padres":
            //                    sd.sanDiegoPadresDigit = g.AwayLastDigit;
            //                    break;
            //                case "San Francisco Giants":
            //                    sd.sanFranciscoGiantsDigit = g.AwayLastDigit;
            //                    break;
            //                case "Seattle Mariners":
            //                    sd.seattleMarinersDigit = g.AwayLastDigit;
            //                    break;
            //                case "St. Louis Cardinals":
            //                    sd.stLouisCardinalsDigit = g.AwayLastDigit;
            //                    break;
            //                case "Tampa Bay Rays":
            //                    sd.tampaBayRaysDigit = g.AwayLastDigit;
            //                    break;
            //                case "Texas Rangers":
            //                    sd.texasRangersDigit = g.AwayLastDigit;
            //                    break;
            //                case "Toronto Blue Jays":
            //                    sd.torontoBlueJaysDigit = g.AwayLastDigit;
            //                    break;
            //                case "Washington Nationals":
            //                    sd.washingtonNationalsDigit = g.AwayLastDigit;
            //                    break;
            //            }
            //        }
            //    }
            //}






            //Also update the session every day until 
        }
    }
}
