using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreepsAPI_NET
{
    public class Season
    {
        /// <summary>
        /// The Id of the season
        /// Can be used with any season function instead of season name
        /// </summary>
        public String _Id { get; set; }

        /// <summary>
        /// The name of the season
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The date and time when the season started
        /// </summary>
        public DateTime Date { get; set; }

        public Season()
        {

        }

        public static Season Parse(JObject obj)
        {
            try
            {
                Season season = new Season();

                season._Id = obj.GetValue<String>("_id");
                season.Name = obj.GetValue<String>("name");
                season.Date = obj.GetValue<DateTime>("date");

                return season;
            }
            catch { }

            return null;
        }

        public static Season[] ParseList(JArray arr)
        {
            try
            {
                List<Season> seasons = new List<Season>();

                foreach(JObject obj in arr)
                {
                    Season season = Season.Parse(obj);
                    if (season != null)
                        seasons.Add(season);
                }

                return seasons.ToArray();
            }
            catch { }

            return new Season[0];
        }

    }
}
