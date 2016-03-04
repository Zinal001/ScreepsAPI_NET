using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreepsAPI_NET
{
    public class Leaderboard
    {
        /// <summary>
        /// The Id of the leaderboard
        /// </summary>
        public String _Id { get; set; }

        /// <summary>
        /// The season with the format: YEAR-Month where Month is 2 digits
        /// </summary>
        public String Season { get; set; }

        /// <summary>
        /// The User id of the leaderboard
        /// </summary>
        public String User_Id { get; set; }

        /// <summary>
        /// The user object of the the leaderboard
        /// Might be null
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The score of the user
        /// </summary>
        public long Score { get; set; }

        /// <summary>
        /// The rank of the user
        /// </summary>
        public int Rank { get; set; }

        public Leaderboard()
        {

        }

        public static Leaderboard Parse(JObject obj)
        {
            try
            {
                Leaderboard board = new Leaderboard();

                board._Id = obj.GetValue<String>("_id");
                board.Season = obj.GetValue<String>("season");
                board.User_Id = obj.GetValue<String>("user");
                board.Score = obj.GetValue<long>("score");
                board.Rank = obj.GetValue<int>("rank");

                return board;
            }
            catch { }

            return null;
        }

        public static Leaderboard[] ParseList(JArray list)
        {
            try
            {
                List<Leaderboard> boards = new List<Leaderboard>();
                foreach(JObject obj in list)
                {
                    Leaderboard board = Leaderboard.Parse(obj);
                    if (board != null)
                        boards.Add(board);
                }

                return boards.ToArray();
            }
            catch { }

            return new Leaderboard[0];
        }

    }
}
