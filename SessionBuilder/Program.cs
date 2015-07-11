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
            //Global
            List<Session> sessions = new List<GetScores.Session>();

            //Get all of the games from the database. Should be ordered by GameDate starting with the earliest
            List<Game> games = DataAccess.GetGames();

            //Start at the earliest game and insert a new record in team progress every day
            foreach (Game g in games)
            {
            }


            //Also update the session every day until 
        }
    }
}
