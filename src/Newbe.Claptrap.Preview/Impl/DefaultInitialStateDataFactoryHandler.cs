using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.Metadata;
using Newbe.Claptrap.Preview.StateStore;

namespace Newbe.Claptrap.Preview
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