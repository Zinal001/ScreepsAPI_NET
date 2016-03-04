using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreepsAPI_NET
{
    public class Message
    {
        /// <summary>
        /// The Id of the message
        /// </summary>
        public String _Id { get; set; }

        /// <summary>
        /// The user id
        /// </summary>
        public String User_Id { get; set; }

        /// <summary>
        /// The respondent id
        /// </summary>
        public String Respondent_Id { get; set; }

        /// <summary>
        /// The respondent object
        /// Might be null
        /// </summary>
        public User Respondent { get; set; }

        /// <summary>
        /// The date and time the message was sent or retrieved
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The type of the message
        /// Known values are: in and out
        /// </summary>
        public String Type { get; set; }

        /// <summary>
        /// The text body of the message
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Is the message unread
        /// </summary>
        public bool Unread { get; set; }

        /// <summary>
        /// The Id of the reply message
        /// Might be empty or null if a reply has not been sent
        /// </summary>
        public String OutMessage_Id { get; set; }

        public Message()
        {

        }

        public static Message Parse(JObject obj)
        {
            try
            {
                Message msg = new Message();

                msg._Id = obj.GetValue<String>("_id");
                msg.User_Id = obj.GetValue<String>("user");
                msg.Respondent_Id = obj.GetValue<String>("respondent");
                msg.Date = obj.GetValue<DateTime>("date");
                msg.Type = obj.GetValue<String>("type");
                msg.Text = obj.GetValue<String>("text");
                msg.Unread = obj.GetValue<bool>("unread");
                msg.OutMessage_Id = obj.GetValue<String>("outMessage");
                
                return msg;
            }
            catch { }

            return null;
        }

        public static Message[] ParseMessages(JObject obj)
        {
            JObject users = obj.GetValue<JObject>("users");
            JArray arr = obj.GetValue<JArray>("messages");
            List<Message> messages = new List<Message>(arr.Count);

            foreach (JObject Jobj in arr)
            {
                JObject Jmsg = Jobj.GetValue<JObject>("message");
                Message msg = Message.Parse(Jmsg);
                if(msg != null)
                {
                    JObject Juser = users.GetValue<JObject>(msg.Respondent_Id);
                    if (Juser != null)
                        msg.Respondent = User.Parse(Juser);

                    messages.Add(msg);
                }
            }


            return messages.ToArray();
        }

    }
}
