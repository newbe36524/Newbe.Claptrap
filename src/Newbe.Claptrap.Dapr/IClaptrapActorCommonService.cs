﻿using Autofac;

namespace Newbe.Claptrap.Dapr
{
    public interface IClaptrapActorCommonService
    {
        IClaptrapFactory ClaptrapFactory { get; }
        IClaptrapAccessor ClaptrapAccessor { get; }
        IClaptrapTypeCodeFactory ClaptrapTypeCodeFactory { get; }
        ILifetimeScope LifetimeScope { get; }
    }
}