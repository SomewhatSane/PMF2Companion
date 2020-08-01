using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PMF2Companion.API
{
    public class Watchlist
    {
        private readonly Plugin plugin;
        public Watchlist(Plugin plugin) => this.plugin = plugin;
        public async Task CheckWatchlist(JoinedEventArgs ev)
        {
            using (HttpClient client = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true; //Allow invalid SSLs.

                var data = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("APISecret", plugin.Config.ApiSecret),
                    new KeyValuePair<string, string>("PlayerUserID", ev.Player.UserId),
                    new KeyValuePair<string, string>("ServerAddress", ServerConsole.Ip + ":" + ServerConsole.Port)
                });

                if (plugin.Config.VerboseMode) Log.Info($"[] Watchlist - Sending data to API to check Watchlist for {ev.Player.Nickname} ({ev.Player.UserId}). Please wait.");

                HttpResponseMessage Response = await client.PostAsync($"https://api.{plugin.Config.CustomerAccountName}.pmf2.somewhatsane.co.uk/WatchlistCheck.php", data);
                string ResponseMessage = Response.Content.ReadAsStringAsync().Result;
                string StatusCode = Response.StatusCode.ToString();

                if (Response.IsSuccessStatusCode)
                {
                    //Now, let's decode JSON.
                    JObject Json = JObject.Parse(ResponseMessage);
                    int WatchlistStatus = (int)Json["Status"];

                    if (WatchlistStatus == 1)
                    {
                        if (plugin.Config.VerboseMode) Log.Info($"[{StatusCode}] Watchlist - Success. Response from API: {ev.Player.Nickname} ({ev.Player.UserId}) found on Watchlist. Banning.");
                        int ID = (int)Json["ID"];
                        string UserID = (string)Json["UserID"];
                        string Offence = (string)Json["Offence"];
                        int OffenceNo = (int)Json["OffenceNo"];
                        int BanDuration = (int)Json["BanDuration"];
                        string Comments = (string)Json["Comments"];
                        string AddedBy = (string)Json["AddedBy"];
                        string DateTime = (string)Json["DateTime"];

                        //Now, make a logical ban reason.

                        string banReason = $"[AUTOMATIC-WATCHLIST-BAN] {Offence} (Offence {OffenceNo}) - {BanDuration} minute(s). Comments given (if any): {Comments}. Added to watchlist at: {DateTime}.";

                        //Now ban them.
                        ev.Player.Ban(BanDuration * 60, banReason, AddedBy); //Compensate as Scopophobia+ uses seconds instead of minutes.

                        if (plugin.Config.VerboseMode) Log.Info($"[] Watchlist - {ev.Player.Nickname} ({ev.Player.UserId}) is now banned. Sending ban back to API. Please wait.");
                        //Send it back!

                        var data2 = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("APISecret", plugin.Config.ApiSecret),
                            new KeyValuePair<string, string>("ID", ID.ToString()),
                            new KeyValuePair<string, string>("Username", ev.Player.Nickname), //Use this as we don't record the username when the report is added. We use the current one at the time of the ban!
                            new KeyValuePair<string, string>("UserID", UserID),
                            new KeyValuePair<string, string>("Offence", Offence),
                            new KeyValuePair<string, string>("OffenceNo", OffenceNo.ToString()),
                            new KeyValuePair<string, string>("BanDuration", BanDuration.ToString()),
                            new KeyValuePair<string, string>("Comments", Comments),
                            new KeyValuePair<string, string>("AddedBy", AddedBy),
                            new KeyValuePair<string, string>("DateTime", DateTime),
                            new KeyValuePair<string, string>("ServerAddress", ServerConsole.Ip + ":" + ServerConsole.Port)

                        });
                        Response = await client.PostAsync($"https://api.{plugin.Config.CustomerAccountName}.pmf2.somewhatsane.co.uk/WatchlistBanned.php", data2);
                        ResponseMessage = Response.Content.ReadAsStringAsync().Result;
                        StatusCode = Response.StatusCode.ToString();

                        if (Response.IsSuccessStatusCode && plugin.Config.VerboseMode)
                            Log.Info($"[{StatusCode}] Watchlist - Success. Response from API: {ResponseMessage}");

                        else
                            Log.Error($"[{StatusCode}] Watchlist - Error from API when trying to send after ban has been performed. Response from API: {ResponseMessage}");
                    }

                    else
                    {
                        if (plugin.Config.VerboseMode)
                            Log.Info($"[{StatusCode}] Watchlist - Success. Response from API: No Watchlist entry for {ev.Player.Nickname} ({ev.Player.UserId}) on Watchlist.");
                    }

                }
                else
                    Log.Error($"[{StatusCode}] Watchlist - Error. There was an error when performing Watchlist check. Response from API: {ResponseMessage}");
            }
        }
    }
}
