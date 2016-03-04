using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ScreepsAPI_NET
{
    public class TerrainData
    {
        /// <summary>
        /// The name of the room
        /// </summary>
        public String RoomName { get; set; }

        /// <summary>
        /// The X position of the room
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The Y position of the room
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The type of terrain
        /// Known values: plain, wall or swamp
        /// </summary>
        public String Type { get; set; }

        public TerrainData()
        {

        }

        public static TerrainData Parse(JObject obj)
        {
            try
            {
                TerrainData td = new TerrainData();

                td.RoomName = obj.GetValue<String>("room");
                td.X = obj.GetValue<int>("x");
                td.Y = obj.GetValue<int>("y");
                td.Type = obj.GetValue<String>("type");

                return td;
            }
            catch { }

            return null;
        }

        public static TerrainData[] ParseList(JArray arr)
        {

            try
            {
                List<TerrainData> terrain = new List<TerrainData>(arr.Count);

                foreach(JObject obj in arr)
                {
                    TerrainData td = TerrainData.Parse(obj);
                    if (td != null)
                        terrain.Add(td);
                }

                return terrain.ToArray();
            }
            catch { }

            return new TerrainData[0];
        }


    }
}
