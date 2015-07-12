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

        public static List<SessionDay> GetSessionDays(Guid sessionID)
        {
            List<SessionDay> sessionDays = new List<SessionDay>();

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select * SessionDays where SessionID=@SessionID order by SessionDayDate";
            cmd.Parameters.AddWithValue("SessionID", sessionID);

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
                    SessionDay sd = new SessionDay();

                    sd.SessionDayID = (Guid)reader["SessionStatus"];
                    sd.SessionID = (Guid)reader["SessionID"];                    
                    sd.SessionDayDate = (DateTime)reader["SessionDayDate"];
                    //sd.GamesThisSession figure this out here or in the calling code?

                    sd.arizonaDiamondbacksDigit = (int)reader["arizonaDiamondbacksDigit"];
                    sd.atlantaBravesDigit = (int)reader["atlantaBravesDigit"];
                    sd.baltimoreOriolesDigit = (int)reader["baltimoreOriolesDigit"];
                    sd.bostonRedSoxDigit = (int)reader["bostonRedSoxDigit"];
                    sd.chicagoCubsDigit = (int)reader["chicagoCubsDigit"];
                    sd.chicagoWhiteSoxDigit = (int)reader["chicagoWhiteSoxDigit"];
                    sd.cincinnatiRedsDigit = (int)reader["cincinnatiRedsDigit"];
                    sd.clevelandIndiansDigit = (int)reader["clevelandIndiansDigit"];
                    sd.coloradoRockiesDigit = (int)reader["coloradoRockiesDigit"];
                    sd.detroitTigersDigit = (int)reader["detroitTigersDigit"];
                    sd.houstonAstrosDigit = (int)reader["houstonAstrosDigit"];
                    sd.kansasCityRoyalsDigit = (int)reader["kansasCityRoyalsDigit"];
                    sd.losAngelesAngelsDigit = (int)reader["losAngelesAngelsDigit"];
                    sd.losAngelesDodgersDigit = (int)reader["losAngelesDodgersDigit"];
                    sd.miamiMarlinsDigit = (int)reader["miamiMarlinsDigit"];
                    sd.milwaukeeBrewersDigit = (int)reader["milwaukeeBrewersDigit"];
                    sd.minnesotaTwinsDigit = (int)reader["minnesotaTwinsDigit"];
                    sd.newYorkMetsDigit = (int)reader["newYorkMetsDigit"];
                    sd.newYorkYankeesDigit = (int)reader["newYorkYankeesDigit"];
                    sd.oaklandAthleticsDigit = (int)reader["oaklandAthleticsDigit"];
                    sd.philadelphiaPhilliesDigit = (int)reader["philadelphiaPhilliesDigit"];
                    sd.pittsburghPiratesDigit = (int)reader["pittsburghPiratesDigit"];
                    sd.sanDiegoPadresDigit = (int)reader["sanDiegoPadresDigit"];
                    sd.sanFranciscoGiantsDigit = (int)reader["sanFranciscoGiantsDigit"];
                    sd.seattleMarinersDigit = (int)reader["seattleMarinersDigit"];
                    sd.stLouisCardinalsDigit = (int)reader["stLouisCardinalsDigit"];
                    sd.tampaBayRaysDigit = (int)reader["tampaBayRaysDigit"];
                    sd.texasRangersDigit = (int)reader["texasRangersDigit"];
                    sd.torontoBlueJaysDigit = (int)reader["torontoBlueJaysDigit"];
                    sd.washingtonNationalsDigit = (int)reader["washingtonNationalsDigit"];

                    sessionDays.Add(sd);
                }
            }
            return sessionDays;
        }

        public static void InsertSessionDay(List<Game> Games)
        {
            SessionDay sd = new SessionDay();
            sd.SessionDayID = Guid.NewGuid();
            //sd.SessionID = Look up somehow. Accept a Session in the parameter list for this method and use that SessionID?
            sd.SessionDayDate = Games[0].GameDate; //Assumes Games has Game objects in it.
            sd.GamesThisSessionDay = Games.Count;

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Insert "; //Long ass insert statement with hella params 'n shit
            //cmd.Parameters.Add //ADD ALL THE STUPID PARAMETERS FOR EVERY TEAM

            foreach (Game g in Games)
            {
                //Build the parameters depending on what scores were had that day for whatever teams
                #region GnarlySwitch
                switch (g.HomeTeam)
                {
                    case "Arizona Diamondbacks":
                        sd.arizonaDiamondbacksDigit = g.HomeLastDigit;
                        break;
                    case "Atlanta Braves":
                        sd.atlantaBravesDigit = g.HomeLastDigit;
                        break;
                    case "Baltimore Orioles":
                        sd.baltimoreOriolesDigit = g.HomeLastDigit;
                        break;
                    case "Boston Red Sox":
                        sd.bostonRedSoxDigit = g.HomeLastDigit;
                        break;
                    case "Chicago Cubs":
                        sd.chicagoCubsDigit = g.HomeLastDigit;
                        break;
                    case "Chicago White Sox":
                        sd.chicagoWhiteSoxDigit = g.HomeLastDigit;
                        break;
                    case "Cincinnati Reds":
                        sd.cincinnatiRedsDigit = g.HomeLastDigit;
                        break;
                    case "Cleveland Indians":
                        sd.clevelandIndiansDigit = g.HomeLastDigit;
                        break;
                    case "Colorado Rockies":
                        sd.coloradoRockiesDigit = g.HomeLastDigit;
                        break;
                    case "Detroit Tigers":
                        sd.detroitTigersDigit = g.HomeLastDigit;
                        break;
                    case "Houston Astros":
                        sd.houstonAstrosDigit = g.HomeLastDigit;
                        break;
                    case "Kansas City Royals":
                        sd.kansasCityRoyalsDigit = g.HomeLastDigit;
                        break;
                    case "Los Angeles Angels":
                        sd.losAngelesAngelsDigit = g.HomeLastDigit;
                        break;
                    case "Los Angeles Dodgers":
                        sd.losAngelesDodgersDigit = g.HomeLastDigit;
                        break;
                    case "Miami Marlins":
                        sd.miamiMarlinsDigit = g.HomeLastDigit;
                        break;
                    case "Milwaukee Brewers":
                        sd.milwaukeeBrewersDigit = g.HomeLastDigit;
                        break;
                    case "Minnesota Twins":
                        sd.minnesotaTwinsDigit = g.HomeLastDigit;
                        break;
                    case "New York Mets":
                        sd.newYorkMetsDigit = g.HomeLastDigit;
                        break;
                    case "New York Yankees":
                        sd.newYorkYankeesDigit = g.HomeLastDigit;
                        break;
                    case "Oakland Athletics":
                        sd.oaklandAthleticsDigit = g.HomeLastDigit;
                        break;
                    case "Philadelphia Phillies":
                        sd.philadelphiaPhilliesDigit = g.HomeLastDigit;
                        break;
                    case "Pittsburgh Pirates":
                        sd.pittsburghPiratesDigit = g.HomeLastDigit;
                        break;
                    case "San Diego Padres":
                        sd.sanDiegoPadresDigit = g.HomeLastDigit;
                        break;
                    case "San Francisco Giants":
                        sd.sanFranciscoGiantsDigit = g.HomeLastDigit;
                        break;
                    case "Seattle Mariners":
                        sd.seattleMarinersDigit = g.HomeLastDigit;
                        break;
                    case "St. Louis Cardinals":
                        sd.stLouisCardinalsDigit = g.HomeLastDigit;
                        break;
                    case "Tampa Bay Rays":
                        sd.tampaBayRaysDigit = g.HomeLastDigit;
                        break;
                    case "Texas Rangers":
                        sd.texasRangersDigit = g.HomeLastDigit;
                        break;
                    case "Toronto Blue Jays":
                        sd.torontoBlueJaysDigit = g.HomeLastDigit;
                        break;
                    case "Washington Nationals":
                        sd.washingtonNationalsDigit = g.HomeLastDigit;
                        break;
                }

                //switch g.AwayTeam
                switch (g.AwayTeam)
                {
                    case "Arizona Diamondbacks":
                        sd.arizonaDiamondbacksDigit = g.AwayLastDigit;
                        break;
                    case "Atlanta Braves":
                        sd.atlantaBravesDigit = g.AwayLastDigit;
                        break;
                    case "Baltimore Orioles":
                        sd.baltimoreOriolesDigit = g.AwayLastDigit;
                        break;
                    case "Boston Red Sox":
                        sd.bostonRedSoxDigit = g.AwayLastDigit;
                        break;
                    case "Chicago Cubs":
                        sd.chicagoCubsDigit = g.AwayLastDigit;
                        break;
                    case "Chicago White Sox":
                        sd.chicagoWhiteSoxDigit = g.AwayLastDigit;
                        break;
                    case "Cincinnati Reds":
                        sd.cincinnatiRedsDigit = g.AwayLastDigit;
                        break;
                    case "Cleveland Indians":
                        sd.clevelandIndiansDigit = g.AwayLastDigit;
                        break;
                    case "Colorado Rockies":
                        sd.coloradoRockiesDigit = g.AwayLastDigit;
                        break;
                    case "Detroit Tigers":
                        sd.detroitTigersDigit = g.AwayLastDigit;
                        break;
                    case "Houston Astros":
                        sd.houstonAstrosDigit = g.AwayLastDigit;
                        break;
                    case "Kansas City Royals":
                        sd.kansasCityRoyalsDigit = g.AwayLastDigit;
                        break;
                    case "Los Angeles Angels":
                        sd.losAngelesAngelsDigit = g.AwayLastDigit;
                        break;
                    case "Los Angeles Dodgers":
                        sd.losAngelesDodgersDigit = g.AwayLastDigit;
                        break;
                    case "Miami Marlins":
                        sd.miamiMarlinsDigit = g.AwayLastDigit;
                        break;
                    case "Milwaukee Brewers":
                        sd.milwaukeeBrewersDigit = g.AwayLastDigit;
                        break;
                    case "Minnesota Twins":
                        sd.minnesotaTwinsDigit = g.AwayLastDigit;
                        break;
                    case "New York Mets":
                        sd.newYorkMetsDigit = g.AwayLastDigit;
                        break;
                    case "New York Yankees":
                        sd.newYorkYankeesDigit = g.AwayLastDigit;
                        break;
                    case "Oakland Athletics":
                        sd.oaklandAthleticsDigit = g.AwayLastDigit;
                        break;
                    case "Philadelphia Phillies":
                        sd.philadelphiaPhilliesDigit = g.AwayLastDigit;
                        break;
                    case "Pittsburgh Pirates":
                        sd.pittsburghPiratesDigit = g.AwayLastDigit;
                        break;
                    case "San Diego Padres":
                        sd.sanDiegoPadresDigit = g.AwayLastDigit;
                        break;
                    case "San Francisco Giants":
                        sd.sanFranciscoGiantsDigit = g.AwayLastDigit;
                        break;
                    case "Seattle Mariners":
                        sd.seattleMarinersDigit = g.AwayLastDigit;
                        break;
                    case "St. Louis Cardinals":
                        sd.stLouisCardinalsDigit = g.AwayLastDigit;
                        break;
                    case "Tampa Bay Rays":
                        sd.tampaBayRaysDigit = g.AwayLastDigit;
                        break;
                    case "Texas Rangers":
                        sd.texasRangersDigit = g.AwayLastDigit;
                        break;
                    case "Toronto Blue Jays":
                        sd.torontoBlueJaysDigit = g.AwayLastDigit;
                        break;
                    case "Washington Nationals":
                        sd.washingtonNationalsDigit = g.AwayLastDigit;
                        break;
                } 
                #endregion
            }
        }

        public static List<Session> GetSessions()
        {
            List<Session> sessions = new List<Session>();

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select * Sessions";

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
                    Session s = new Session();

                    s.SessionID = (Guid)reader["SessionID"];
                    s.SessionStatus = reader["SessionStatus"].ToString();
                    s.CurrentPot = (decimal)reader["CurrentPot"];
                    s.StartDate = (DateTime)reader["StartDate"];
                    s.EndDate = (DateTime)reader["EndDate"];
                    s.WinningTeam = reader["WinningTeam"].ToString();
                    //s.WinningPlayer look this up from Teams table or something?

                    s.arizonaDiamondbacksDigitsNeeded = ParseScores(reader["arizonaDiamondbacksDigitsNeeded"].ToString());
                    s.atlantaBravesDigitsNeeded = ParseScores(reader["atlantaBravesDigitsNeeded"].ToString());
                    s.baltimoreOriolesDigitsNeeded = ParseScores(reader["baltimoreOriolesDigitsNeeded"].ToString());
                    s.bostonRedsoxDigitsNeeded = ParseScores(reader["bostonRedsoxDigitsNeeded"].ToString());
                    s.chicagoCubsDigitsNeeded = ParseScores(reader["chicagoCubsDigitsNeeded"].ToString());
                    s.chicagoWhiteSoxDigitsNeeded = ParseScores(reader["chicagoWhiteSoxDigitsNeeded"].ToString());
                    s.cincinnatiRedsDigitsNeeded = ParseScores(reader["cincinnatiRedsDigitsNeeded"].ToString());
                    s.clevelandIndiansDigitsNeeded = ParseScores(reader["clevelandIndiansDigitsNeeded"].ToString());
                    s.coloradoRockiesDigitsNeeded = ParseScores(reader["coloradoRockiesDigitsNeeded"].ToString());
                    s.detroitTigersDigitsNeeded = ParseScores(reader["detroitTigersDigitsNeeded"].ToString());
                    s.houstonAstrosDigitsNeeded = ParseScores(reader["houstonAstrosDigitsNeeded"].ToString());
                    s.kansasCityRoyalsDigitsNeeded = ParseScores(reader["kansasCityRoyalsDigitsNeeded"].ToString());
                    s.losAngelesAngelsDigitsNeeded = ParseScores(reader["losAngelesAngelsDigitsNeeded"].ToString());
                    s.losAngelesDodgersDigitsNeeded = ParseScores(reader["losAngelesDodgersDigitsNeeded"].ToString());
                    s.miamiMarlinsDigitsNeeded = ParseScores(reader["miamiMarlinsDigitsNeeded"].ToString());
                    s.milwaukeeBrewersDigitsNeeded = ParseScores(reader["milwaukeeBrewersDigitsNeeded"].ToString());
                    s.minnesotaTwinsDigitsNeeded = ParseScores(reader["minnesotaTwinsDigitsNeeded"].ToString());
                    s.newYorkMetsDigitsNeeded = ParseScores(reader["newYorkMetsDigitsNeeded"].ToString());
                    s.newYorkYankeesDigitsNeeded = ParseScores(reader["newYorkYankeesDigitsNeeded"].ToString());
                    s.oaklandAthleticsDigitsNeeded = ParseScores(reader["oaklandAthleticsDigitsNeeded"].ToString());
                    s.philadelphiaPhilliesDigitsNeeded = ParseScores(reader["philadelphiaPhilliesDigitsNeeded"].ToString());
                    s.pittsburghPiratesDigitsNeeded = ParseScores(reader["pittsburghPiratesDigitsNeeded"].ToString());
                    s.sanDiegoPadresDigitsNeeded = ParseScores(reader["sanDiegoPadresDigitsNeeded"].ToString());
                    s.sanFranciscoGiantsDigitsNeeded = ParseScores(reader["sanFranciscoGiantsDigitsNeeded"].ToString());
                    s.seattleMarinersDigitsNeeded = ParseScores(reader["seattleMarinersDigitsNeeded"].ToString());
                    s.stLouisCardinalsDigitsNeeded = ParseScores(reader["stLouisCardinalsDigitsNeeded"].ToString());
                    s.tampaBayRaysDigitsNeeded = ParseScores(reader["tampaBayRaysDigitsNeeded"].ToString());
                    s.texasRangersDigitsNeeded = ParseScores(reader["texasRangersDigitsNeeded"].ToString());
                    s.torontoBlueJaysDigitsNeeded = ParseScores(reader["torontoBlueJaysDigitsNeeded"].ToString());
                    s.washingtonNationalsDigitsNeeded = ParseScores(reader["washingtonNationalsDigitsNeeded"].ToString());

                    sessions.Add(s);
                }
            }
            return sessions;
        }

        public static List<Game> GetGames(DateTime startingDate)
        {
            List<Game> games = new List<Game>();

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"select * from games where GameDate like '%@date%'";
            cmd.Parameters.AddWithValue("date", startingDate);

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

        internal static int[] ParseScores(string Scores)
        {
            string[] sDigits = Scores.Split(',');
            int[] digits = new int[sDigits.Count()];

            for(int i=0; i<sDigits.Count(); i++)
            {
                digits[i] = Convert.ToInt16(sDigits[i]);
            }

            return digits;
        }
    }
    public class SqlReturnObject
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
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
    public class SessionDay
    {
        public Guid SessionDayID { get; set; }
        public Guid SessionID { get; set; }
        public DateTime SessionDayDate { get; set; }
        public int GamesThisSessionDay { get; internal set; }

        public int arizonaDiamondbacksDigit {get;set;}
        public int atlantaBravesDigit { get; set; }
        public int baltimoreOriolesDigit { get; set; }
        public int bostonRedSoxDigit { get; set; }
        public int chicagoCubsDigit { get; set; }
        public int chicagoWhiteSoxDigit { get; set; }
        public int cincinnatiRedsDigit { get; set; }
        public int clevelandIndiansDigit { get; set; }
        public int coloradoRockiesDigit { get; set; }
        public int detroitTigersDigit { get; set; }
        public int houstonAstrosDigit { get; set; }
        public int kansasCityRoyalsDigit { get; set; }
        public int losAngelesAngelsDigit { get; set; }
        public int losAngelesDodgersDigit { get; set; }
        public int miamiMarlinsDigit { get; set; }
        public int milwaukeeBrewersDigit { get; set; }
        public int minnesotaTwinsDigit { get; set; }
        public int newYorkMetsDigit { get; set; }
        public int newYorkYankeesDigit { get; set; }
        public int oaklandAthleticsDigit { get; set; }
        public int philadelphiaPhilliesDigit { get; set; }
        public int pittsburghPiratesDigit { get; set; }
        public int sanDiegoPadresDigit { get; set; }
        public int sanFranciscoGiantsDigit { get; set; }
        public int seattleMarinersDigit { get; set; }
        public int stLouisCardinalsDigit { get; set; }
        public int tampaBayRaysDigit { get; set; }
        public int texasRangersDigit { get; set; }
        public int torontoBlueJaysDigit { get; set; }
        public int washingtonNationalsDigit { get; set; }
    }
    public class Session
    {
        public Guid SessionID { get; set; }
        public string SessionStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Player WinningPlayer { get; set; }
        public string WinningTeam { get; set; }
        public decimal CurrentPot { get; set; }
        public int[] arizonaDiamondbacksDigitsNeeded { get; set; }
        public int[] atlantaBravesDigitsNeeded { get; set; }
        public int[] baltimoreOriolesDigitsNeeded { get; set; }
        public int[] bostonRedsoxDigitsNeeded { get; set; }
        public int[] chicagoCubsDigitsNeeded { get; set; }
        public int[] chicagoWhiteSoxDigitsNeeded { get; set; }
        public int[] cincinnatiRedsDigitsNeeded { get; set; }
        public int[] clevelandIndiansDigitsNeeded { get; set; }
        public int[] coloradoRockiesDigitsNeeded { get; set; }
        public int[] detroitTigersDigitsNeeded { get; set; }
        public int[] houstonAstrosDigitsNeeded { get; set; }
        public int[] kansasCityRoyalsDigitsNeeded { get; set; }
        public int[] losAngelesAngelsDigitsNeeded { get; set; }
        public int[] losAngelesDodgersDigitsNeeded { get; set; }
        public int[] miamiMarlinsDigitsNeeded { get; set; }
        public int[] milwaukeeBrewersDigitsNeeded { get; set; }
        public int[] minnesotaTwinsDigitsNeeded { get; set; }
        public int[] newYorkMetsDigitsNeeded { get; set; }
        public int[] newYorkYankeesDigitsNeeded { get; set; }
        public int[] oaklandAthleticsDigitsNeeded { get; set; }
        public int[] philadelphiaPhilliesDigitsNeeded { get; set; }
        public int[] pittsburghPiratesDigitsNeeded { get; set; }
        public int[] sanDiegoPadresDigitsNeeded { get; set; }
        public int[] sanFranciscoGiantsDigitsNeeded { get; set; }
        public int[] seattleMarinersDigitsNeeded { get; set; }
        public int[] stLouisCardinalsDigitsNeeded { get; set; }
        public int[] tampaBayRaysDigitsNeeded { get; set; }
        public int[] texasRangersDigitsNeeded { get; set; }
        public int[] torontoBlueJaysDigitsNeeded { get; set; }
        public int[] washingtonNationalsDigitsNeeded { get; set; }
    }
    public class Player
    {
        public int ID { get; set; }
        public int TeamID { get; set; }
        public string PlayerName { get; set; }
        public string PlayerEmail { get; set; }
    }
    public class GameStatus
    {
        public bool IsGameInDb { get; set; }
        public string Status { get; set; }
    }
    public class TeamProgress
    {
        public Guid TeamProgressID { get; set; }
        public string TeamName { get; set; }
        public Guid PlayerID { get; set; }
        public Guid SessionID { get; set; }
        public bool Digit0 { get; set; }
        public bool Digit1 { get; set; }
        public bool Digit2 { get; set; }
        public bool Digit3 { get; set; }
        public bool Digit4 { get; set; }
        public bool Digit5 { get; set; }
        public bool Digit6 { get; set; }
        public bool Digit7 { get; set; }
        public bool Digit8 { get; set; }
        public bool Digit9 { get; set; }
        public DateTime DateLastModified { get; set; }
    }
}

