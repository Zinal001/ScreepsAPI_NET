using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace ScreepsAPI_NET
{
    /// <summary>
    /// The primary class of the library.
    /// </summary>
    public class API
    {
        /// <summary>
        /// The base URI to the API
        /// </summary>
        public static readonly String APIUrl = "https://screeps.com/api/";

        /// <summary>
        /// The Authorization Token given by the SignIn function
        /// </summary>
        public String AuthToken { get; private set; }

        /// <summary>
        /// The Webclient that does all the work
        /// </summary>
        private WebClient wc;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public API()
        {
            this.wc = new WebClient();
        }

        #region Authorization

        /// <summary>
        /// Authorize against the API
        /// This function most be the first to be called
        /// </summary>
        /// <param name="email">The email of your Screep account</param>
        /// <param name="password">The password</param>
        /// <param name="ex">The exception is set if an error occurred (Like invalid username or password)</param>
        /// <returns></returns>
        public bool SignIn(String email, String password, out WebException ex)
        {
            ex = null;
            wcReset();
            wc.Headers.Add(HttpRequestHeader.Authorization, GetAuthString(email, password));

            try
            {
                JObject ret = this.Post("auth/signin", new NameValueCollection() {
                    { "email", email },
                    { "password", password }
                });

                JToken error;
                JToken ok;

                if (ret.TryGetValue("error", out error))
                    return false;
                else if (ret.TryGetValue("ok", out ok))
                {
                    this.AuthToken = ret.Value<String>("token");
                }
            }
            catch(WebException wex)
            {
                ex = wex;
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Authorize against the API
        /// This function most be the first to be called
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <param name="email">The email of your Screep account</param>
        /// <param name="password">The password</param>
        /// <returns></returns>
        public bool SignIn(String email, String password)
        {
            WebException ex;
            return this.SignIn(email, password, out ex);
        }

        /// <summary>
        /// Set the Authorization Token to a specific value
        /// </summary>
        /// <param name="AuthToken"></param>
        public void SetAuthToken(String AuthToken)
        {
            this.AuthToken = AuthToken;
        }

        #endregion

        #region User Information

        /// <summary>
        /// Get general information of a user
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <returns></returns>
        public User FindUser(String username, out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return null;
            }
            wcReset();

            JObject ret = this.Get("user/find", new NameValueCollection() { { "username", username } });

            if(ret.GetValue<int>("ok") == 1)
            {
                JObject JUser = ret.Value<JObject>("user");
                if (JUser == null)
                    return null;

                return User.Parse(JUser);
            }

            return null;
        }

        /// <summary>
        /// Get general information of a user
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <param name="username">The username to search for</param>
        public User FindUser(String username)
        {
            Exception ex;
            return FindUser(username, out ex);
        }

        /// <summary>
        /// Get your account information such as username, notification preferences and credits
        /// </summary>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <returns></returns>
        public Account Me(out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return null;
            }
            wcReset();

            JObject ret = this.Get("auth/me", new NameValueCollection());

            if (ret.GetValue<int>("ok") == 1)
            {
                return Account.Parse(ret);
            }

            return null;
        }

        /// <summary>
        /// Get your account information such as username, notification preferences and credits
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <returns></returns>
        public Account Me()
        {
            Exception ex;
            return this.Me(out ex);
        }

        /// <summary>
        /// WORK IN PROGRESS - DO NOT USE
        /// </summary>
        /// <remarks>WIP</remarks>
        public void Overview()
        {
            JObject ret = this.Get("user/overview", new NameValueCollection() {
                { "stats", "8" },
                { "statName", "energyHarvestedasd" }
            });

            Console.WriteLine("Hmm");
        }

        #endregion

        #region Messages

        /// <summary>
        /// Get the last messages in your inbox & outbox
        /// </summary>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <returns></returns>
        public Message[] GetMessages(out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return null;
            }
            wcReset();

            JObject ret = this.Get("user/messages/index", new NameValueCollection());

            if (ret.GetValue<int>("ok") == 1)
            {
                Message[] messages = Message.ParseMessages(ret);

                return messages;
            }

            return new Message[0];
        }

        /// <summary>
        /// Get the last messages in your inbox & outbox
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <returns></returns>
        public Message[] GetMessages()
        {
            Exception ex;
            return GetMessages(out ex);
        }

        /// <summary>
        /// Get all messages from or to a specific user
        /// </summary>
        /// <param name="respondent_id">The id of the user</param>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <returns></returns>
        public Message[] GetMessages(String respondent_id, out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return null;
            }
            wcReset();

            JObject ret = this.Get("user/messages/list", new NameValueCollection() { { "respondent", respondent_id } });

            if (ret.GetValue<int>("ok") == 1)
            {
                JArray jMessages = ret.GetValue<JArray>("messages");

                List<Message> messages = new List<Message>(jMessages.Count);

                foreach(JObject obj in jMessages)
                {
                    Message msg = Message.Parse(obj);
                    if (msg != null)
                    {
                        msg.Respondent_Id = respondent_id;
                        messages.Add(msg);
                    }
                }

                return messages.ToArray();
            }
            return new Message[0];
        }

        /// <summary>
        /// Get all messages from or to a specific user
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <param name="respondent_id">The id of the user</param>
        /// <returns></returns>
        public Message[] GetMessages(String respondent_id)
        {
            Exception ex;
            return GetMessages(respondent_id, out ex);
        }

        /// <summary>
        /// Get number of unread messages
        /// </summary>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <returns></returns>
        public int? GetUnreadMessagesCount(out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return null;
            }
            wcReset();

            JObject ret = this.Get("user/messages/unread-count", new NameValueCollection());

            if (ret.GetValue<int>("ok") == 1)
            {
                int count = ret.GetValue<int>("count");
                return count;
            }

            return null;
        }

        /// <summary>
        /// Get number of unread messages
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <returns></returns>
        public int? GetUnreadMessagesCount()
        {
            Exception ex;
            return GetUnreadMessagesCount(out ex);
        }

        /// <summary>
        /// Send a message to another user
        /// </summary>
        /// <param name="respondent_id">The id of the user to send to</param>
        /// <param name="message">The text to send</param>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <returns></returns>
        public bool SendMessage(String respondent_id, String message, out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return false;
            }
            wcReset();

            JObject ret = this.Post("user/messages/send", new NameValueCollection() {
                { "respondent", respondent_id },
                { "text", message }
            });

            return ret.GetValue<int>("ok") == 1;
        }

        /// <summary>
        /// Send a message to another user
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <param name="respondent_id">The id of the user to send to</param>
        /// <param name="message">The text to send</param>
        /// <returns></returns>
        public bool SendMessage(String respondent_id, String message)
        {
            Exception ex;
            return SendMessage(respondent_id, message, out ex);
        }

        #endregion

        #region World

        /// <summary>
        /// Get the world status.
        /// Known statuses are: normal and empty
        /// </summary>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <returns></returns>
        public String WorldStatus(out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return null;
            }
            wcReset();

            JObject ret = this.Get("user/world-status", new NameValueCollection());

            if (ret.GetValue<int>("ok") == 1)
                return ret.GetValue<String>("status");

            return null;
        }

        /// <summary>
        /// Get the world status.
        /// Known statuses are: normal and empty
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <returns></returns>
        public String WorldStatus()
        {
            Exception ex;
            return WorldStatus(out ex);
        }

        /// <summary>
        /// Get a list of all rooms you have started in?
        /// </summary>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <returns></returns>
        public String[] WorldStartRoom(out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return new String[0];
            }
            wcReset();

            JObject ret = this.Get("user/world-start-room", new NameValueCollection());

            if (ret.GetValue<int>("ok") == 1)
            {
                JArray arr = ret.GetValue<JArray>("room");
                List<String> rooms = new List<String>();
                foreach (String room in arr)
                    rooms.Add(room);

                return rooms.ToArray();
            }

            return new String[0];
        }

        /// <summary>
        /// Get a list of all rooms you have started in?
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <returns></returns>
        public String[] WorldStartRoom()
        {
            Exception ex;
            return WorldStartRoom(out ex);
        }

        #endregion

        #region Room Information

        /// <summary>
        /// Get basic terrain info of a room
        /// <para>Note: Requires no auth</para>
        /// </summary>
        /// <param name="roomName">The name of the room</param>
        /// <returns></returns>
        public TerrainData[] RoomTerrainData(String roomName)
        {
            wcReset();

            JObject ret = this.Get("game/room-terrian", new NameValueCollection() {
                { "room", roomName },
                { "encoded", false.ToString() }
            });

            if(ret.GetValue<int>("ok") == 1)
            {
                return TerrainData.ParseList(ret.GetValue<JArray>("terrain"));
            }

            return new TerrainData[0];
        }

        /// <summary>
        /// Get basic terrain info of a room in serialized form
        /// See <see cref="SerializedTerrainData.Terrain"/> for info
        /// <para>Note: Requires no auth</para>
        /// </summary>
        /// <param name="roomName">The name of the room</param>
        /// <returns></returns>
        public SerializedTerrainData RoomTerrainDataEncoded(String roomName)
        {
            wcReset();

            JObject ret = this.Get("game/room-terrian", new NameValueCollection() {
                { "room", roomName },
                { "encoded", true.ToString() }
            });

            if (ret.GetValue<int>("ok") == 1)
            {
                JObject obj = ret.GetValue<JArray>("terrain").Value<JObject>();
                return SerializedTerrainData.Parse(obj);
            }

            return null;
        }

        #endregion

        #region Leaderboard Information

        /// <summary>
        /// Get a users leaderboard information
        /// </summary>
        /// <param name="mode">The mode to see. Known values are: world</param>
        /// <param name="season">The season you want to see. Specified by: YEAR-MONTH (Month is 2 digits)</param>
        /// <param name="username">The username to find</param>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <returns></returns>
        public Leaderboard FindLeaderboard(String mode, String season, String username, out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return null;
            }
            wcReset();
            
            JObject ret = this.Get("leaderboard/find", new NameValueCollection() {
                { "mode", mode },
                { "username", username }
            });

            if (ret.GetValue<int>("ok") == 1)
            {
                return Leaderboard.Parse(ret);
            }

            return null;
        }

        /// <summary>
        /// Get a users leaderboard information
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <param name="mode">The mode to see. Known values are: world</param>
        /// <param name="season">The season you want to see. Specified by: YEAR-MONTH (Month is 2 digits)</param>
        /// <param name="username">The username to find</param>
        /// <returns></returns>
        public Leaderboard FindLeaderboard(String mode, String season, String username)
        {
            Exception ex;
            return FindLeaderboard(mode, season, username, out ex);
        }

        /// <summary>
        /// Get a users leaderboard information
        /// This will get all seasons
        /// </summary>
        /// <param name="mode">The mode to see. Known values are: world</param>
        /// <param name="username">The username to find</param>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <returns></returns>
        public Leaderboard[] FindLeaderboard(String mode, String username, out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return new Leaderboard[0];
            }
            wcReset();

            JObject ret = this.Get("leaderboard/find", new NameValueCollection() {
                { "mode", mode },
                { "username", username }
            });

            if (ret.GetValue<int>("ok") == 1)
            {
                return Leaderboard.ParseList(ret.GetValue<JArray>("list"));
            }

            return new Leaderboard[0];
        }

        /// <summary>
        /// Get a users leaderboard information
        /// This will get all seasons
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <param name="mode">The mode to see. Known values are: world</param>
        /// <param name="username">The username to find</param>
        /// <returns></returns>
        public Leaderboard[] FindLeaderboard(String mode, String username)
        {
            Exception ex;
            return FindLeaderboard(mode, username, out ex);
        }

        /// <summary>
        /// Get the leaderboard for a season.
        /// Limitied by 100 rows by default with an offset of 0
        /// </summary>
        /// <param name="mode">The mode to see. Known values are: world</param>
        /// <param name="season">The season you want to see. Specified by: YEAR-MONTH (Month is 2 digits)</param>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <param name="limit">Limit of many rows to retrieve</param>
        /// <param name="offset">Offset the leaderboard from this number</param>
        /// <returns></returns>
        public Leaderboard[] GetLeaderboard(String mode, String season, out Exception ex, int limit = 100, int offset = 0)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return new Leaderboard[0];
            }
            wcReset();

            JObject ret = this.Get("leaderboard/list", new NameValueCollection() {
                { "mode", mode },
                { "limit", limit.ToString() },
                { "offset", offset.ToString() },
                { "season", season }
            });

            if (ret.GetValue<int>("ok") == 1)
            {
                JObject users = ret.GetValue<JObject>("users");
                Leaderboard[] list = Leaderboard.ParseList(ret.GetValue<JArray>("list"));

                foreach(Leaderboard board in list)
                {
                    board.User = User.Parse(users.GetValue<JObject>(board.User_Id));
                }

                return list;
            }

            return new Leaderboard[0];
        }

        /// <summary>
        /// Get the leaderboard for a season.
        /// Limitied by 100 rows by default with an offset of 0
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <param name="mode">The mode to see. Known values are: world</param>
        /// <param name="season">The season you want to see. Specified by: YEAR-MONTH (Month is 2 digits)</param>
        /// <param name="limit">Limit of many rows to retrieve</param>
        /// <param name="offset">Offset the leaderboard from this number</param>
        /// <returns></returns>
        public Leaderboard[] GetLeaderboard(String mode, String season, int limit = 100, int offset = 0)
        {
            Exception ex;
            return GetLeaderboard(mode, season, out ex, limit, offset);
        }

        /// <summary>
        /// Get a list of all known seasons
        /// </summary>
        /// <param name="ex">The exception is set only if an error occurred</param>
        /// <returns></returns>
        public Season[] GetSeasons(out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return new Season[0];
            }
            wcReset();

            JObject ret = this.Get("leaderboard/seasons", new NameValueCollection());

            if (ret.GetValue<int>("ok") == 1)
            {
                return Season.ParseList(ret.GetValue<JArray>("seasons"));
            }

            return new Season[0];
        }

        /// <summary>
        /// Get a list of all known seasons
        /// NOTE: The exception is dismissed here
        /// </summary>
        /// <returns></returns>
        public Season[] GetSeasons()
        {
            Exception ex;
            return GetSeasons(out ex);
        }

        #endregion

        #region Console

        public bool SendCommand(String command, out Exception ex)
        {
            ex = null;
            if (this.AuthToken == null)
            {
                ex = new Exception("Not Signed In");
                return false;
            }
            wcReset();

            JObject ret = this.Post("user/console", new NameValueCollection() { { "expression", command } });

            return ret.GetValue<int>("ok") == 1;
        }

        public bool SendCommand(String command)
        {
            Exception ex;
            return SendCommand(command, out ex);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// A helper function for POST requests
        /// </summary>
        /// <param name="func">The specific API url to retrieve</param>
        /// <param name="data">The specific data to send to the API</param>
        /// <returns></returns>
        private JObject Post(String func, NameValueCollection data)
        {
            wcReset();

            wc.Headers.Add(HttpRequestHeader.Referer, "https://screeps.com/a/");
            wc.Headers.Add("Origin", "https://screep.com");
            byte[] retBytes = wc.UploadValues(APIUrl + func, "POST", data);
            this.AuthToken = wc.Headers.Get("X-Token");

            String retStr = Encoding.UTF8.GetString(retBytes);

            return JObject.Parse(retStr);
        }

        private JObject Post(String func, String data)
        {
            wcReset();

            wc.Headers.Add(HttpRequestHeader.Referer, "https://screeps.com/a/");
            wc.Headers.Add("Origin", "https://screep.com");
            String retStr = wc.UploadString(APIUrl + func, "POST", data);
            this.AuthToken = wc.Headers.Get("X-Token");

            return JObject.Parse(retStr);
        }

        /// <summary>
        /// A helper function for GET requests
        /// </summary>
        /// <param name="func">The specific API url to retrieve</param>
        /// <param name="data">The specific data to send to the API</param>
        /// <returns></returns>
        private JObject Get(String func, NameValueCollection data)
        {
            wcReset();

            String[] dataArr = new String[data.Count];
            int dataStrIndex = 0;
            foreach (String key in data.Keys)
            {
                dataArr[dataStrIndex] = key + "=" + data[key];
                dataStrIndex++;
            }

            String dataStr = String.Join("&", dataArr);

            byte[] retBytes = wc.DownloadData(APIUrl + func + "?" + dataStr);
            this.AuthToken = wc.Headers.Get("X-Token");

            String retStr = Encoding.UTF8.GetString(retBytes);

            return JObject.Parse(retStr);
        }

        /// <summary>
        /// Resets the WebClient and adds the Token to the headers
        /// </summary>
        private void wcReset()
        {
            wc.Dispose();
            wc = new WebClient();
            wc.Headers.Add("X-Token", this.AuthToken);
            wc.Headers.Add("X-Username", this.AuthToken);
        }

        /// <summary>
        /// A helper function to retrieve a Base64 encoded authorization string
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private String GetAuthString(String email, String password)
        {
            String auth1 = email + ":" + password;

            return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(auth1), Base64FormattingOptions.None);
        }

        #endregion
    }
}
