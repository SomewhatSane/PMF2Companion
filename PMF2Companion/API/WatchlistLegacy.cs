﻿using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace PMF2Companion.API
{
    public class WatchlistLegacy
    {
        private readonly Plugin plugin;
        public WatchlistLegacy(Plugin plugin) => this.plugin = plugin;

        public async Task CheckWatchlistLegacy(JoinedEventArgs ev)
        {
            using (HttpClient client = new HttpClient())
            {
                var data = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("APISecret", plugin.Config.ApiSecret),
                    new KeyValuePair<string, string>("PlayerNickname", ev.Player.Nickname),
                    new KeyValuePair<string, string>("PlayerUserID", ev.Player.UserId),
                    new KeyValuePair<string, string>("PlayerPlayerID", ev.Player.Id.ToString()),
                    new KeyValuePair<string, string>("PlayerIPAddress", ev.Player.Connection.address),
                    new KeyValuePair<string, string>("DateAndTime", DateTime.Now.ToString()),
                    new KeyValuePair<string, string>("ServerAddress", ServerConsole.Ip + ":" + ServerConsole.Port)
                });

                if (plugin.Config.VerboseMode) Log.Info("[] Watchlist Legacy - Sending data to Watchlist Legacy.");

                HttpResponseMessage Response = await client.PostAsync($"https://api.{plugin.Config.CustomerAccountName}.pmf2.somewhatsane.co.uk/WatchlistLegacy.php", data);
                string ResponseMessage = Response.Content.ReadAsStringAsync().Result;
                string StatusCode = Response.StatusCode.ToString();

                if (Response.IsSuccessStatusCode && plugin.Config.VerboseMode)
                    Log.Info($"[{StatusCode}] Watchlist Legacy - Success. Response from API: {ResponseMessage}.");
                else
                    Log.Error($"[{StatusCode}] Watchlist Legacy - Error when sending data to API. Response from API: {ResponseMessage}.");
            }
        }
    }
}