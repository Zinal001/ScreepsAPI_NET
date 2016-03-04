using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreepsAPI_NET
{
    public class Badge
    {
        /// <summary>
        /// The type number of the badge
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// The first color of the badge
        /// </summary>
        public int Color1 { get; set; }

        /// <summary>
        /// The second color of the badge
        /// </summary>
        public int Color2 { get; set; }

        /// <summary>
        /// The thrid color of the badge
        /// </summary>
        public int Color3 { get; set; }

        /// <summary>
        /// ??????
        /// </summary>
        public int Param { get; set; }

        /// <summary>
        /// Is the badge flipped
        /// </summary>
        public bool Flip { get; set; }

        public Badge()
        {

        }

        public static Badge Parse(JObject obj)
        {
            try
            {
                Badge badge = new Badge();

                badge.Type = obj.GetValue<int>("type");
                badge.Color1 = obj.GetValue<int>("color1");
                badge.Color2 = obj.GetValue<int>("color2");
                badge.Color3 = obj.GetValue<int>("color3");
                badge.Param = obj.GetValue<int>("param");
                badge.Flip = obj.GetValue<bool>("flip");

                return badge;
            }
            catch
            {

            }

            return null;
        }

    }
}
