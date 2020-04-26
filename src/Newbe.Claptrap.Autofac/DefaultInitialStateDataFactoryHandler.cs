using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.Autofac
{
    public class DefaultInitialStateDataFactoryHandler : IInitialStateDataFactoryHandler
    {
        private readonly IClaptrapRegistrationAccessor _claptrapRegistrationAccessor;

        public DefaultInitialStateDataFactoryHandler(
            IClaptrapRegistrationAccessor claptrapRegistrationAccessor)
        {
            _claptrapRegistrationAccessor = claptrapRegistrationAccessor;
        }

        public Task<IStateData> Create(IActorIdentity identity)
        {
            var findStateDataType = _claptrapRegistrationAccessor.FindStateDataType(identity.TypeCode);
            var stateData = (IStateData) Activator.CreateInstance(findStateDataType);
            return Task.FromResult(stateData);
        }
    }
}