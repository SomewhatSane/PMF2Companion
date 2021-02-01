using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PMF2Companion.API
{
    public class DeathLog
    {
        private readonly Plugin plugin;
        public DeathLog(Plugin plugin) => this.plugin = plugin;

        public async Task SendDeath(DiedEventArgs ev)
        {
            if (ev.Killer.DoNotTrack || ev.Target.DoNotTrack)
            {
                Log.Warn($"[] Cannot send data for death regading killer {ev.Killer.Nickname} ({ev.Killer.UserId} and target {ev.Target.Nickname} ({ev.Killer.UserId}). Killer or target has DNT enabled.");
                return;
            }

            if (ev.Killer.Team == Team.TUT & ev.Target.Team == Team.TUT)
                return;

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("APISecret", plugin.Config.ApiSecret),
                new KeyValuePair<string, string>("TargetNickname", ev.Target.Nickname),
                new KeyValuePair<string, string>("TargetUserID", ev.Target.UserId),
                new KeyValuePair<string, string>("TargetPlayerID", ev.Target.Id.ToString()),
                new KeyValuePair<string, string>("TargetClass", ev.Target.ReferenceHub.characterClassManager.CurClass.ToString()),
                new KeyValuePair<string, string>("KillerNickname", ev.Killer.Nickname),
                new KeyValuePair<string, string>("KillerUserID", ev.Killer.UserId),
                new KeyValuePair<string, string>("KillerPlayerID", ev.Killer.Id.ToString()),
                new KeyValuePair<string, string>("KillerClass", ev.Killer.ReferenceHub.characterClassManager.CurClass.ToString()),
                new KeyValuePair<string, string>("Damage", ev.HitInformations.GetDamageName()),
                new KeyValuePair<string, string>("ServerAddress", ServerConsole.Ip + ":" + ServerConsole.Port)
            });

            using (HttpClient Client = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                if (plugin.Config.VerboseMode) Log.Info($"[] Sending data to API regarding killer {ev.Killer.Nickname} ({ev.Killer.UserId}) and target {ev.Target.Nickname} ({ev.Target.UserId}). Please wait.");

                HttpResponseMessage Response = await Client.PostAsync($"https://api.{plugin.Config.CustomerAccountName}.pmf2.somewhatsane.co.uk/DeathLog.php", data);
                string ResponseMessage = Response.Content.ReadAsStringAsync().Result;
                string StatusCode = Response.StatusCode.ToString();

                if (Response.IsSuccessStatusCode)
                {
                    if (plugin.Config.VerboseMode)
                        Log.Info($"[{StatusCode}] Success. Response from API: {ResponseMessage}.");
                }

                else
                {
                    Log.Error($"[{StatusCode}] Error when sending data to API. Response from API: {ResponseMessage}.");
                }
            }
        }
    }
}
