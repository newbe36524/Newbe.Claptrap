using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public static class SyntaxHelper
    {
        public static (bool isSupport, string unwrapTaskReturnTypeName) UnwrapTaskReturnTypeName(TypeSyntax syntax)
        {
            string unwrapTaskReturnTypeName;
            if (syntax is GenericNameSyntax rType)
            {
                if (rType.Identifier.ToString() == "Task")
                {
                    unwrapTaskReturnTypeName = rType.TypeArgumentList.ToString().RemoveGtLt();
                }
                else
                {
                    return (false, string.Empty);
                }
            }
            else if (syntax.ToString() != "Task")
            {
                return (false, string.Empty);
            }
            else
            {
                unwrapTaskReturnTypeName = string.Empty;
            }

            return (true, unwrapTaskReturnTypeName);
        }
    }
}