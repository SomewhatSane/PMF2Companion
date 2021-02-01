using Exiled.API.Features;
using System;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace PMF2Companion
{
    public class Plugin : Plugin<Config>
    {
        public API.DeathLog DeathLog;
        public API.Watchlist Watchlist;
        public API.WatchlistLegacy WatchlistLegacy;
        public UpdateCheck UpdateCheck;

        public Handlers.PlayerEventHandlers PlayerEventHandlers;

        public override string Name { get; } = "PMF2Companion";
        public override string Author { get; } = "SomewhatSane";
        public override string Prefix { get; } = "pmf2";
        public override Version RequiredExiledVersion { get; } = new Version("2.1.30");

        internal const string version = "1.0.2";
        internal const string lastModified = "2021/02/01 20:46 UTC";

        public override void OnEnabled()
        {
            if (!Config.IsEnabled) return;
            Log.Info($"{Name} v{version} by {Author}. Last Modified: {lastModified}.");
            Log.Info("Loading configuration.");

            Log.Info("Registering base Scripts.");

            DeathLog = new API.DeathLog(this);
            Watchlist = new API.Watchlist(this);
            WatchlistLegacy = new API.WatchlistLegacy(this);
            UpdateCheck = new UpdateCheck(this);

            if (Config.CheckForUpdate)
                _ = UpdateCheck.CheckForUpdate();


            Log.Info("Registering Event Handlers.");
            PlayerEventHandlers = new Handlers.PlayerEventHandlers(this);
            PlayerEvents.Verified += PlayerEventHandlers.Verified;
            PlayerEvents.Died += PlayerEventHandlers.Died;

            Log.Info("Done.");
        }

        public override void OnDisabled()
        {
            if (!Config.IsEnabled) return;

            PlayerEvents.Verified -= PlayerEventHandlers.Verified;
            PlayerEvents.Died -= PlayerEventHandlers.Died;
            PlayerEventHandlers = null;

            DeathLog = null;
            Watchlist = null;
            WatchlistLegacy = null;
            UpdateCheck = null;

            Log.Info("Disabled.");
        }
    }
}
