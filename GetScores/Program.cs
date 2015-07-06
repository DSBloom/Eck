using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLStats;
using XMLStats.Entities;
using XMLStats.Helpers;

namespace GetScores
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set the date we should start looking for MLB games
            DateTime eventDate = new DateTime(2015, 4, 5);
            List<Eck.Models.EckGame> listOfScores = new List<Eck.Models.EckGame>();

            while (eventDate < DateTime.Now.AddDays(1))
            {
                //Get all the games for each day
                string visitingTeam, homeTeam;

                MLBBoxScore mlbBox = null;

                EventsRequest er = new EventsRequest("a36e424b-8c00-48c1-8b2d-d112f8ce61b0", "nadcraker@gmail.com");
                EventList eList = er.get("mlb", eventDate);

                foreach (Event singleEvent in eList.@event)
                {
                    Eck.Models.EckGame game = new Eck.Models.EckGame();

                    visitingTeam = singleEvent.away_team.team_id;
                    homeTeam = singleEvent.home_team.team_id;

                    if (DateTime.TryParse(singleEvent.start_date_time, out eventDate))
                    {
                        MLBBoxScoreRequest mlbBoxRequest = new MLBBoxScoreRequest("a36e424b-8c00-48c1-8b2d-d112f8ce61b0", "nadcraker@gmail.com");
                        mlbBox = mlbBoxRequest.get(eventDate, visitingTeam, homeTeam);

                        int homeTeamScore = 0;
                        foreach (int s in mlbBox.home_period_scores)
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
                        game.AwayLastDigit = awayTeamScore % 10;
                        game.HomeScore = homeTeamScore;
                        game.HomeLastDigit = homeTeamScore % 10;
                        game.HomeTeam = singleEvent.home_team.full_name;
                        game.GameDate = eventDate;

                        listOfScores.Add(game);
                    }
                }
                //Delay the thread so we don't exceed 6 requests per minute
                System.Threading.Thread.Sleep(15000);
                eventDate.AddDays(1);
            }

            //Insert into the database
            foreach (Eck.Models.EckGame g in listOfScores)
            {
                SqlConnection conn = new System.Data.SqlClient.SqlConnection();
                conn.ConnectionString = "Server=tcp:ye8viai1bq.database.windows.net,1433;Database=EckDB;User ID=nadcraker@ye8viai1bq;Password=Gh7oBttlYlmX5ykJ;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO Games (GameID, GameDate, HomeTeam, AwayTeam, HomeTeamScore, HomeTeamLastDigit, AwayTeamScore, AwayTeamLastDigit) VALUES (@GameID, @GameDate, @HomeTeam, @AwayTeam, @HomeTeamScore, @HomeTeamLastDigit, @AwayTeamScore, @AwayTeamLastDigit)";
                cmd.Parameters.AddWithValue("@GameID", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@GameDate", g.GameDate);
                cmd.Parameters.AddWithValue("@HomeTeam", g.HomeTeam);
                cmd.Parameters.AddWithValue("@AwayTeam", g.AwayTeam);
                cmd.Parameters.AddWithValue("@HomeTeamScore", g.HomeScore);
                cmd.Parameters.AddWithValue("@HomeTeamLastDigit", g.HomeLastDigit);
                cmd.Parameters.AddWithValue("@AwayTeamScore", g.AwayScore);
                cmd.Parameters.AddWithValue("@AwayTeamLastDigit", g.AwayLastDigit);

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }

                cmd.ExecuteNonQuery();
            }
        }
    }
}