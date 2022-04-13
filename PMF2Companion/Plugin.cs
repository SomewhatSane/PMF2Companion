using Exiled.API.Features;
using System;
using System.Reflection;
using PMF2Companion.API;
using PMF2Companion.Handlers;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace PMF2Companion
{
    public class Plugin : Plugin<Config>
    {
        public Watchlist Watchlist;
        public WatchlistLegacy WatchlistLegacy;
        public UpdateCheck UpdateCheck;

        public PlayerEventHandlers PlayerEventHandlers;

        public override string Name { get; } = "PMF2Companion";
        public override string Author { get; } = "SomewhatSane (thisis@somewhatsane.co.uk)";
        public override string Prefix { get; } = "pmf2";
        public override Version RequiredExiledVersion { get; } = new Version("5.1.3");

        public static readonly string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        internal const string lastModified = "2022/04/13 22:50 UTC";

        public override void OnEnabled()
        {
            Log.Info($"{Name} v{version} by {Author}. Last modified: {lastModified}.");
            Log.Info("Loading configuration.");

            Log.Info("Loading base scripts.");

            Watchlist = new Watchlist(this);
            WatchlistLegacy = new WatchlistLegacy(this);
            UpdateCheck = new UpdateCheck(this);

            if (Config.CheckForUpdate)
                _ = UpdateCheck.CheckForUpdate();

            if (string.IsNullOrWhiteSpace(Config.Pmf2ApiUrl))
            {
                Log.Error("PMF2 API url is null. PMF2Companion cannot continue.");
                return;
            }

            Log.Info("Registering Event Handlers.");
            PlayerEventHandlers = new PlayerEventHandlers(this);
            PlayerEvents.Verified += PlayerEventHandlers.Verified;

            Log.Info("Done.");
        }

        public override void OnDisabled()
        {
            if (!Config.IsEnabled) return;

            PlayerEvents.Verified -= PlayerEventHandlers.Verified;
            PlayerEventHandlers = null;

            Watchlist = null;
            WatchlistLegacy = null;
            UpdateCheck = null;

            Log.Info("Disabled.");
        }
    }
}
