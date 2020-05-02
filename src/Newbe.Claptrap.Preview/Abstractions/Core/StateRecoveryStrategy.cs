namespace Newbe.Claptrap.Preview.Abstractions.Core
{
    public enum StateRecoveryStrategy
    {
        /// <summary>
        /// Recovery state from state holder
        /// </summary>
        FromStateHolder,

        /// <summary>
        /// Recovery state from store
        /// </summary>
        FromStore
    }
}