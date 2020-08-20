using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Saga
{
    public interface ISagaFlowBuilder
    {
        ISagaFlowBuilder WithStep(Type step, Type compensateStep);
        ISagaFlowBuilder WithUserData(Dictionary<string, string> userData);
        SagaFlow Build();
    }

    public class SagaFlowBuilder : ISagaFlowBuilder
    {
        private readonly IList<Type> _steps = new List<Type>();
        private readonly IList<Type> _compensateSteps = new List<Type>();
        private Dictionary<string, string> _userData;

        public ISagaFlowBuilder WithStep(Type step, Type compensateStep)
        {
            _steps.Add(step);
            _compensateSteps.Add(compensateStep);
            return this;
        }

        public ISagaFlowBuilder WithUserData(Dictionary<string, string> userData)
        {
            _userData = userData;
            return this;
        }

        public SagaFlow Build()
        {
            var re = new SagaFlow
            {
                Steps = _steps,
                CompensateSteps = _compensateSteps,
                UserData = _userData,
            };
            return re;
        }

        public static SagaFlowBuilder Create()
        {
            return new SagaFlowBuilder();
        }
    }

    public static class SagaFlowBuilderExtensions
    {
        public static ISagaFlowBuilder WithStep<TStep, TCompensateStep>(this ISagaFlowBuilder builder)
            where TStep : ISagaStep
            where TCompensateStep : ISagaStep
        {
            builder.WithStep(typeof(TStep), typeof(TCompensateStep));
            return builder;
        }
    }
}