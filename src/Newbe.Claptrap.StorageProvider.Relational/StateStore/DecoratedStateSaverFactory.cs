﻿using System;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class DecoratedStateSaverFactory<TImpl>
        : DecoratedClaptrapComponentFactory<TImpl, IStateSaver>
        where TImpl : IStateSaver
    {
        public DecoratedStateSaverFactory(Func<IStateSaver, TImpl> func,
            RelationalStoreFactory relationalStoreFactory) : base(func,
            relationalStoreFactory)
        {
        }
    }
}