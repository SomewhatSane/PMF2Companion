using Exiled.API.Interfaces;
using System.ComponentModel;

namespace PMF2Companion
{
    public class Config : IConfig
    {
        [Description("Whether the plugin is enabled or not.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Customer account name. This should all be in lowercase and with no spaces. Ask SomewhatSane if you don't know what yours is.")]
        public string CustomerAccountName { get; private set; } = null;

        [Description("Your API secret. DO NOT GIVE THIS OUT as if you do, others can post data to your API.")]
        public string ApiSecret { get; private set; } = null;

        [Description("Enable or disable Watchlist.")]
        public bool EnableWatchlist { get; private set; } = true;

        [Description("Enable or disable Watchlist Legacy.")]
        public bool EnableWatchlistLegacy { get; private set; } = true;

        [Description("Enable or disable Death Log. Do not enable if you don't have Death Log enabled on your instance of PMF2!")]
        public bool EnableDeathLog { get; private set; } = false;

        [Description("Check for an update to PMF2Companion on startup.")]
        public bool CheckForUpdate { get; private set; } = true;

        [Description("Verbose mode. Prints more console messages with useful information.")]
        public bool VerboseMode { get; private set; } = false;
    }
}
