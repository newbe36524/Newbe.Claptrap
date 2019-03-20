using System;
using System.Reflection;
using System.Threading.Tasks;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators
{
    public class ClaptrapEventMethodCodeInfo
    {
        public ClaptrapEventMethodMetadata Metadata { get; }

        public ClaptrapEventMethodCodeInfo(
            ClaptrapEventMethodMetadata metadata)
        {
            Metadata = metadata;
        }

        public string InterfaceName => $"I{Metadata.MethodInfo.Name}";
        
        public string ImplName => Metadata.MethodInfo.Name;

        public Type ReturnType
        {
            get
            {
                if (Metadata.MethodInfo.ReturnType == typeof(Task))
                {
                    return null;
                }

                if (Metadata.MethodInfo.ReturnType.IsConstructedGenericType &&
                    Metadata.MethodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    return Metadata.MethodInfo.ReturnType.GenericTypeArguments[0];
                }

                throw new NotSupportedException("method must return Task or Task<>");
            }
        }

        public ParameterInfo[] ParameterInfos
        {
            get
            {
                var parameterInfos = Metadata.MethodInfo.GetParameters();
                return parameterInfos;
            }
        }
    }
}