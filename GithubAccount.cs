using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreepsAPI_NET
{
    public class GithubAccount
    {
        /// <summary>
        /// The Id of the Github Account
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// The username of the Github Account
        /// </summary>
        public String Username { get; set; }

        public GithubAccount()
        {

        }

        public static GithubAccount Parse(JObject obj)
        {
            try
            {
                GithubAccount acc = new GithubAccount();

                acc.Id = obj.GetValue<String>("id");
                acc.Username = obj.GetValue<String>("username");

                return acc;
            }
            catch { }

            return null;
        }

    }
}
