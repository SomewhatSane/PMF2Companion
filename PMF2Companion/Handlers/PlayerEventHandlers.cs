using Exiled.Events.EventArgs;

namespace PMF2Companion.Handlers
{
    public class PlayerEventHandlers
    {
        private readonly Plugin plugin;
        public PlayerEventHandlers(Plugin plugin) => this.plugin = plugin;

        public void Verified(VerifiedEventArgs ev)
        {
            if (plugin.Config.EnableWatchlist) _ = plugin.Watchlist.CheckWatchlist(ev);
            if (plugin.Config.EnableWatchlistLegacy) _ = plugin.WatchlistLegacy.CheckWatchlistLegacy(ev);
        }

        public void Died(DiedEventArgs ev)
        {
            if (plugin.Config.EnableDeathLog) _ = plugin.DeathLog.SendDeath(ev);
        }
    }
}
