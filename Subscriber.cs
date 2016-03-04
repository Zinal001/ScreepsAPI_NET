using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using Newtonsoft.Json.Linq;

namespace ScreepsAPI_NET
{
    /// <summary>
    /// Subscribe to different streams to receive the latest updates from the game
    /// </summary>
    public class Subscriber
    {
        /// <summary>
        /// The WebSocket
        /// </summary>
        private WebSocket socket = null;

        /// <summary>
        /// The Authorization Token obtained by called API.SignIn
        /// </summary>
        private String authToken = null;

        /// <summary>
        /// The account information
        /// </summary>
        private Account account = null;

        /// <summary>
        /// Triggered when the connection is opened
        /// </summary>
        public event EventHandler ConnectionOpened;

        /// <summary>
        /// Triggered when the server has authorized the token
        /// </summary>
        public event EventHandler Authorized;

        /// <summary>
        /// Triggered when a subscribed stream is updated
        /// Check the <see cref="StreamDataEventArgs.Stream_Name"/> to see what stream was updated
        /// or see the example below
        /// </summary>
        /// <example>
        /// if(e is CPUStreamEventArgs) { console.log("This is a CPU Stream update!"; }
        /// </example>
        public event EventHandler<StreamDataEventArgs> OnStreamUpdate;

        /// <summary>
        /// Triggered when an error occurred on the websocket
        /// </summary>
        public event EventHandler<ErrorEventArgs> OnError;

        /// <summary>
        /// Triggered when the connection is closed, either by the user or by an error
        /// </summary>
        public event EventHandler<CloseEventArgs> ConnectionClosed;

        public Subscriber()
        {

        }

        /// <summary>
        /// Open the connection with the server and authenticate the token
        /// </summary>
        /// <param name="authToken">A token retrieved by calling API.SignIn()</param>
        public void Authenticate(String authToken)
        {
            Authenticate(authToken, null);
        }

        /// <summary>
        /// Open the connection with the server and authenticate the token
        /// </summary>
        /// <param name="authToken">A token retrieved by calling API.SignIn()</param>
        /// <param name="account">An account obtained by calling API.Me()</param>
        public void Authenticate(String authToken, Account account)
        {
            this.authToken = authToken;

            if(account == null)
            {
                API api = new API();
                api.SetAuthToken(authToken);
                this.account = api.Me();
            }
            else
                this.account = account;

            this.socket = new WebSocket("wss://screeps.com/socket/websocket");
            this.socket.OnOpen += Socket_OnOpen;
            this.socket.OnClose += Socket_OnClose;
            this.socket.OnError += Socket_OnError;
            this.socket.OnMessage += Socket_OnMessage;

            this.socket.ConnectAsync();
        }

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void Disconnect()
        {
            if(this.socket != null)
            {
                this.socket.Close(CloseStatusCode.Normal, "User closed the connection");
                this.socket = null;
            }
        }

        /// <summary>
        /// Subscribe to a stream
        /// </summary>
        /// <param name="stream">The stream name. See <see cref="Streams"/> for a few stream names</param>
        public void Subscribe(String stream)
        {
            stream = stream.Replace("[USER_ID]", this.account._Id);
            this.socket.Send("subscribe " + stream);
        }

        /// <summary>
        /// Unsubscribe from a previously subscribed stream
        /// </summary>
        /// <param name="stream">The stream name. See <see cref="Streams"/> for a few stream names</param>
        public void Unsubscribe(String stream)
        {
            stream = stream.Replace("[USER_ID]", this.account._Id);
            this.socket.Send("unsubscribe " + stream);
        }

        /// <summary>
        /// Handles message parsing
        /// </summary>
        /// <param name="sender">The websocket</param>
        /// <param name="e"></param>
        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            if(e.Data.StartsWith("auth ok"))
            {
                this.authToken = e.Data.Substring(8);
                if (this.Authorized != null)
                    this.Authorized(this, new EventArgs());
            }
            else if(e.Data.StartsWith("time "))
            {
                //Who cares?!
            }
            else
            {
                JArray payload = JArray.Parse(e.Data);

                StreamDataEventArgs baseArgs = new StreamDataEventArgs(payload);

                if (baseArgs.Stream_Name == "user:" + this.account._Id + "/cpu")
                    baseArgs = new CPUStreamEventArgs(payload);
                else if (baseArgs.Stream_Name == "user:" + this.account._Id + "/console")
                    baseArgs = new ConsoleStreamEventArgs(payload);
                else if (baseArgs.Stream_Name.StartsWith("user:" + this.account._Id + "/memory"))
                    baseArgs = new MemoryStreamEventArgs(payload);

                if (this.OnStreamUpdate != null)
                    this.OnStreamUpdate(this, baseArgs);
            }
        }

        /// <summary>
        /// Triggered when the websocket gets an error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Socket_OnError(object sender, ErrorEventArgs e)
        {
            if (this.OnError != null)
                this.OnError(this, e);
        }

        /// <summary>
        /// Triggered when the websocket closes the connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            if (this.ConnectionClosed != null)
                this.ConnectionClosed(this, e);

            this.socket = null;
        }

        /// <summary>
        /// Triggered when a connection to the websocket is established
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Socket_OnOpen(object sender, EventArgs e)
        {
            if (this.ConnectionOpened != null)
                this.ConnectionOpened(this, e);

            this.socket.Send("auth " + this.authToken);
        }
    }

    /// <summary>
    /// A collection of stream names
    /// </summary>
    public static class Streams
    {
        /// <summary>
        /// Get the Last CPU ms and last Memory bytes
        /// </summary>
        public const String CPU = "user:[USER_ID]/cpu";

        /// <summary>
        /// Subscribe to the output from the console
        /// </summary>
        public const String Console = "user:[USER_ID]/console";

        /// <summary>
        /// Subscribe to a memory object.
        /// Append this string with the path to the memory object
        /// (Dont forget the / at the beginning of the appended string)
        /// NOTE: retrieving an object in the memory will result in an unparseable string: [object Object]
        /// </summary>
        public const String Memory = "user:[USER_ID]/memory";
    }

    /// <summary>
    /// The base stream arguments for the OnStreamUpdate function.
    /// Can be used with all types of streams
    /// </summary>
    public class StreamDataEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the stream
        /// </summary>
        public String Stream_Name { get; private set; }

        /// <summary>
        /// The data retrieved, usually in JSON
        /// </summary>
        public String RawData { get; private set; }

        public StreamDataEventArgs(JArray Payload)
        {
            this.Stream_Name = Payload.Value<String>(0);
            this.RawData = Newtonsoft.Json.JsonConvert.SerializeObject(Payload.Value<Object>(1));
        }
    }

    /// <summary>
    /// Stream arguments for a <see cref="Streams.CPU" stream/>
    /// </summary>
    public class CPUStreamEventArgs : StreamDataEventArgs
    {
        /// <summary>
        /// The current CPU
        /// </summary>
        public long CPU { get; private set; }
        
        /// <summary>
        /// The current Memory load
        /// </summary>
        public long Memory { get; private set; }

        public CPUStreamEventArgs(JArray Payload) : base(Payload)
        {
            JObject Data = Payload.Value<JObject>(1);
            this.CPU = Data.Value<long>("cpu");
            this.Memory = Data.Value<long>("memory");
        }
    }

    /// <summary>
    /// Stream arguments for a <see cref="Streams.Console" stream/>
    /// </summary>
    public class ConsoleStreamEventArgs: StreamDataEventArgs
    {
        /// <summary>
        /// The console logs
        /// </summary>
        public String[] Log { get; private set; }

        /// <summary>
        /// The console texts
        /// </summary>
        public String[] Results { get; private set; }

        public ConsoleStreamEventArgs(JArray Payload): base(Payload)
        {
            JObject Data = Payload.Value<JObject>(1);
            this.Log = Data.Value<String[]>("log");
            this.Results = Data.Value<String[]>("results");
        }
    }

    /// <summary>
    /// Stream arguments for a <see cref="Streams.Memory" stream/>
    /// </summary>
    public class MemoryStreamEventArgs : StreamDataEventArgs
    {
        /// <summary>
        /// The raw memory data retrieved 
        /// </summary>
        public String RawMemory { get; private set; }

        /// <summary>
        /// The parsed memory data retrieved
        /// </summary>
        public Object Memory { get; private set; }

        public MemoryStreamEventArgs(JArray Payload) : base(Payload)
        {
            String Data = Payload.Value<String>(1);
            this.RawMemory = Data;

            if (Data == "[object Object]")
                this.Memory = new object();
            else
                this.Memory = Newtonsoft.Json.JsonConvert.DeserializeObject(Data);
        }
    }

}
