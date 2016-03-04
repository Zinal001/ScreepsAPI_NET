using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ScreepsAPI_NET
{
    public class SerializedTerrainData
    {
        /// <summary>
        /// The ID of the room?
        /// </summary>
        public String _Id { get; set; }

        /// <summary>
        /// The name of the room
        /// </summary>
        public String RoomName { get; set; }

        /// <summary>
        /// The Terrain Data in serialized form.
        /// <para>Terrain is read Left-To-Right and Top-To-Bottom.</para>
        /// <para>Types: 0 = plain, 1 = wall, 2 = swamp, 3 = wall?</para>
        /// </summary>
        public String Terrain { get; set; }

        /// <summary>
        /// The type of this serialized data
        /// Always "terrain"
        /// </summary>
        public String Type { get; set; }


        public SerializedTerrainData()
        {

        }

        public static SerializedTerrainData Parse(JObject obj)
        {
            try
            {
                SerializedTerrainData std = new SerializedTerrainData();

                std._Id = obj.GetValue<String>("_id");
                std.RoomName = obj.GetValue<String>("room");
                std.Terrain = obj.GetValue<String>("terrain");
                std.Type = obj.GetValue<String>("type");

                return std;
            }
            catch { }

            return null;
        }

    }
}
