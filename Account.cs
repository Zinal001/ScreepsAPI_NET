using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreepsAPI_NET
{
    public class Account
    {
        /// <summary>
        /// The unique user id of the account
        /// </summary>
        public String _Id { get; set; }

        /// <summary>
        /// The email address of the account
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// The username of the account
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// Your CPU
        /// </summary>
        public int CPU { get; set; }

        /// <summary>
        /// The badge information
        /// </summary>
        public Badge Badge { get; set; }

        /// <summary>
        /// Is the password set on this account
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// Last UNIX timestamp of spawn
        /// </summary>
        public long LastRespawnDate { get; set; }

        /// <summary>
        /// The Global Control Level
        /// </summary>
        public long GCL { get; set; }

        /// <summary>
        /// The CPU Credits of this account
        /// </summary>
        public long Credits { get; set; }

        /// <summary>
        /// Twitter account information
        /// Might be null
        /// </summary>
        public TwitterAccount TwitterAccount { get; set; }

        /// <summary>
        /// Github account information
        /// Might null
        /// </summary>
        public GithubAccount GithubAccount { get; set; }

        public Account()
        {

        }

        public static Account Parse(JObject obj)
        {
            try
            {
                Account acc = new Account();

                acc._Id = obj.Value<String>("_id");
                acc.Email = obj.Value<String>("email");
                acc.Username = obj.Value<String>("username");
                acc.CPU = obj.Value<int>("cpu");
                acc.Badge = Badge.Parse(obj.Value<JObject>("badge"));
                acc.Password = obj.Value<bool>("password");
                acc.LastRespawnDate = obj.Value<long>("lastRespawnDate");
                acc.GCL = obj.Value<long>("gcl");
                acc.Credits = obj.Value<long>("credits");
                acc.TwitterAccount = TwitterAccount.Parse(obj.Value<JObject>("twitter"));
                acc.GithubAccount = GithubAccount.Parse(obj.Value<JObject>("github"));

                return acc;
            }
            catch
            {

            }

            return null;
        }
        

    }
}
