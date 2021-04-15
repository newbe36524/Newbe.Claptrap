using System;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public abstract class DecoratedClaptrapComponentFactory<TImpl, TInterface> : IClaptrapComponentFactory<TInterface>
        where TInterface : class, IClaptrapComponent
        where TImpl : TInterface
    {
        private readonly Func<TInterface, TImpl> _func;
        private readonly IClaptrapComponentFactory<TInterface> _relationalStoreFactory;

        protected DecoratedClaptrapComponentFactory(
            Func<TInterface, TImpl> func,
            RelationalStoreFactory relationalStoreFactory)
        {
            _func = func;
            _relationalStoreFactory = (IClaptrapComponentFactory<TInterface>) relationalStoreFactory;
        }

        public TInterface Create(IClaptrapIdentity claptrapIdentity)
        {
            var inner = _relationalStoreFactory.Create(claptrapIdentity);
            var re = _func.Invoke(inner);
            return re;
        }
    }
}