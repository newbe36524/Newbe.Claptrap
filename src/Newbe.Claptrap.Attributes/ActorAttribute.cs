using System;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ActorAttribute : Attribute
    {
        public ActorType ActorType { get; }

        public ActorAttribute(ActorType actorType)
        {
            ActorType = actorType;
        }
    }
}