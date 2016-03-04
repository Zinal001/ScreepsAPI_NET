using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ScreepsAPI_NET
{
    public class RoomOverlayData
    {

        public Pos[] Walls { get; set; }

        public Pos[] Roads { get; set; }

        public Dictionary<String, Pos[]> Users { get; set; }

        public RoomOverlayData()
        {

        }

        public static RoomOverlayData Parse(JObject obj)
        {
            try
            {
                RoomOverlayData data = new RoomOverlayData();

                List<Pos> tmpList = new List<Pos>();

                JArray JWalls = obj.GetValue<JArray>("walls");
                foreach (JObject JPos in JWalls)
                    tmpList.Add(new Pos(JPos.GetValue<int>("x"), JPos.GetValue<int>("y")));

                data.Walls = tmpList.ToArray();

                tmpList.Clear();

                JArray JRoads = obj.GetValue<JArray>("roads");
                foreach (JObject JPos in JRoads)
                    tmpList.Add(new Pos(JPos.GetValue<int>("x"), JPos.GetValue<int>("y")));

                data.Roads = tmpList.ToArray();

                tmpList.Clear();

                IEnumerator<KeyValuePair<String, JToken>> all = obj.GetEnumerator();

                while(all.MoveNext())
                {
                    if(all.Current.Key != "walls" && all.Current.Key != "roads")
                        
                }


            }
            catch { }

            return null;
        }

    }

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
