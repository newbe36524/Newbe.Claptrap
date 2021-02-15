using System.Diagnostics;
using Newbe.Claptrap.Extensions;
using static Newbe.Claptrap.ClaptrapActivitySource.Tags;

namespace Newbe.Claptrap
{
    public static class ClaptrapActivitySourceExtensions
    {
        public static Activity? AddClaptrapTags(this Activity? activity, IClaptrapDesign design, IClaptrapIdentity identity)
        {
            return activity?
                .AddTag(IsMinion, design.IsMinion())
                .AddTag(TypeCode, identity.TypeCode)
                .AddTag(Id, identity.Id);
        }
    }
}