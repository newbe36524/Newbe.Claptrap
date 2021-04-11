using System;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public abstract class DecoratedClaptrapComponentFactory<TImpl, TInterface> : IClaptrapComponentFactory<TInterface>
        where TInterface : class, IClaptrapComponent
        where TImpl : TInterface
    {
        private readonly Func<IStateLoader, TImpl> _func;
        private readonly IClaptrapComponentFactory<IStateLoader> _relationalStoreFactory;

        protected DecoratedClaptrapComponentFactory(
            Func<IStateLoader, TImpl> func,
            RelationalStoreFactory relationalStoreFactory)
        {
            _func = func;
            _relationalStoreFactory = relationalStoreFactory;
        }

        public TInterface Create(IClaptrapIdentity claptrapIdentity)
        {
            var inner = _relationalStoreFactory.Create(claptrapIdentity);
            var re = _func.Invoke(inner);
            return re;
        }
    }
}