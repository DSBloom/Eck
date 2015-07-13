using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using XMLStats;
using System.Data;

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
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
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
            cmd.CommandText = "Select * From SessionDays where SessionID=@SessionID order by SessionDayDate";
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

                    sd.SessionDayID = (Guid)reader["SessionDayID"];
                    sd.SessionID = (Guid)reader["SessionID"];                    
                    sd.SessionDayDate = (DateTime)reader["SessionDayDate"];
                    sd.GamesThisSessionDay = (int)reader["GamesThisSessionDate"];

                    sd.arizonaDiamondbacksDigit = reader["arizonaDiamondbacksDigit"].ToString();
                    sd.atlantaBravesDigit = reader["atlantaBravesDigit"].ToString();
                    sd.baltimoreOriolesDigit = reader["baltimoreOriolesDigit"].ToString();
                    sd.bostonRedSoxDigit = reader["bostonRedSoxDigit"].ToString();
                    sd.chicagoCubsDigit = reader["chicagoCubsDigit"].ToString();
                    sd.chicagoWhiteSoxDigit = reader["chicagoWhiteSoxDigit"].ToString();
                    sd.cincinnatiRedsDigit = reader["cincinnatiRedsDigit"].ToString();
                    sd.clevelandIndiansDigit = reader["clevelandIndiansDigit"].ToString();
                    sd.coloradoRockiesDigit = reader["coloradoRockiesDigit"].ToString();
                    sd.detroitTigersDigit = reader["detroitTigersDigit"].ToString();
                    sd.houstonAstrosDigit = reader["houstonAstrosDigit"].ToString();
                    sd.kansasCityRoyalsDigit = reader["kansasCityRoyalsDigit"].ToString();
                    sd.losAngelesAngelsDigit = reader["losAngelesAngelsDigit"].ToString();
                    sd.losAngelesDodgersDigit = reader["losAngelesDodgersDigit"].ToString();
                    sd.miamiMarlinsDigit = reader["miamiMarlinsDigit"].ToString();
                    sd.milwaukeeBrewersDigit = reader["milwaukeeBrewersDigit"].ToString();
                    sd.minnesotaTwinsDigit = reader["minnesotaTwinsDigit"].ToString();
                    sd.newYorkMetsDigit = reader["newYorkMetsDigit"].ToString();
                    sd.newYorkYankeesDigit = reader["newYorkYankeesDigit"].ToString();
                    sd.oaklandAthleticsDigit = reader["oaklandAthleticsDigit"].ToString();
                    sd.philadelphiaPhilliesDigit = reader["philadelphiaPhilliesDigit"].ToString();
                    sd.pittsburghPiratesDigit = reader["pittsburghPiratesDigit"].ToString();
                    sd.sanDiegoPadresDigit = reader["sanDiegoPadresDigit"].ToString();
                    sd.sanFranciscoGiantsDigit = reader["sanFranciscoGiantsDigit"].ToString();
                    sd.seattleMarinersDigit = reader["seattleMarinersDigit"].ToString();
                    sd.stLouisCardinalsDigit = reader["stLouisCardinalsDigit"].ToString();
                    sd.tampaBayRaysDigit = reader["tampaBayRaysDigit"].ToString();
                    sd.texasRangersDigit = reader["texasRangersDigit"].ToString();
                    sd.torontoBlueJaysDigit = reader["torontoBlueJaysDigit"].ToString();
                    sd.washingtonNationalsDigit = reader["washingtonNationalsDigit"].ToString();

                    sessionDays.Add(sd);
                }
            }
            conn.Close();
            return sessionDays;
        }

        public static bool UpdateSessionWithWinners(List<string> winningTeams, Session session, DateTime endDate)
        {
            string winners = string.Join("-", winningTeams);

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Update Session Set WinningTeam=@WinningTeams, SessionStatus=@SessionStatus, EndDate=@EndDate, Where SessionID=@SessionID";
            cmd.Parameters.AddWithValue("SessionID", session.SessionID);
            cmd.Parameters.AddWithValue("WinningTeams", winners);
            cmd.Parameters.AddWithValue("SessionStatus", "Completed");
            cmd.Parameters.AddWithValue("EndDate", endDate.Date);

            //Open the connection if need be
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static List<string> GetWinners(Session latestSess)
        {
            List<string> winners = new List<string>();

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select * from Sessions Where SessionID=@SessionID";
            cmd.Parameters.AddWithValue("SessionID", latestSess.SessionID);

            if(conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            SqlDataReader reader = cmd.ExecuteReader();

            if(reader.HasRows)
            {                
                while(reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string colName = reader.GetName(i);
                        if (colName.Contains("DigitsNeeded"))
                        {
                            string dNeeded = reader[colName].ToString();
                            if (dNeeded == "")
                            {
                                //This time has no digits needed, and is a winnah!
                                int index = colName.IndexOf("DigitsNeeded");
                                string cleanTeamName = (index < 0) ? colName : colName.Remove(index, "DigitsNeeded".Length);
                                winners.Add(cleanTeamName);
                            }
                        }
                    }

                    //var table = reader.GetSchemaTable();
                    //foreach(DataColumn column in table.Columns)
                    //{
                    //    if(column.ColumnName.Contains("DigitsNeeded"))
                    //    {
                    //        string dNeeded = reader[column.ColumnName].ToString();
                    //        if(dNeeded == "")
                    //        {
                    //            //This time has no digits needed, and is a winnah!
                    //            int index = column.ColumnName.IndexOf("DigitsNeeded");
                    //            string cleanTeamName = (index < 0) ? column.ColumnName : column.ColumnName.Remove(index, "DigitsNeeded".Length);
                    //            winners.Add(cleanTeamName);
                    //        }
                    //    }
                    //}
                }
            }
            conn.Close();
            return winners;
        }

        public static bool InsertSessionDay(Session Session)
        {
            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"Insert INTO Sessions (SessionID, SessionStatus, CurrentPot, StartDate, EndDate, WinningTeam, WinningPlayer, arizonaDiamondbacksDigitsNeeded, atlantaBravesDigitsNeeded, baltimoreOriolesDigitsNeeded, " +
                "bostonRedSoxDigitsNeeded, chicagoCubsDigitsNeeded, chicagoWhiteSoxDigitsNeeded, cincinnatiRedsDigitsNeeded, clevelandIndiansDigitsNeeded, coloradoRockiesDigitsNeeded, detroitTigersDigitsNeeded, houstonAstrosDigitsNeeded, kansasCityRoyalsDigitsNeeded, " +
                "losAngelesAngelsDigitsNeeded, losAngelesDodgersDigitsNeeded, miamiMarlinsDigitsNeeded, milwaukeeBrewersDigitsNeeded, minnesotaTwinsDigitsNeeded, newYorkMetsDigitsNeeded, newYorkYankeesDigitsNeeded, oaklandAthleticsDigitsNeeded, " +
                "philadelphiaPhilliesDigitsNeeded, pittsburghPiratesDigitsNeeded, sanDiegoPadresDigitsNeeded, sanFranciscoGiantsDigitsNeeded, seattleMarinersDigitsNeeded, stLouisCardinalsDigitsNeeded, tampaBayRaysDigitsNeeded, texasRangersDigitsNeeded, " +
                "torontoBlueJaysDigitsNeeded, washingtonNationalsDigitsNeeded) " +
                "VALUES (@SessionID, @SessionStatus, @CurrentPot, @StartDate, @EndDate, @WinningTeam, @WinningPlayer, @arizonaDiamondbacksDigitsNeeded, @atlantaBravesDigitsNeeded, @baltimoreOriolesDigitsNeeded, " +
                "@bostonRedSoxDigitsNeeded, @chicagoCubsDigitsNeeded, @chicagoWhiteSoxDigitsNeeded, @cincinnatiRedsDigitsNeeded, @clevelandIndiansDigitsNeeded, @coloradoRockiesDigitsNeeded, @detroitTigersDigitsNeeded, @houstonAstrosDigitsNeeded, @kansasCityRoyalsDigitsNeeded, " +
                "@losAngelesAngelsDigitsNeeded, @losAngelesDodgersDigitsNeeded, @miamiMarlinsDigitsNeeded, @milwaukeeBrewersDigitsNeeded, @minnesotaTwinsDigitsNeeded, @newYorkMetsDigitsNeeded, @newYorkYankeesDigitsNeeded, @oaklandAthleticsDigitsNeeded, " +
                "@philadelphiaPhilliesDigitsNeeded, @pittsburghPiratesDigitsNeeded, @sanDiegoPadresDigitsNeeded, @sanFranciscoGiantsDigitsNeeded, @seattleMarinersDigitsNeeded, @stLouisCardinalsDigitsNeeded, @tampaBayRaysDigitsNeeded, @texasRangersDigitsNeeded, " +
                "@torontoBlueJaysDigitsNeeded, @washingtonNationalsDigitsNeeded)";

            cmd.Parameters.AddWithValue("@SessionID", Session.SessionID);
            cmd.Parameters.AddWithValue("@SessionStatus", Session.SessionStatus);
            cmd.Parameters.AddWithValue("@CurrentPot", Session.CurrentPot);
            cmd.Parameters.AddWithValue("@StartDate", Session.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", Session.EndDate);
            cmd.Parameters.AddWithValue("@WinningTeam", Session.WinningTeam);
            cmd.Parameters.AddWithValue("@WinningPlayer", Session.WinningPlayer);

            cmd.Parameters.AddWithValue("@arizonaDiamondbacksDigitsNeeded", Session.arizonaDiamondbacksDigitsNeeded);
            cmd.Parameters.AddWithValue("@atlantaBravesDigitsNeeded", Session.atlantaBravesDigitsNeeded);
            cmd.Parameters.AddWithValue("@baltimoreOriolesDigitsNeeded", Session.baltimoreOriolesDigitsNeeded);
            cmd.Parameters.AddWithValue("@bostonRedSoxDigitsNeeded", Session.bostonRedsoxDigitsNeeded);
            cmd.Parameters.AddWithValue("@chicagoCubsDigitsNeeded", Session.chicagoCubsDigitsNeeded);
            cmd.Parameters.AddWithValue("@chicagoWhiteSoxDigitsNeeded", Session.chicagoWhiteSoxDigitsNeeded);
            cmd.Parameters.AddWithValue("@cincinnatiRedsDigitsNeeded", Session.cincinnatiRedsDigitsNeeded);
            cmd.Parameters.AddWithValue("@clevelandIndiansDigitsNeeded", Session.clevelandIndiansDigitsNeeded);
            cmd.Parameters.AddWithValue("@coloradoRockiesDigitsNeeded", Session.coloradoRockiesDigitsNeeded);
            cmd.Parameters.AddWithValue("@detroitTigersDigitsNeeded", Session.detroitTigersDigitsNeeded);
            cmd.Parameters.AddWithValue("@houstonAstrosDigitsNeeded", Session.houstonAstrosDigitsNeeded);
            cmd.Parameters.AddWithValue("@kansasCityRoyalsDigitsNeeded", Session.kansasCityRoyalsDigitsNeeded);
            cmd.Parameters.AddWithValue("@losAngelesAngelsDigitsNeeded", Session.losAngelesAngelsDigitsNeeded);
            cmd.Parameters.AddWithValue("@losAngelesDodgersDigitsNeeded", Session.losAngelesDodgersDigitsNeeded);
            cmd.Parameters.AddWithValue("@miamiMarlinsDigitsNeeded", Session.miamiMarlinsDigitsNeeded);
            cmd.Parameters.AddWithValue("@milwaukeeBrewersDigitsNeeded", Session.milwaukeeBrewersDigitsNeeded);
            cmd.Parameters.AddWithValue("@minnesotaTwinsDigitsNeeded", Session.minnesotaTwinsDigitsNeeded);
            cmd.Parameters.AddWithValue("@newYorkMetsDigitsNeeded", Session.newYorkMetsDigitsNeeded);
            cmd.Parameters.AddWithValue("@newYorkYankeesDigitsNeeded", Session.newYorkYankeesDigitsNeeded);
            cmd.Parameters.AddWithValue("@oaklandAthleticsDigitsNeeded", Session.oaklandAthleticsDigitsNeeded);
            cmd.Parameters.AddWithValue("@philadelphiaPhilliesDigitsNeeded", Session.philadelphiaPhilliesDigitsNeeded);
            cmd.Parameters.AddWithValue("@pittsburghPiratesDigitsNeeded", Session.pittsburghPiratesDigitsNeeded);
            cmd.Parameters.AddWithValue("@sanDiegoPadresDigitsNeeded", Session.sanDiegoPadresDigitsNeeded);
            cmd.Parameters.AddWithValue("@sanFranciscoGiantsDigitsNeeded", Session.sanFranciscoGiantsDigitsNeeded);
            cmd.Parameters.AddWithValue("@seattleMarinersDigitsNeeded", Session.seattleMarinersDigitsNeeded);
            cmd.Parameters.AddWithValue("@stLouisCardinalsDigitsNeeded", Session.stLouisCardinalsDigitsNeeded);
            cmd.Parameters.AddWithValue("@tampaBayRaysDigitsNeeded", Session.tampaBayRaysDigitsNeeded);
            cmd.Parameters.AddWithValue("@texasRangersDigitsNeeded", Session.texasRangersDigitsNeeded);
            cmd.Parameters.AddWithValue("@torontoBlueJaysDigitsNeeded", Session.torontoBlueJaysDigitsNeeded);
            cmd.Parameters.AddWithValue("@washingtonNationalsDigitsNeeded", Session.washingtonNationalsDigitsNeeded);

            if(conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            try
            {
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                conn.Close();
                return false;
            }
        }

        public static bool UpdateSessionDay(SessionDay SessionDay)
        {
            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"UPDATE SessionDays SET SessionDayID=@SessionDayID, SessionID=@SessionID, SessionDayDate=@SessionDayDate, GamesThisSessionDate=@GamesThisSessionDate, arizonaDiamondbacksDigit=@arizonaDiamondbacksDigit, " +
                "atlantaBravesDigit=@atlantaBravesDigit, baltimoreOriolesDigit=@baltimoreOriolesDigit, " +
                "bostonRedSoxDigit=@bostonRedSoxDigit, chicagoCubsDigit=@chicagoCubsDigit, chicagoWhiteSoxDigit=@chicagoWhiteSoxDigit, " +
                "cincinnatiRedsDigit=@cincinnatiRedsDigit, clevelandIndiansDigit=@clevelandIndiansDigit, " +
                "coloradoRockiesDigit=@coloradoRockiesDigit, detroitTigersDigit=@detroitTigersDigit, houstonAstrosDigit=@houstonAstrosDigit, kansasCityRoyalsDigit=@kansasCityRoyalsDigit, " +
                "losAngelesAngelsDigit=@losAngelesAngelsDigit, losAngelesDodgersDigit=@losAngelesDodgersDigit, miamiMarlinsDigit=@miamiMarlinsDigit, milwaukeeBrewersDigit=@milwaukeeBrewersDigit, " +
                "minnesotaTwinsDigit=@minnesotaTwinsDigit, newYorkMetsDigit=@newYorkMetsDigit, newYorkYankeesDigit=@newYorkYankeesDigit, oaklandAthleticsDigit=@oaklandAthleticsDigit, " +
                "philadelphiaPhilliesDigit=@philadelphiaPhilliesDigit, pittsburghPiratesDigit=@pittsburghPiratesDigit, sanDiegoPadresDigit=@sanDiegoPadresDigit, sanFranciscoGiantsDigit=@sanFranciscoGiantsDigit, " +
                "seattleMarinersDigit=@seattleMarinersDigit, stLouisCardinalsDigit=@stLouisCardinalsDigit, tampaBayRaysDigit=@tampaBayRaysDigit, texasRangersDigit=@texasRangersDigit, " +
                "torontoBlueJaysDigit=@torontoBlueJaysDigit, washingtonNationalsDigit=@washingtonNationalsDigit " +
                "WHERE SessionDayID=@SessionDayID";

            cmd.Parameters.AddWithValue("SessionDayID", SessionDay.SessionDayID);
            cmd.Parameters.AddWithValue("SessionID", SessionDay.SessionID);
            cmd.Parameters.AddWithValue("SessionDayDate", SessionDay.SessionDayDate);
            cmd.Parameters.AddWithValue("GamesThisSessionDate", SessionDay.GamesThisSessionDay);

            cmd.Parameters.AddWithValue("arizonaDiamondbacksDigit", SessionDay.arizonaDiamondbacksDigit);
            cmd.Parameters.AddWithValue("atlantaBravesDigit", SessionDay.atlantaBravesDigit);
            cmd.Parameters.AddWithValue("baltimoreOriolesDigit", SessionDay.baltimoreOriolesDigit);
            cmd.Parameters.AddWithValue("bostonRedSoxDigit", SessionDay.bostonRedSoxDigit);
            cmd.Parameters.AddWithValue("chicagoCubsDigit", SessionDay.chicagoCubsDigit);
            cmd.Parameters.AddWithValue("chicagoWhiteSoxDigit", SessionDay.chicagoWhiteSoxDigit);
            cmd.Parameters.AddWithValue("cincinnatiRedsDigit", SessionDay.cincinnatiRedsDigit);
            cmd.Parameters.AddWithValue("clevelandIndiansDigit", SessionDay.clevelandIndiansDigit);
            cmd.Parameters.AddWithValue("coloradoRockiesDigit", SessionDay.coloradoRockiesDigit);
            cmd.Parameters.AddWithValue("detroitTigersDigit", SessionDay.detroitTigersDigit);
            cmd.Parameters.AddWithValue("houstonAstrosDigit", SessionDay.houstonAstrosDigit);
            cmd.Parameters.AddWithValue("kansasCityRoyalsDigit", SessionDay.kansasCityRoyalsDigit);
            cmd.Parameters.AddWithValue("losAngelesAngelsDigit", SessionDay.losAngelesAngelsDigit);
            cmd.Parameters.AddWithValue("losAngelesDodgersDigit", SessionDay.losAngelesDodgersDigit);
            cmd.Parameters.AddWithValue("miamiMarlinsDigit", SessionDay.miamiMarlinsDigit);
            cmd.Parameters.AddWithValue("milwaukeeBrewersDigit", SessionDay.milwaukeeBrewersDigit);
            cmd.Parameters.AddWithValue("minnesotaTwinsDigit", SessionDay.minnesotaTwinsDigit);
            cmd.Parameters.AddWithValue("newYorkMetsDigit", SessionDay.newYorkMetsDigit);
            cmd.Parameters.AddWithValue("newYorkYankeesDigit", SessionDay.newYorkYankeesDigit);
            cmd.Parameters.AddWithValue("oaklandAthleticsDigit", SessionDay.oaklandAthleticsDigit);
            cmd.Parameters.AddWithValue("philadelphiaPhilliesDigit", SessionDay.philadelphiaPhilliesDigit);
            cmd.Parameters.AddWithValue("pittsburghPiratesDigit", SessionDay.pittsburghPiratesDigit);
            cmd.Parameters.AddWithValue("sanDiegoPadresDigit", SessionDay.sanDiegoPadresDigit);
            cmd.Parameters.AddWithValue("sanFranciscoGiantsDigit", SessionDay.sanFranciscoGiantsDigit);
            cmd.Parameters.AddWithValue("seattleMarinersDigit", SessionDay.seattleMarinersDigit);
            cmd.Parameters.AddWithValue("stLouisCardinalsDigit", SessionDay.stLouisCardinalsDigit);
            cmd.Parameters.AddWithValue("tampaBayRaysDigit", SessionDay.tampaBayRaysDigit);
            cmd.Parameters.AddWithValue("texasRangersDigit", SessionDay.texasRangersDigit);
            cmd.Parameters.AddWithValue("torontoBlueJaysDigit", SessionDay.torontoBlueJaysDigit);
            cmd.Parameters.AddWithValue("washingtonNationalsDigit", SessionDay.washingtonNationalsDigit);

            if(conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }
            try
            {
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch
            {
                conn.Close();
                return false;
            }

        }

        public static bool UpdateSession(Session Session, SessionDay SessionDay)
        {
            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"UPDATE Sessions SET SessionID=@SessionID, SessionStatus=@SessionStatus, CurrentPot=@CurrentPot, StartDate=@StartDate, EndDate=@EndDate, " +
                "WinningTeam=@WinningTeam, WinningPlayer=@WinningPlayer, arizonaDiamondbacksDigitsNeeded=@arizonaDiamondbacksDigitsNeeded, atlantaBravesDigitsNeeded=@atlantaBravesDigitsNeeded, baltimoreOriolesDigitsNeeded=@baltimoreOriolesDigitsNeeded, " +
                "bostonRedSoxDigitsNeeded=@bostonRedSoxDigitsNeeded, chicagoCubsDigitsNeeded=@chicagoCubsDigitsNeeded, chicagoWhiteSoxDigitsNeeded=@chicagoWhiteSoxDigitsNeeded, cincinnatiRedsDigitsNeeded=@cincinnatiRedsDigitsNeeded, clevelandIndiansDigitsNeeded=@clevelandIndiansDigitsNeeded, " +
                "coloradoRockiesDigitsNeeded=@coloradoRockiesDigitsNeeded, detroitTigersDigitsNeeded=@detroitTigersDigitsNeeded, houstonAstrosDigitsNeeded=@houstonAstrosDigitsNeeded, kansasCityRoyalsDigitsNeeded=@kansasCityRoyalsDigitsNeeded, " +
                "losAngelesAngelsDigitsNeeded=@losAngelesAngelsDigitsNeeded, losAngelesDodgersDigitsNeeded=@losAngelesDodgersDigitsNeeded, miamiMarlinsDigitsNeeded=@miamiMarlinsDigitsNeeded, milwaukeeBrewersDigitsNeeded=@milwaukeeBrewersDigitsNeeded, " +
                "minnesotaTwinsDigitsNeeded=@minnesotaTwinsDigitsNeeded, newYorkMetsDigitsNeeded=@newYorkMetsDigitsNeeded, newYorkYankeesDigitsNeeded=@newYorkYankeesDigitsNeeded, oaklandAthleticsDigitsNeeded=@oaklandAthleticsDigitsNeeded, " +
                "philadelphiaPhilliesDigitsNeeded=@philadelphiaPhilliesDigitsNeeded, pittsburghPiratesDigitsNeeded=@pittsburghPiratesDigitsNeeded, sanDiegoPadresDigitsNeeded=@sanDiegoPadresDigitsNeeded, sanFranciscoGiantsDigitsNeeded=@sanFranciscoGiantsDigitsNeeded, " +
                "seattleMarinersDigitsNeeded=@seattleMarinersDigitsNeeded, stLouisCardinalsDigitsNeeded=@stLouisCardinalsDigitsNeeded, tampaBayRaysDigitsNeeded=@tampaBayRaysDigitsNeeded, texasRangersDigitsNeeded=@texasRangersDigitsNeeded, " +
                "torontoBlueJaysDigitsNeeded=@torontoBlueJaysDigitsNeeded, washingtonNationalsDigitsNeeded=@washingtonNationalsDigitsNeeded " +
                "WHERE SessionID=@SessionID";

            cmd.Parameters.AddWithValue("SessionID", Session.SessionID);
            cmd.Parameters.AddWithValue("SessionStatus", "In progress");
            cmd.Parameters.AddWithValue("CurrentPot", 25);
            cmd.Parameters.AddWithValue("StartDate", Session.StartDate);
            cmd.Parameters.AddWithValue("EndDate", Session.EndDate);
            cmd.Parameters.AddWithValue("WinningTeam", "YoMomma"); //How to calc winning team? Seperate method most likely
            cmd.Parameters.AddWithValue("WinningPlayer", "EatAburger"); //Calc this with serperate method

            cmd.Parameters.AddWithValue("arizonaDiamondbacksDigitsNeeded", GetDigitsNeeded(Session, "arizonaDiamondbacks"));
            cmd.Parameters.AddWithValue("atlantaBravesDigitsNeeded", GetDigitsNeeded(Session, "atlantaBraves"));
            cmd.Parameters.AddWithValue("baltimoreOriolesDigitsNeeded", GetDigitsNeeded(Session, "baltimoreOrioles"));
            cmd.Parameters.AddWithValue("bostonRedSoxDigitsNeeded", GetDigitsNeeded(Session, "bostonRedSox"));
            cmd.Parameters.AddWithValue("chicagoCubsDigitsNeeded", GetDigitsNeeded(Session, "chicagoCubs"));
            cmd.Parameters.AddWithValue("chicagoWhiteSoxDigitsNeeded", GetDigitsNeeded(Session, "chicagoWhiteSox"));
            cmd.Parameters.AddWithValue("cincinnatiRedsDigitsNeeded", GetDigitsNeeded(Session, "cincinnatiReds"));
            cmd.Parameters.AddWithValue("clevelandIndiansDigitsNeeded", GetDigitsNeeded(Session, "clevelandIndians"));
            cmd.Parameters.AddWithValue("coloradoRockiesDigitsNeeded", GetDigitsNeeded(Session, "coloradoRockies"));
            cmd.Parameters.AddWithValue("detroitTigersDigitsNeeded", GetDigitsNeeded(Session, "detroitTigers"));
            cmd.Parameters.AddWithValue("houstonAstrosDigitsNeeded", GetDigitsNeeded(Session, "houstonAstros"));
            cmd.Parameters.AddWithValue("kansasCityRoyalsDigitsNeeded", GetDigitsNeeded(Session, "kansasCityRoyals"));
            cmd.Parameters.AddWithValue("losAngelesAngelsDigitsNeeded", GetDigitsNeeded(Session, "losAngelesAngels"));
            cmd.Parameters.AddWithValue("losAngelesDodgersDigitsNeeded", GetDigitsNeeded(Session, "losAngelesDodgers"));
            cmd.Parameters.AddWithValue("miamiMarlinsDigitsNeeded", GetDigitsNeeded(Session, "miamiMarlins"));
            cmd.Parameters.AddWithValue("milwaukeeBrewersDigitsNeeded", GetDigitsNeeded(Session, "milwaukeeBrewers"));
            cmd.Parameters.AddWithValue("minnesotaTwinsDigitsNeeded", GetDigitsNeeded(Session, "minnesotaTwins"));
            cmd.Parameters.AddWithValue("newYorkMetsDigitsNeeded", GetDigitsNeeded(Session, "newYorkMets"));
            cmd.Parameters.AddWithValue("newYorkYankeesDigitsNeeded", GetDigitsNeeded(Session, "newYorkYankees"));
            cmd.Parameters.AddWithValue("oaklandAthleticsDigitsNeeded", GetDigitsNeeded(Session, "oaklandAthletics"));
            cmd.Parameters.AddWithValue("philadelphiaPhilliesDigitsNeeded", GetDigitsNeeded(Session, "philadelphiaPhillies"));
            cmd.Parameters.AddWithValue("pittsburghPiratesDigitsNeeded", GetDigitsNeeded(Session, "pittsburghPirates"));
            cmd.Parameters.AddWithValue("sanDiegoPadresDigitsNeeded", GetDigitsNeeded(Session, "sanDiegoPadres"));
            cmd.Parameters.AddWithValue("sanFranciscoGiantsDigitsNeeded", GetDigitsNeeded(Session, "sanFranciscoGiants"));
            cmd.Parameters.AddWithValue("seattleMarinersDigitsNeeded", GetDigitsNeeded(Session, "seattleMariners"));
            cmd.Parameters.AddWithValue("stLouisCardinalsDigitsNeeded", GetDigitsNeeded(Session, "stLouisCardinals"));
            cmd.Parameters.AddWithValue("tampaBayRaysDigitsNeeded", GetDigitsNeeded(Session, "tampaBayRays"));
            cmd.Parameters.AddWithValue("texasRangersDigitsNeeded", GetDigitsNeeded(Session, "texasRangers"));
            cmd.Parameters.AddWithValue("torontoBlueJaysDigitsNeeded", GetDigitsNeeded(Session, "torontoBlueJays"));
            cmd.Parameters.AddWithValue("washingtonNationalsDigitsNeeded", GetDigitsNeeded(Session, "washingtonNationals"));

            if(conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            try
            {
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch(Exception ex)
            {
                conn.Close();
                return false;
            }
        }

        public static bool InsertSessionDay(SessionDay SessionDay, Session Session)
        {
            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"Insert INTO SessionDays (SessionDayID, SessionID, SessionDayDate, GamesThisSessionDate, arizonaDiamondbacksDigit, atlantaBravesDigit, baltimoreOriolesDigit, " +
                "bostonRedSoxDigit, chicagoCubsDigit, chicagoWhiteSoxDigit, cincinnatiRedsDigit, clevelandIndiansDigit, coloradoRockiesDigit, detroitTigersDigit, houstonAstrosDigit, kansasCityRoyalsDigit, " +
                "losAngelesAngelsDigit, losAngelesDodgersDigit, miamiMarlinsDigit, milwaukeeBrewersDigit, minnesotaTwinsDigit, newYorkMetsDigit, newYorkYankeesDigit, oaklandAthleticsDigit, " +
                "philadelphiaPhilliesDigit, pittsburghPiratesDigit, sanDiegoPadresDigit, sanFranciscoGiantsDigit, seattleMarinersDigit, stLouisCardinalsDigit, tampaBayRaysDigit, texasRangersDigit, " +
                "torontoBlueJaysDigit, washingtonNationalsDigit) " +
                "VALUES (@SessionDayID, @SessionID, @SessionDayDate, @GamesThisSessionDate, @arizonaDiamondbacksDigit, @atlantaBravesDigit, @baltimoreOriolesDigit, " +
                "@bostonRedSoxDigit, @chicagoCubsDigit, @chicagoWhiteSoxDigit, @cincinnatiRedsDigit, @clevelandIndiansDigit, @coloradoRockiesDigit, @detroitTigersDigit, @houstonAstrosDigit, @kansasCityRoyalsDigit, " +
                "@losAngelesAngelsDigit, @losAngelesDodgersDigit, @miamiMarlinsDigit, @milwaukeeBrewersDigit, @minnesotaTwinsDigit, @newYorkMetsDigit, @newYorkYankeesDigit, @oaklandAthleticsDigit, " +
                "@philadelphiaPhilliesDigit, @pittsburghPiratesDigit, @sanDiegoPadresDigit, @sanFranciscoGiantsDigit, @seattleMarinersDigit, @stLouisCardinalsDigit, @tampaBayRaysDigit, @texasRangersDigit, " +
                "@torontoBlueJaysDigit, @washingtonNationalsDigit)";

            cmd.Parameters.AddWithValue("SessionDayID", SessionDay.SessionDayID);
            cmd.Parameters.AddWithValue("SessionID", SessionDay.SessionID);
            cmd.Parameters.AddWithValue("SessionDayDate", SessionDay.SessionDayDate);
            cmd.Parameters.AddWithValue("GamesThisSessionDate", SessionDay.GamesThisSessionDay);

            cmd.Parameters.AddWithValue("arizonaDiamondbacksDigit", SessionDay.arizonaDiamondbacksDigit);
            cmd.Parameters.AddWithValue("atlantaBravesDigit", SessionDay.atlantaBravesDigit);
            cmd.Parameters.AddWithValue("baltimoreOriolesDigit", SessionDay.baltimoreOriolesDigit);
            cmd.Parameters.AddWithValue("bostonRedSoxDigit", SessionDay.bostonRedSoxDigit);
            cmd.Parameters.AddWithValue("chicagoCubsDigit", SessionDay.chicagoCubsDigit);
            cmd.Parameters.AddWithValue("chicagoWhiteSoxDigit", SessionDay.chicagoWhiteSoxDigit);
            cmd.Parameters.AddWithValue("cincinnatiRedsDigit", SessionDay.cincinnatiRedsDigit);
            cmd.Parameters.AddWithValue("clevelandIndiansDigit", SessionDay.clevelandIndiansDigit);
            cmd.Parameters.AddWithValue("coloradoRockiesDigit", SessionDay.coloradoRockiesDigit);
            cmd.Parameters.AddWithValue("detroitTigersDigit", SessionDay.detroitTigersDigit);
            cmd.Parameters.AddWithValue("houstonAstrosDigit", SessionDay.houstonAstrosDigit);
            cmd.Parameters.AddWithValue("kansasCityRoyalsDigit", SessionDay.kansasCityRoyalsDigit);
            cmd.Parameters.AddWithValue("losAngelesAngelsDigit", SessionDay.losAngelesAngelsDigit);
            cmd.Parameters.AddWithValue("losAngelesDodgersDigit", SessionDay.losAngelesDodgersDigit);
            cmd.Parameters.AddWithValue("miamiMarlinsDigit", SessionDay.miamiMarlinsDigit);
            cmd.Parameters.AddWithValue("milwaukeeBrewersDigit", SessionDay.milwaukeeBrewersDigit);
            cmd.Parameters.AddWithValue("minnesotaTwinsDigit", SessionDay.minnesotaTwinsDigit);
            cmd.Parameters.AddWithValue("newYorkMetsDigit", SessionDay.newYorkMetsDigit);
            cmd.Parameters.AddWithValue("newYorkYankeesDigit", SessionDay.newYorkYankeesDigit);
            cmd.Parameters.AddWithValue("oaklandAthleticsDigit", SessionDay.oaklandAthleticsDigit);
            cmd.Parameters.AddWithValue("philadelphiaPhilliesDigit", SessionDay.philadelphiaPhilliesDigit);
            cmd.Parameters.AddWithValue("pittsburghPiratesDigit", SessionDay.pittsburghPiratesDigit);
            cmd.Parameters.AddWithValue("sanDiegoPadresDigit", SessionDay.sanDiegoPadresDigit);
            cmd.Parameters.AddWithValue("sanFranciscoGiantsDigit", SessionDay.sanFranciscoGiantsDigit);
            cmd.Parameters.AddWithValue("seattleMarinersDigit", SessionDay.seattleMarinersDigit);
            cmd.Parameters.AddWithValue("stLouisCardinalsDigit", SessionDay.stLouisCardinalsDigit);
            cmd.Parameters.AddWithValue("tampaBayRaysDigit", SessionDay.tampaBayRaysDigit);
            cmd.Parameters.AddWithValue("texasRangersDigit", SessionDay.texasRangersDigit);
            cmd.Parameters.AddWithValue("torontoBlueJaysDigit", SessionDay.torontoBlueJaysDigit);
            cmd.Parameters.AddWithValue("washingtonNationalsDigit", SessionDay.washingtonNationalsDigit);

            if(conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            try
            {
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch(Exception ex)
            {
                conn.Close();
                return false;
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
            cmd.CommandText = "Select * From Sessions order by StartDate";

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

                    s.arizonaDiamondbacksDigitsNeeded = (reader["arizonaDiamondbacksDigitsNeeded"].ToString());
                    s.atlantaBravesDigitsNeeded = (reader["atlantaBravesDigitsNeeded"].ToString());
                    s.baltimoreOriolesDigitsNeeded = (reader["baltimoreOriolesDigitsNeeded"].ToString());
                    s.bostonRedsoxDigitsNeeded = (reader["bostonRedsoxDigitsNeeded"].ToString());
                    s.chicagoCubsDigitsNeeded = (reader["chicagoCubsDigitsNeeded"].ToString());
                    s.chicagoWhiteSoxDigitsNeeded = (reader["chicagoWhiteSoxDigitsNeeded"].ToString());
                    s.cincinnatiRedsDigitsNeeded = (reader["cincinnatiRedsDigitsNeeded"].ToString());
                    s.clevelandIndiansDigitsNeeded = (reader["clevelandIndiansDigitsNeeded"].ToString());
                    s.coloradoRockiesDigitsNeeded = (reader["coloradoRockiesDigitsNeeded"].ToString());
                    s.detroitTigersDigitsNeeded = (reader["detroitTigersDigitsNeeded"].ToString());
                    s.houstonAstrosDigitsNeeded = (reader["houstonAstrosDigitsNeeded"].ToString());
                    s.kansasCityRoyalsDigitsNeeded = (reader["kansasCityRoyalsDigitsNeeded"].ToString());
                    s.losAngelesAngelsDigitsNeeded = (reader["losAngelesAngelsDigitsNeeded"].ToString());
                    s.losAngelesDodgersDigitsNeeded = (reader["losAngelesDodgersDigitsNeeded"].ToString());
                    s.miamiMarlinsDigitsNeeded = (reader["miamiMarlinsDigitsNeeded"].ToString());
                    s.milwaukeeBrewersDigitsNeeded = (reader["milwaukeeBrewersDigitsNeeded"].ToString());
                    s.minnesotaTwinsDigitsNeeded = (reader["minnesotaTwinsDigitsNeeded"].ToString());
                    s.newYorkMetsDigitsNeeded = (reader["newYorkMetsDigitsNeeded"].ToString());
                    s.newYorkYankeesDigitsNeeded = (reader["newYorkYankeesDigitsNeeded"].ToString());
                    s.oaklandAthleticsDigitsNeeded = (reader["oaklandAthleticsDigitsNeeded"].ToString());
                    s.philadelphiaPhilliesDigitsNeeded = (reader["philadelphiaPhilliesDigitsNeeded"].ToString());
                    s.pittsburghPiratesDigitsNeeded = (reader["pittsburghPiratesDigitsNeeded"].ToString());
                    s.sanDiegoPadresDigitsNeeded = (reader["sanDiegoPadresDigitsNeeded"].ToString());
                    s.sanFranciscoGiantsDigitsNeeded = (reader["sanFranciscoGiantsDigitsNeeded"].ToString());
                    s.seattleMarinersDigitsNeeded = (reader["seattleMarinersDigitsNeeded"].ToString());
                    s.stLouisCardinalsDigitsNeeded = (reader["stLouisCardinalsDigitsNeeded"].ToString());
                    s.tampaBayRaysDigitsNeeded = (reader["tampaBayRaysDigitsNeeded"].ToString());
                    s.texasRangersDigitsNeeded = (reader["texasRangersDigitsNeeded"].ToString());
                    s.torontoBlueJaysDigitsNeeded = (reader["torontoBlueJaysDigitsNeeded"].ToString());
                    s.washingtonNationalsDigitsNeeded = (reader["washingtonNationalsDigitsNeeded"].ToString());

                    sessions.Add(s);
                }
            }

            conn.Close();
            return sessions;
        }

        public static List<Game> GetGames(DateTime startingDate)
        {
            List<Game> games = new List<Game>();
            string date = startingDate.ToString("yyyy-MM-dd");

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"Select * from Games Where GameDate Like @date";
            cmd.Parameters.AddWithValue("date", "%" + date + "%");

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
                    g.HomeScore = reader["HomeTeamScore"].ToString();
                    g.HomeLastDigit = reader["HomeTeamLastDigit"].ToString();
                    g.AwayScore = reader["AwayTeamScore"].ToString();
                    g.AwayLastDigit = reader["AwayTeamLastDigit"].ToString();
                    g.HomeTeam = reader["HomeTeam"].ToString();
                    g.AwayTeam = reader["AwayTeam"].ToString();
                    g.GameDate = DateTime.Parse(reader["GameDate"].ToString());

                    games.Add(g);
                }
            }

            conn.Close();
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
                    g.HomeScore = reader["HomeScore"].ToString();
                    g.HomeLastDigit = reader["HomeLastDigit"].ToString();
                    g.AwayScore = reader["AwayScore"].ToString();
                    g.AwayLastDigit = reader["AwayLastDigit"].ToString();
                    g.HomeTeam = reader["HomeTeam"].ToString();
                    g.AwayTeam = reader["AwayTeam"].ToString();
                    g.GameDate = DateTime.Parse(reader["GameDate"].ToString());

                    games.Add(g);
                }
            }

            conn.Close();
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
                conn.Close();
                return sro;
            }
            catch (Exception ex)
            {
                sro.Success = false;
                sro.ErrorMessage = ex.Message;
                conn.Close();
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

                conn.Close();
                return gs;
            }
            else
            {
                gs.IsGameInDb = false;
                gs.Status = null;

                conn.Close();
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
                conn.Close();
                return sro;
            }
            catch (Exception ex)
            {
                sro.Success = false;
                sro.ErrorMessage = ex.Message;
                conn.Close();
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

        internal static string GetDigitsNeeded(Session session, string Team)
        {
            //Get all the sessionDays for that session for that team
            //List<SessionDay> sdList = DataAccess.GetSessionDays(session.SessionID, Team);
            List<int> totalDigits = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            List<int> digits = new List<int>();

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select " + Team + "Digit From SessionDays where SessionID=@SessionID";
            cmd.Parameters.AddWithValue("SessionID", session.SessionID);

            //Open the connection if need be
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string teamCol = Team + "Digit";

                    string d = reader[teamCol].ToString();
                    if (d != "-")
                    {
                        int n = Convert.ToInt16(reader[teamCol].ToString());
                        digits.Add(n);
                    }
                    
                }
            }
            conn.Close();

            //Go through the digits we got from the database and SUBTRACT them from the totalDigits. This will
            //leave us with the digits that we need
            foreach(int i in digits)
            {
                totalDigits.Remove(i);
            }

            string digitsToReturn = string.Join(",", totalDigits);

            return digitsToReturn;
        }

        private static List<SessionDay> GetSessionDays(Guid sessionID, string team)
        {
            List<SessionDay> sessionDays = new List<SessionDay>();

            //SQL Connection and Command objects
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EckDB"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select " + team + "Digit From SessionDays where SessionID=@SessionID";
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

                    sd.arizonaDiamondbacksDigit = reader["arizonaDiamondbacksDigit"].ToString();
                    sd.atlantaBravesDigit = reader["atlantaBravesDigit"].ToString();
                    sd.baltimoreOriolesDigit = reader["baltimoreOriolesDigit"].ToString();
                    sd.bostonRedSoxDigit = reader["bostonRedSoxDigit"].ToString();
                    sd.chicagoCubsDigit = reader["chicagoCubsDigit"].ToString();
                    sd.chicagoWhiteSoxDigit = reader["chicagoWhiteSoxDigit"].ToString();
                    sd.cincinnatiRedsDigit = reader["cincinnatiRedsDigit"].ToString();
                    sd.clevelandIndiansDigit = reader["clevelandIndiansDigit"].ToString();
                    sd.coloradoRockiesDigit = reader["coloradoRockiesDigit"].ToString();
                    sd.detroitTigersDigit = reader["detroitTigersDigit"].ToString();
                    sd.houstonAstrosDigit = reader["houstonAstrosDigit"].ToString();
                    sd.kansasCityRoyalsDigit = reader["kansasCityRoyalsDigit"].ToString();
                    sd.losAngelesAngelsDigit = reader["losAngelesAngelsDigit"].ToString();
                    sd.losAngelesDodgersDigit = reader["losAngelesDodgersDigit"].ToString();
                    sd.miamiMarlinsDigit = reader["miamiMarlinsDigit"].ToString();
                    sd.milwaukeeBrewersDigit = reader["milwaukeeBrewersDigit"].ToString();
                    sd.minnesotaTwinsDigit = reader["minnesotaTwinsDigit"].ToString();
                    sd.newYorkMetsDigit = reader["newYorkMetsDigit"].ToString();
                    sd.newYorkYankeesDigit = reader["newYorkYankeesDigit"].ToString();
                    sd.oaklandAthleticsDigit = reader["oaklandAthleticsDigit"].ToString();
                    sd.philadelphiaPhilliesDigit = reader["philadelphiaPhilliesDigit"].ToString();
                    sd.pittsburghPiratesDigit = reader["pittsburghPiratesDigit"].ToString();
                    sd.sanDiegoPadresDigit = reader["sanDiegoPadresDigit"].ToString();
                    sd.sanFranciscoGiantsDigit = reader["sanFranciscoGiantsDigit"].ToString();
                    sd.seattleMarinersDigit = reader["seattleMarinersDigit"].ToString();
                    sd.stLouisCardinalsDigit = reader["stLouisCardinalsDigit"].ToString();
                    sd.tampaBayRaysDigit = reader["tampaBayRaysDigit"].ToString();
                    sd.texasRangersDigit = reader["texasRangersDigit"].ToString();
                    sd.torontoBlueJaysDigit = reader["torontoBlueJaysDigit"].ToString();
                    sd.washingtonNationalsDigit = reader["washingtonNationalsDigit"].ToString();

                    sessionDays.Add(sd);
                }
            }

            conn.Close();
            return sessionDays;
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
        public string HomeScore { get; set; }
        public string HomeLastDigit { get; set; }
        public string AwayScore { get; set; }
        public string AwayLastDigit { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime GameDate { get; set; }
    }
    public class SessionDay
    {
        private List<Game> games;

        public SessionDay(List<Game> games, Session Session)
        {
            this.games = games;

            this.SessionDayID = Guid.NewGuid();
            this.SessionID = Session.SessionID;
            this.SessionDayDate = games[0].GameDate; //Assumes Games has Game objects in it.
            this.GamesThisSessionDay = games.Count;

            foreach (Game g in games)
            {
                //Build the parameters depending on what scores were had that day for whatever teams
                #region GnarlySwitch
                switch (g.HomeTeam)
                {
                    case "Arizona Diamondbacks":
                        this.arizonaDiamondbacksDigit = g.HomeLastDigit;
                        break;
                    case "Atlanta Braves":
                        this.atlantaBravesDigit = g.HomeLastDigit;
                        break;
                    case "Baltimore Orioles":
                        this.baltimoreOriolesDigit = g.HomeLastDigit;
                        break;
                    case "Boston Red Sox":
                        this.bostonRedSoxDigit = g.HomeLastDigit;
                        break;
                    case "Chicago Cubs":
                        this.chicagoCubsDigit = g.HomeLastDigit;
                        break;
                    case "Chicago White Sox":
                        this.chicagoWhiteSoxDigit = g.HomeLastDigit;
                        break;
                    case "Cincinnati Reds":
                        this.cincinnatiRedsDigit = g.HomeLastDigit;
                        break;
                    case "Cleveland Indians":
                        this.clevelandIndiansDigit = g.HomeLastDigit;
                        break;
                    case "Colorado Rockies":
                        this.coloradoRockiesDigit = g.HomeLastDigit;
                        break;
                    case "Detroit Tigers":
                        this.detroitTigersDigit = g.HomeLastDigit;
                        break;
                    case "Houston Astros":
                        this.houstonAstrosDigit = g.HomeLastDigit;
                        break;
                    case "Kansas City Royals":
                        this.kansasCityRoyalsDigit = g.HomeLastDigit;
                        break;
                    case "Los Angeles Angels":
                        this.losAngelesAngelsDigit = g.HomeLastDigit;
                        break;
                    case "Los Angeles Dodgers":
                        this.losAngelesDodgersDigit = g.HomeLastDigit;
                        break;
                    case "Miami Marlins":
                        this.miamiMarlinsDigit = g.HomeLastDigit;
                        break;
                    case "Milwaukee Brewers":
                        this.milwaukeeBrewersDigit = g.HomeLastDigit;
                        break;
                    case "Minnesota Twins":
                        this.minnesotaTwinsDigit = g.HomeLastDigit;
                        break;
                    case "New York Mets":
                        this.newYorkMetsDigit = g.HomeLastDigit;
                        break;
                    case "New York Yankees":
                        this.newYorkYankeesDigit = g.HomeLastDigit;
                        break;
                    case "Oakland Athletics":
                        this.oaklandAthleticsDigit = g.HomeLastDigit;
                        break;
                    case "Philadelphia Phillies":
                        this.philadelphiaPhilliesDigit = g.HomeLastDigit;
                        break;
                    case "Pittsburgh Pirates":
                        this.pittsburghPiratesDigit = g.HomeLastDigit;
                        break;
                    case "San Diego Padres":
                        this.sanDiegoPadresDigit = g.HomeLastDigit;
                        break;
                    case "San Francisco Giants":
                        this.sanFranciscoGiantsDigit = g.HomeLastDigit;
                        break;
                    case "Seattle Mariners":
                        this.seattleMarinersDigit = g.HomeLastDigit;
                        break;
                    case "St. Louis Cardinals":
                        this.stLouisCardinalsDigit = g.HomeLastDigit;
                        break;
                    case "Tampa Bay Rays":
                        this.tampaBayRaysDigit = g.HomeLastDigit;
                        break;
                    case "Texas Rangers":
                        this.texasRangersDigit = g.HomeLastDigit;
                        break;
                    case "Toronto Blue Jays":
                        this.torontoBlueJaysDigit = g.HomeLastDigit;
                        break;
                    case "Washington Nationals":
                        this.washingtonNationalsDigit = g.HomeLastDigit;
                        break;
                }

                //switch g.AwayTeam
                switch (g.AwayTeam)
                {
                    case "Arizona Diamondbacks":
                        this.arizonaDiamondbacksDigit = g.AwayLastDigit;
                        break;
                    case "Atlanta Braves":
                        this.atlantaBravesDigit = g.AwayLastDigit;
                        break;
                    case "Baltimore Orioles":
                        this.baltimoreOriolesDigit = g.AwayLastDigit;
                        break;
                    case "Boston Red Sox":
                        this.bostonRedSoxDigit = g.AwayLastDigit;
                        break;
                    case "Chicago Cubs":
                        this.chicagoCubsDigit = g.AwayLastDigit;
                        break;
                    case "Chicago White Sox":
                        this.chicagoWhiteSoxDigit = g.AwayLastDigit;
                        break;
                    case "Cincinnati Reds":
                        this.cincinnatiRedsDigit = g.AwayLastDigit;
                        break;
                    case "Cleveland Indians":
                        this.clevelandIndiansDigit = g.AwayLastDigit;
                        break;
                    case "Colorado Rockies":
                        this.coloradoRockiesDigit = g.AwayLastDigit;
                        break;
                    case "Detroit Tigers":
                        this.detroitTigersDigit = g.AwayLastDigit;
                        break;
                    case "Houston Astros":
                        this.houstonAstrosDigit = g.AwayLastDigit;
                        break;
                    case "Kansas City Royals":
                        this.kansasCityRoyalsDigit = g.AwayLastDigit;
                        break;
                    case "Los Angeles Angels":
                        this.losAngelesAngelsDigit = g.AwayLastDigit;
                        break;
                    case "Los Angeles Dodgers":
                        this.losAngelesDodgersDigit = g.AwayLastDigit;
                        break;
                    case "Miami Marlins":
                        this.miamiMarlinsDigit = g.AwayLastDigit;
                        break;
                    case "Milwaukee Brewers":
                        this.milwaukeeBrewersDigit = g.AwayLastDigit;
                        break;
                    case "Minnesota Twins":
                        this.minnesotaTwinsDigit = g.AwayLastDigit;
                        break;
                    case "New York Mets":
                        this.newYorkMetsDigit = g.AwayLastDigit;
                        break;
                    case "New York Yankees":
                        this.newYorkYankeesDigit = g.AwayLastDigit;
                        break;
                    case "Oakland Athletics":
                        this.oaklandAthleticsDigit = g.AwayLastDigit;
                        break;
                    case "Philadelphia Phillies":
                        this.philadelphiaPhilliesDigit = g.AwayLastDigit;
                        break;
                    case "Pittsburgh Pirates":
                        this.pittsburghPiratesDigit = g.AwayLastDigit;
                        break;
                    case "San Diego Padres":
                        this.sanDiegoPadresDigit = g.AwayLastDigit;
                        break;
                    case "San Francisco Giants":
                        this.sanFranciscoGiantsDigit = g.AwayLastDigit;
                        break;
                    case "Seattle Mariners":
                        this.seattleMarinersDigit = g.AwayLastDigit;
                        break;
                    case "St. Louis Cardinals":
                        this.stLouisCardinalsDigit = g.AwayLastDigit;
                        break;
                    case "Tampa Bay Rays":
                        this.tampaBayRaysDigit = g.AwayLastDigit;
                        break;
                    case "Texas Rangers":
                        this.texasRangersDigit = g.AwayLastDigit;
                        break;
                    case "Toronto Blue Jays":
                        this.torontoBlueJaysDigit = g.AwayLastDigit;
                        break;
                    case "Washington Nationals":
                        this.washingtonNationalsDigit = g.AwayLastDigit;
                        break;
                }
                #endregion
            }
        }

        public SessionDay()
        {
        }

        public Guid SessionDayID { get; set; }
        public Guid SessionID { get; set; }
        public DateTime SessionDayDate { get; set; }
        public int GamesThisSessionDay { get; internal set; }

        public string arizonaDiamondbacksDigit {get;set;} = "-";
        public string atlantaBravesDigit { get; set; } = "-";
        public string baltimoreOriolesDigit { get; set; } = "-";
        public string bostonRedSoxDigit { get; set; } = "-";
        public string chicagoCubsDigit { get; set; } = "-";
        public string chicagoWhiteSoxDigit { get; set; } = "-";
        public string cincinnatiRedsDigit { get; set; } = "-";
        public string clevelandIndiansDigit { get; set; } = "-";
        public string coloradoRockiesDigit { get; set; } = "-";
        public string detroitTigersDigit { get; set; } = "-";
        public string houstonAstrosDigit { get; set; } = "-";
        public string kansasCityRoyalsDigit { get; set; } = "-";
        public string losAngelesAngelsDigit { get; set; } = "-";
        public string losAngelesDodgersDigit { get; set; } = "-";
        public string miamiMarlinsDigit { get; set; } = "-";
        public string milwaukeeBrewersDigit { get; set; } = "-";
        public string minnesotaTwinsDigit { get; set; } = "-";
        public string newYorkMetsDigit { get; set; } = "-";
        public string newYorkYankeesDigit { get; set; } = "-";
        public string oaklandAthleticsDigit { get; set; } = "-";
        public string philadelphiaPhilliesDigit { get; set; } = "-";
        public string pittsburghPiratesDigit { get; set; } = "-";
        public string sanDiegoPadresDigit { get; set; } = "-";
        public string sanFranciscoGiantsDigit { get; set; } = "-";
        public string seattleMarinersDigit { get; set; } = "-";
        public string stLouisCardinalsDigit { get; set; } = "-";
        public string tampaBayRaysDigit { get; set; } = "-";
        public string texasRangersDigit { get; set; } = "-";
        public string torontoBlueJaysDigit { get; set; } = "-";
        public string washingtonNationalsDigit { get; set; } = "-";
    }
    public class Session
    {
        public Guid SessionID { get; set; }
        public string SessionStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string WinningPlayer { get; set; }
        public string WinningTeam { get; set; }
        public decimal CurrentPot { get; set; }
        public string arizonaDiamondbacksDigitsNeeded { get; set; }
        public string atlantaBravesDigitsNeeded { get; set; }
        public string baltimoreOriolesDigitsNeeded { get; set; }
        public string bostonRedsoxDigitsNeeded { get; set; }
        public string chicagoCubsDigitsNeeded { get; set; }
        public string chicagoWhiteSoxDigitsNeeded { get; set; }
        public string cincinnatiRedsDigitsNeeded { get; set; }
        public string clevelandIndiansDigitsNeeded { get; set; }
        public string coloradoRockiesDigitsNeeded { get; set; }
        public string detroitTigersDigitsNeeded { get; set; }
        public string houstonAstrosDigitsNeeded { get; set; }
        public string kansasCityRoyalsDigitsNeeded { get; set; }
        public string losAngelesAngelsDigitsNeeded { get; set; }
        public string losAngelesDodgersDigitsNeeded { get; set; }
        public string miamiMarlinsDigitsNeeded { get; set; }
        public string milwaukeeBrewersDigitsNeeded { get; set; }
        public string minnesotaTwinsDigitsNeeded { get; set; }
        public string newYorkMetsDigitsNeeded { get; set; }
        public string newYorkYankeesDigitsNeeded { get; set; }
        public string oaklandAthleticsDigitsNeeded { get; set; }
        public string philadelphiaPhilliesDigitsNeeded { get; set; }
        public string pittsburghPiratesDigitsNeeded { get; set; }
        public string sanDiegoPadresDigitsNeeded { get; set; }
        public string sanFranciscoGiantsDigitsNeeded { get; set; }
        public string seattleMarinersDigitsNeeded { get; set; }
        public string stLouisCardinalsDigitsNeeded { get; set; }
        public string tampaBayRaysDigitsNeeded { get; set; }
        public string texasRangersDigitsNeeded { get; set; }
        public string torontoBlueJaysDigitsNeeded { get; set; }
        public string washingtonNationalsDigitsNeeded { get; set; }
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

