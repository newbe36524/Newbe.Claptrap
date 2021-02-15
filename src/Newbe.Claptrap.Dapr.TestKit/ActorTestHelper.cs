using System;
using System.Linq;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.Dapr.TestKit
{
    public static class ActorTestHelper
    {
        /// <summary>
        /// Get attribute based claptrap design. It is used in unit test in common.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IClaptrapDesign GetDesign(Type type)
        {
            var claptrapBootstrapperBuilder = new AutofacClaptrapBootstrapperBuilder()
                .ScanClaptrapDesigns(new[] {type})
                .SetDesignValidation(false)
                .Build();
            var claptrapDesignStore = claptrapBootstrapperBuilder.DumpDesignStore();
            var claptrapDesign = claptrapDesignStore.First();
            return claptrapDesign;
        }
    }
}