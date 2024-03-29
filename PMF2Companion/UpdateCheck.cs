﻿using Exiled.API.Features;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PMF2Companion
{
    public class UpdateCheck
    {
        private readonly Plugin plugin;
        public UpdateCheck(Plugin plugin) => this.plugin = plugin;
        public async Task CheckForUpdate()
        {
            Log.Info("Checking for update.");

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true; //Allow invalid SSLs.

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", $"{plugin.Name} Update Checker - Running {plugin.Name} v{Plugin.version}");
                HttpResponseMessage response = await client.GetAsync("https://scpsl.somewhatsane.co.uk/plugins/pmf2companion/latest.html");

                if (!response.IsSuccessStatusCode)
                {
                    Log.Error($"An error occurred when trying to check for update. Response from server: {response.StatusCode}");
                    return;
                }

                string data = await response.Content.ReadAsStringAsync();
                string[] dataarray = data.Split(';');
                if (dataarray[0] == Plugin.version)
                    Log.Info($"You are running the latest version of {plugin.Name}.");
                else if (dataarray[0] != Plugin.version)
                    Log.Info($"A new version of {plugin.Name} (v{dataarray[0]}) is available. Download it at: {dataarray[1]} .");
                else
                    Log.Error($"Unexpected reply from server when trying to check for update. Response from server: {data}");
            }
        }
    }
}
