namespace Newbe.Claptrap
{
    public class ClaptrapOptions
    {
        /// <summary>
        /// options about how to save state
        /// </summary>
        public StateSavingOptions StateSavingOptions { get; set; } = null!;

        /// <summary>
        /// options about how to recover state when event handling failed
        /// </summary>
        public StateRecoveryOptions StateRecoveryOptions { get; set; } = null!;

        /// <summary>
        /// options about how to load event
        /// </summary>
        public EventLoadingOptions EventLoadingOptions { get; set; } = null!;

        /// <summary>
        /// options about minions of this claptrap. null if this claptrap isn`t a master claptrap
        /// </summary>
        public MinionOptions? MinionOptions { get; set; }
    }
}