using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreepsAPI_NET
{
    public class User
    {
        /// <summary>
        /// The uniquie Id of the user
        /// </summary>
        public String _Id { get; set; }

        /// <summary>
        /// The username
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// The badge information
        /// </summary>
        public Badge Badge { get; set; }

        /// <summary>
        /// The Global Control Level
        /// </summary>
        public long GCL { get; set; }

        public User()
        {

        }

        public User(String _Id, String Username, Badge Badge, long GCL)
        {
            this._Id = _Id;
            this.Username = Username;
            this.Badge = Badge;
            this.GCL = GCL;
        }

        public static User Parse(JObject obj)
        {
            try
            {
                User u = new User();

                u._Id = obj.GetValue<String>("_id");
                u.Username = obj.GetValue<String>("username");
                u.Badge = Badge.Parse(obj.GetValue<JObject>("badge"));
                u.GCL = obj.GetValue<long>("gcl");


                return u;
            }
            catch
            {

            }

            return null;
        }

    }
}
