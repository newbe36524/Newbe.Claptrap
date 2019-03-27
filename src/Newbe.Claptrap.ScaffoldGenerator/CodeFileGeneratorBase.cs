using System;
using Microsoft.CodeAnalysis;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public abstract class CodeFileGeneratorBase<TContext, TCodeFile> : ICodeFileGenerator
        where TContext : ICodeFileGeneratorContext
        where TCodeFile : ICodeFile
    {
        public ICodeFile CreateCodeFile(ICodeFileGeneratorContext context)
        {
            if (!(context is TContext c))
            {
                throw new ArgumentOutOfRangeException(nameof(context));
            }

            return CreateCodeFileCore(c);
        }

        public abstract TCodeFile CreateCodeFileCore(TContext context);

        public SyntaxTree GenerateCode(ICodeFile codeFile)
        {
            if (!(codeFile is TCodeFile file))
            {
                throw new ArgumentOutOfRangeException(nameof(codeFile));
            }

            return GenerateCore(file);
        }

        public abstract SyntaxTree GenerateCore(TCodeFile file);
    }
}