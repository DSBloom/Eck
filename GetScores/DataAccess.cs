using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using XMLStats;

namespace GetScores
{
    public class DataAccess
    {
        public static Boolean IsEventInDb(XMLStats.Event SingleEvent)
        {
            XMLStats.Event _event = SingleEvent;

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select * from Games where gameID=@gameID";
            cmd.Parameters.AddWithValue("gameID", _event.event_id);

            //Open the connection if need be
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            //Check if the event is in the database
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Game> GetGames()
        {
            List<Game> games = new List<Game>();

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select * from Games order by GameDate";

            //Open the connection if need be
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            //Check if the event is in the database
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Game g = new Game();

                    g.GameID = reader["GameID"].ToString();
                    g.HomeScore = Convert.ToInt16(reader["HomeScore"]);
                    g.HomeLastDigit = Convert.ToInt16(reader["HomeLastDigit"]);
                    g.AwayScore = Convert.ToInt16(reader["AwayScore"]);
                    g.AwayLastDigit = Convert.ToInt16(reader["AwayLastDigit"]);
                    g.HomeTeam = reader["HomeTeam"].ToString();
                    g.AwayTeam = reader["AwayTeam"].ToString();
                    g.GameDate = DateTime.Parse(reader["GameDate"].ToString());

                    games.Add(g);
                }
            }

            return games;
        }

        public static SqlReturnObject InsertEvent(XMLStats.Event SingleEvent)
        {
            XMLStats.Event _event = SingleEvent;
            MLBBoxScore mlbBox = null;
            SqlReturnObject sro = new SqlReturnObject();

            //Get the MLB box score for this event if it is completed
            int homeTeamScore = 0;
            int awayTeamScore = 0;
            if (_event.event_status == "completed")
            {
                MLBBoxScoreRequest mlbBoxRequest = new MLBBoxScoreRequest("a36e424b-8c00-48c1-8b2d-d112f8ce61b0", "nadcraker@gmail.com");
                mlbBox = mlbBoxRequest.get(_event.event_id);

                //Calculate the scores for the game
                foreach (int s in mlbBox.home_period_scores)
                {
                    homeTeamScore = homeTeamScore + s;
                }

                foreach (int s in mlbBox.away_period_scores)
                {
                    awayTeamScore = awayTeamScore + s;
                }
            }

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO Games (GameID, GameDate, GameStatus, HomeTeam, AwayTeam, HomeTeamScore, HomeTeamLastDigit, AwayTeamScore, AwayTeamLastDigit) VALUES (@GameID, @GameDate, @GameStatus, @HomeTeam, @AwayTeam, @HomeTeamScore, @HomeTeamLastDigit, @AwayTeamScore, @AwayTeamLastDigit)";
            cmd.Parameters.AddWithValue("@GameID", _event.event_id);
            cmd.Parameters.AddWithValue("@GameDate", _event.start_date_time);
            cmd.Parameters.AddWithValue("@GameStatus", _event.event_status);
            cmd.Parameters.AddWithValue("@HomeTeam", _event.home_team.full_name);
            cmd.Parameters.AddWithValue("@AwayTeam", _event.away_team.full_name);
            cmd.Parameters.AddWithValue("@HomeTeamScore", homeTeamScore);
            cmd.Parameters.AddWithValue("@HomeTeamLastDigit", homeTeamScore % 10);
            cmd.Parameters.AddWithValue("@AwayTeamScore", awayTeamScore);
            cmd.Parameters.AddWithValue("@AwayTeamLastDigit", awayTeamScore % 10);

            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            try
            {
                cmd.ExecuteNonQuery();

                sro.Success = true;
                sro.ErrorMessage = null;
                return sro;
            }
            catch (Exception ex)
            {
                sro.Success = false;
                sro.ErrorMessage = ex.Message;
                return sro;
            }
        }

        internal static GameStatus GameStatus(Event SingleEvent)
        {
            Event _event = SingleEvent;
            GameStatus gs = new GameStatus();

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select * from Games where gameID=@gameID";
            cmd.Parameters.AddWithValue("gameID", _event.event_id);

            //Open the connection if need be
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            //Check if the event is in the database
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    gs.IsGameInDb = true;
                    gs.Status = reader["GameStatus"].ToString();
                }

                return gs;
            }
            else
            {
                gs.IsGameInDb = false;
                gs.Status = null;

                return gs;
            }
        }

        internal static SqlReturnObject UpdateEvent(Event SingleEvent)
        {
            XMLStats.Event _event = SingleEvent;
            MLBBoxScore mlbBox = null;
            SqlReturnObject sro = new SqlReturnObject();

            //Get the MLB box score for this event if it is completed
            int homeTeamScore = 0;
            int awayTeamScore = 0;
            if (_event.event_status == "completed")
            {
                MLBBoxScoreRequest mlbBoxRequest = new MLBBoxScoreRequest("a36e424b-8c00-48c1-8b2d-d112f8ce61b0", "nadcraker@gmail.com");
                mlbBox = mlbBoxRequest.get(_event.event_id);

                //Calculate the scores for the game
                foreach (int s in mlbBox.home_period_scores)
                {
                    homeTeamScore = homeTeamScore + s;
                }

                foreach (int s in mlbBox.away_period_scores)
                {
                    awayTeamScore = awayTeamScore + s;
                }
            }

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Update Games Set GameID=@GameID, GameDate=@GameDate, GameStatus=@GameStatus, HomeTeam=@HomeTeam, AwayTeam=@AwayTeam, HomeTeamScore=@HomeTeamScore, HomeTeamLastDigit=@HomeTeamLastDigit, AwayTeamScore=@AwayTeamScore, AwayTeamLastDigit=@AwayTeamLastDigit WHERE GameID=@GameID";
            cmd.Parameters.AddWithValue("@GameID", _event.event_id);
            cmd.Parameters.AddWithValue("@GameDate", _event.start_date_time);
            cmd.Parameters.AddWithValue("@GameStatus", _event.event_status);
            cmd.Parameters.AddWithValue("@HomeTeam", _event.home_team.full_name);
            cmd.Parameters.AddWithValue("@AwayTeam", _event.away_team.full_name);
            cmd.Parameters.AddWithValue("@HomeTeamScore", homeTeamScore);
            cmd.Parameters.AddWithValue("@HomeTeamLastDigit", homeTeamScore % 10);
            cmd.Parameters.AddWithValue("@AwayTeamScore", awayTeamScore);
            cmd.Parameters.AddWithValue("@AwayTeamLastDigit", awayTeamScore % 10);

            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            try
            {
                cmd.ExecuteNonQuery();

                sro.Success = true;
                sro.ErrorMessage = null;
                return sro;
            }
            catch (Exception ex)
            {
                sro.Success = false;
                sro.ErrorMessage = ex.Message;
                return sro;
            }
        }
    }

    public class SqlReturnObject
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class GameStatus
    {
        public bool IsGameInDb { get; set; }
        public string Status { get; set; }
    }

    public class Session
    {
        public Guid SessionID { get; set; }
        public string SessionState { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Player WinningPlayer { get; set; }
        public string WinningTeam { get; set; }
        public decimal PotValue { get; set; }
    }

    public class TeamProgress
    {

    }

    public class Game
    {
        public string GameID { get; set; }
        public int HomeScore { get; set; }
        public int HomeLastDigit { get; set; }
        public int AwayScore { get; set; }
        public int AwayLastDigit { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime GameDate { get; set; }
    }

    public class Player
    {
        public int ID { get; set; }
        public int TeamID { get; set; }
        public string PlayerName { get; set; }
        public string PlayerEmail { get; set; }
    }
}