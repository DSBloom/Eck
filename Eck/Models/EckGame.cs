using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eck.Models
{
    public class EckGame
    {
        public int HomeScore { get; set; }
        public int HomeLastDigit { get; set; }
        public int AwayScore { get; set; }
        public int AwayLastDigit { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime GameDate { get; set; }
    }
}