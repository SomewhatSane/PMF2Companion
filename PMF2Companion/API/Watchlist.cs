using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PMF2Companion.Objects;
using Utf8Json;

namespace PMF2Companion.API
{
    public class Watchlist
    {
        private readonly Plugin plugin;
        public Watchlist(Plugin plugin) => this.plugin = plugin;

        public async Task CheckWatchlist(VerifiedEventArgs ev)
        {
            using (HttpClient client = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true; //Allow invalid SSLs.

                FormUrlEncodedContent data = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("APISecret", plugin.Config.ApiSecret),
                    new KeyValuePair<string, string>("PlayerUserID", ev.Player.UserId),
                    new KeyValuePair<string, string>("ServerAddress", ServerConsole.Ip + ":" + ServerConsole.Port)
                });

                if (plugin.Config.VerboseMode) Log.Debug($"[] Watchlist - Sending data to API to check watchlist for {ev.Player.Nickname} ({ev.Player.UserId}). Please wait.");

                HttpResponseMessage Response = await client.PostAsync($"{plugin.Config.Pmf2ApiUrl}/WatchlistCheck.php", data);
                string ResponseMessage = Response.Content.ReadAsStringAsync().Result;
                string StatusCode = Response.StatusCode.ToString();

                if (Response.IsSuccessStatusCode)
                {
                    //Now, let's decode JSON.
                    WatchlistResponse watchlistResponse = JsonSerializer.Deserialize<WatchlistResponse>(ResponseMessage);

                    if (watchlistResponse.Status == 1)
                    {
                        if (plugin.Config.VerboseMode) Log.Debug($"[{StatusCode}] Watchlist - Success. Response from API: {watchlistResponse.Message}. Banning.");
                        int ID = watchlistResponse.ID;
                        string UserID = watchlistResponse.UserID;
                        string Offence = watchlistResponse.Offence;
                        int OffenceNo = watchlistResponse.OffenceNo;
                        int BanDuration = watchlistResponse.BanDuration;
                        string Comments = watchlistResponse.Comments;
                        string AddedBy = watchlistResponse.AddedBy;
                        string DateTime = watchlistResponse.DateTime;

                        //Now, make a logical ban reason.

                        string banReason = $"[AUTOMATIC-WATCHLIST-BAN] {Offence} (Offence {OffenceNo}) - {BanDuration} minute(s). Comments given (if any): {Comments}. Added to watchlist at: {DateTime}.";

                        //Now ban them.
                        ev.Player.Ban(BanDuration * 60, banReason, AddedBy); //Convert minutes into seconds for ban.

                        if (plugin.Config.VerboseMode) Log.Debug($"[] Watchlist - {ev.Player.Nickname} ({ev.Player.UserId}) is now banned. Sending ban back to API. Please wait.");
                        //Send it back!

                        FormUrlEncodedContent data2 = new FormUrlEncodedContent(new[]
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
                        Response = await client.PostAsync($"{plugin.Config.Pmf2ApiUrl}/WatchlistBanned.php", data2);
                        ResponseMessage = Response.Content.ReadAsStringAsync().Result;
                        StatusCode = Response.StatusCode.ToString();

                        if (Response.IsSuccessStatusCode)
                        {
                            if (plugin.Config.VerboseMode)
                                Log.Debug($"[{StatusCode}] Watchlist - Success. Response from API: {ResponseMessage}");
                        }
                        else
                            Log.Error($"[{StatusCode}] Watchlist - Error from API when trying to send after ban has been performed. Response from API: {ResponseMessage}");
                    }

                    else
                    {
                        if (plugin.Config.VerboseMode)
                            Log.Debug($"[{StatusCode}] Watchlist - Success. Response from API: {watchlistResponse.Message}");
                    }

                }
                else
                    Log.Error($"[{StatusCode}] Watchlist - Error. There was an error when performing watchlist check. Response from API: {ResponseMessage}");
            }
        }
    }
}
