using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public abstract class DecoratedStateSaver : IStateSaver
    {
        public IStateSaver StateSaver { get; }
        public IClaptrapIdentity Identity => StateSaver.Identity;

        protected DecoratedStateSaver(
            IStateSaver stateSaver)
        {
            StateSaver = stateSaver;
        }

        public abstract Task SaveAsync(IState state);
    }
}