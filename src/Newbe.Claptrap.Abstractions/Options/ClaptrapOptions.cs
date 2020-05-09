namespace Newbe.Claptrap.Options
{
    public class ClaptrapOptions
    {
        /// <summary>
        /// options about how to save state
        /// </summary>
        public StateSavingOptions StateSavingOptions { get; set; }

        /// <summary>
        /// options about how to recover state when event handling failed
        /// </summary>
        public StateRecoveryOptions StateRecoveryOptions { get; set; }

        /// <summary>
        /// options about how to load event
        /// </summary>
        public EventLoadingOptions EventLoadingOptions { get; set; }

        /// <summary>
        /// options about minions of this claptrap. null if this claptrap isn`t a master claptrap
        /// </summary>
        public MinionOptions? MinionOptions { get; set; }
    }
}