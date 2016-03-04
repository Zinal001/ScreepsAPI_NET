using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreepsAPI_NET
{
    public class TwitterAccount
    {
        /// <summary>
        /// The username of the Twitter account
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// The number of followers of the Twitter account
        /// </summary>
        public int Followers { get; set; }

        public TwitterAccount()
        {

        }

        public TwitterAccount(String Username, int Followers)
        {
            this.Username = Username;
            this.Followers = Followers;
        }

        public static TwitterAccount Parse(JObject obj)
        {
            try
            {
                TwitterAccount acc = new TwitterAccount();

                acc.Username = obj.Value<String>("username");
                acc.Followers = obj.Value<int>("followers_count");

                return acc;
            }
            catch
            {

            }

            return null;
        }
    }
}
