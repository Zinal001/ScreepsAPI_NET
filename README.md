# ScreepsAPI_NET

A .NET Framework 4.0 Library written in C# for interacting with Screeps REST API (https://screeps.com)

##### Current Functionality:
* Authorize against the API with email and password
* Retrieve information about your account
* Retrieve basic information about another user
* Get your messages
* Send messages
* Get leaderboard information
* Get World Status
* Subscribe to various streams to get data in real-time.
* Retrieve room information such as terrain data and structure/creep data
* Download your code to your local computer
* Upload code from your local computer
* Send console commands

###### TODO
* Retrieve detailed room information
* Figure out how map-stats work
* More examples!

###### Requirements
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
* [websocket-sharp](https://github.com/sta/websocket-sharp)

**A very basic example to authenticate and retrieve leaderboard information:**

```c#

using ScreepsAPI_NET;

public void test()
{
  API api = new API();
  if(api.SignIn("ACCOUNT_NAME", "PASSWORD"))
  {
    Leaderboard board = api.FindLeaderboard("world", "2016-03", "Zinal001");
    console.log("Current Score/Rank: " + board.Score + ", " + board.Rank);
    //Outputs something like: "Current Score/Rank: 158148, 75"
  }
}

```
