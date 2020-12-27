using System;
using Autofac;

namespace Newbe.Claptrap.Saga
{
    public static class ClaptrapActorExtensions
    {
        /// <summary>
        /// Create a SagaClaptrap to handle saga flow
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="masterIdentity"></param>
        /// <param name="flowKey"></param>
        /// <param name="userDataType"></param>
        /// <returns></returns>
        public static IDisposableSagaClaptrap CreateSagaClaptrap(this ILifetimeScope scope,
            IClaptrapIdentity masterIdentity,
            string flowKey,
            Type? userDataType = null)
        {
            return scope.CreateSagaClaptrap(() =>
                new SagaClaptrapIdentity(masterIdentity, flowKey, userDataType ?? typeof(object)));
        }

        /// <summary>
        /// Create a SagaClaptrap to handle saga flow
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="sagaClaptrapIdFactory"></param>
        /// <returns></returns>
        public static IDisposableSagaClaptrap CreateSagaClaptrap(
            this ILifetimeScope scope,
            Func<ISagaClaptrapIdentity> sagaClaptrapIdFactory)
        {
            var factory = scope.Resolve<SagaClaptrap.Factory>();
            var sagaClaptrapIdentity = sagaClaptrapIdFactory();
            var sagaClaptrap = factory.Invoke(sagaClaptrapIdentity);
            var re = new DisposableSagaClaptrap(sagaClaptrap, scope);
            return re;
        }
    }
}