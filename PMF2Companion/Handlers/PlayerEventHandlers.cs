using Exiled.Events.EventArgs;

namespace PMF2Companion.Handlers
{
    public class PlayerEventHandlers
    {
        private readonly Plugin plugin;
        public PlayerEventHandlers(Plugin plugin) => this.plugin = plugin;

        public void Joined(JoinedEventArgs ev)
        {
            _ = plugin.Watchlist.CheckWatchlist(ev);
            _ = plugin.WatchlistLegacy.CheckWatchlistLegacy(ev);
        }
    }
}
