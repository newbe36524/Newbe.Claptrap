using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators
{
    public class ClaptrapEventMethodCodeInfo
    {
        public ClaptrapEventMethodMetadata Metadata { get; }
        public MethodDeclarationSyntax MethodDeclarationSyntax { get; }

        public ClaptrapEventMethodCodeInfo(
            ClaptrapEventMethodMetadata metadata,
            MethodDeclarationSyntax methodDeclarationSyntax)
        {
            Metadata = metadata;
            MethodDeclarationSyntax = methodDeclarationSyntax;
        }

        public string InterfaceName => $"I{Metadata.MethodInfo.Name}";

        public string ImplName => Metadata.MethodInfo.Name;

        public string UnwrapTaskReturnType
        {
            get
            {
                if (MethodDeclarationSyntax.ReturnType is GenericNameSyntax returnType)
                {
                    if (returnType.Identifier.ToString() == "Task")
                    {
                        var typeString = returnType.TypeArgumentList.ToString();
                        // remove '<' and '>'
                        var re = typeString.Substring(1, typeString.Length - 2);
                        return re;
                    }
                }
                else if (MethodDeclarationSyntax.ReturnType.ToString() == "Task")
                {
                    return string.Empty;
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