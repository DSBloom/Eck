using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetScores;
using System.Data.SqlTypes;

namespace SessionBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            //Start Darte
            DateTime startDate = new DateTime(2015, 4, 26).Date; //Get this from config file later

            while (startDate < DateTime.Now.AddDays(1))
            {
                //Check to see if there are sessions in the db
                List<Session> sessions = DataAccess.GetSessions();

                if (sessions.Count > 0)
                {
                    //Get the latest session
                    Session latestSess = sessions.Last();

                    if (latestSess.SessionStatus.ToUpper() == "In Progress".ToUpper())
                    {
                        //What is the latest date in that session
                        //Get all the sessionDays associated with that session
                        List<SessionDay> sessDays = DataAccess.GetSessionDays(latestSess.SessionID);

                        if (sessDays.Count > 0)
                        {
                            //Get the latest one
                            SessionDay latestSessDay = sessDays.Last();

                            //Now we now which date to start on
                            startDate = latestSessDay.SessionDayDate.Date;

                            while (startDate <= DateTime.Now)
                            {
                                sessDays = DataAccess.GetSessionDays(latestSess.SessionID);
                                latestSessDay = sessDays.Last();

                                //If the latest session in the database is before or on today, proceed
                                if (latestSessDay.SessionDayDate <= DateTime.Now)
                                {
                                    //Start getting games on that day, make sure we have all the games accounted for that day (no partial records)
                                    //and start inserting sessionDays from that day on
                                    List<Game> games = DataAccess.GetGames(latestSessDay.SessionDayDate);

                                    if (games.Count == latestSessDay.GamesThisSessionDay)
                                    {
                                        //Ok, this isn't a partial record. So add a day and get the games/sessionDay info for that
                                        DateTime newDate = latestSessDay.SessionDayDate.Date.AddDays(1);
                                        List<Game> newGames = DataAccess.GetGames(newDate);
                                        SessionDay sd = new SessionDay(newGames, latestSess);

                                        //Insert new SessionDay from the list of Game objects
                                        if (DataAccess.InsertSessionDay(sd, latestSess))
                                        {
                                            //Insert succeeded
                                        }
                                        else
                                        {
                                            //It failed :(
                                        }

                                        DataAccess.UpdateSession(latestSess, sd);

                                        //Are there any winners?
                                        List<string> winningTeams = DataAccess.GetWinners(latestSess); //THIS IS NOT WORKING :(
                                        if (winningTeams.Count > 0)
                                        {
                                            //There are winners!
                                            DataAccess.UpdateSessionWithWinners(winningTeams, latestSess, latestSessDay.SessionDayDate);
                                        }
                                    }
                                    else
                                    {
                                        //Need to update this partial record
                                        //Use the UniqueID from latestSess to update the record
                                        DataAccess.UpdateSessionDay(latestSessDay);
                                    }
                                }
                                else
                                {
                                    //We're all caught up!
                                    Environment.Exit(0);
                                }

                                //Increment the day
                                startDate = startDate.Date.AddDays(1).Date;
                            }
                        }
                        else
                        {
                            //There are no sessionDays associated with this session
                            //Insert one
                            List<Game> games = DataAccess.GetGames(latestSess.StartDate);

                            //Ok, this isn't a partial record
                            //Go through the games and make sessiondays
                            //Also update the session associated with those days
                            if (games.Count > 0)
                            {
                                SessionDay sd = new SessionDay(games, latestSess);


                                //Insert new SessionDay from the list of Game objects
                                if (DataAccess.InsertSessionDay(sd, latestSess))
                                {
                                    //Insert succeeded
                                }
                                else
                                {
                                    //It failed :(
                                }
                                DataAccess.UpdateSession(latestSess, sd);
                            }
                        }
                    }
                    else
                    {
                        //The latest session was not In Progress
                        //So we need to start a new one
                        //Check if we are at the end of the season first
                        DateTime seasonEnd = new DateTime(2015, 10, 4);
                        List<SessionDay> sdList = DataAccess.GetSessionDays(latestSess.SessionID);
                        if (sdList.Last().SessionDayDate == seasonEnd)
                        {
                            //Season is over!
                            Environment.Exit(0);
                        }
                        else
                        {
                            //We need to start a new session I guess
                            //Get the date the last one ended and start a new one the day after that
                        }
                    }
                }
                else
                {
                    //No sessions in progress?
                    //So we need to start a new one?
                    //If there are ZERO sessions in the database, then the pool hasn't even started. We're starting from scratch

                    Session firstSession = new Session();
                    firstSession.SessionID = Guid.NewGuid();
                    firstSession.SessionStatus = "In Progress";
                    firstSession.CurrentPot = 25; //Get the initial pot starting amount from the config file
                    firstSession.StartDate = startDate; //Use the opening day from the config file
                    firstSession.EndDate = (DateTime)SqlDateTime.MinValue;
                    firstSession.WinningPlayer = DBNull.Value.ToString();
                    firstSession.WinningTeam = DBNull.Value.ToString();

                    //Set all of the team digitsNeeded columns to the initial value
                    foreach (var prop in firstSession.GetType().GetProperties())
                    {
                        if (prop.Name.Contains("DigitsNeeded"))
                        {
                            prop.SetValue(firstSession, @"0,1,2,3,4,5,6,7,8,9", null);
                        }
                    }
                    DataAccess.InsertSessionDay(firstSession);

                    startDate.AddDays(1);

                    //Now we need to start getting SessionDays and associating them with this first session
                    //Date = firstSession.StartDate;
                    //while(Date < ????)
                    //List<SessionDays> sdList = GetSessionDays(date);
                    //foreach(SessionDay sd in sdList)
                    //InsertSessionDay(sd);
                    //Check if the session is now closed, if there is a winner, etc...
                }
            }
        }
    }
}