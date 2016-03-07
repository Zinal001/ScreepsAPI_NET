using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ScreepsAPI_NET
{
    public class RoomOverlayData
    {

        /// <summary>
        /// An array of position containing a constructed wall
        /// </summary>
        public Pos[] Walls { get; set; }

        /// <summary>
        /// An array of positions containing a road
        /// </summary>
        public Pos[] Roads { get; set; }

        /// <summary>
        /// A dictionary where the key is a User_Id and the value is an array of positions containing a creep
        /// </summary>
        public Dictionary<String, Pos[]> Users { get; set; }

        public RoomOverlayData()
        {
            this.Users = new Dictionary<String, Pos[]>();
        }

        public static RoomOverlayData Parse(JObject obj)
        {
            try
            {
                RoomOverlayData data = new RoomOverlayData();

                List<Pos> tmpList = new List<Pos>();

                JArray JWalls = obj.GetValue<JArray>("walls");
                foreach (JArray JPos in JWalls)
                    tmpList.Add(new Pos(JPos.Value<int>(0), JPos.Value<int>(1)));

                data.Walls = tmpList.ToArray();

                tmpList.Clear();

                JArray JRoads = obj.GetValue<JArray>("roads");
                foreach (JArray JPos in JRoads)
                    tmpList.Add(new Pos(JPos.Value<int>(0), JPos.Value<int>(1)));

                data.Roads = tmpList.ToArray();

                tmpList.Clear();

                IEnumerator<KeyValuePair<String, JToken>> all = obj.GetEnumerator();

                while(all.MoveNext())
                {
                    if (all.Current.Key != "walls" && all.Current.Key != "roads")
                    {
                        List<Pos> Positions = new List<Pos>();
                        foreach (JArray JPos in all.Current.Value.Value<JArray>())
                            Positions.Add(new Pos(JPos.Value<int>(0), JPos.Value<int>(1)));
                        data.Users.Add(all.Current.Key, Positions.ToArray());
                    }
                }

                return data;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

    }

    /// <summary>
    /// A Position object
    /// </summary>
    public struct Pos
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Pos(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
