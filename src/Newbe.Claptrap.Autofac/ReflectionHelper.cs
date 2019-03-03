using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Autofac
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetBaseType(Type type)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }
    }
}