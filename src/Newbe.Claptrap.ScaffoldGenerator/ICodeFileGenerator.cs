using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface ICodeFileGenerator
    {
        ICodeFile CreateCodeFile(ICodeFileGeneratorContext context);
        SyntaxTree Generate(ICodeFile codeFile);
    }

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

        public SyntaxTree Generate(ICodeFile codeFile)
        {
            if (!(codeFile is TCodeFile file))
            {
                throw new ArgumentOutOfRangeException(nameof(codeFile));
            }

            return GenerateCore(file);
        }

        public abstract SyntaxTree GenerateCore(TCodeFile file);
    }

    public interface ICodeFile
    {
    }

    public interface ICodeFileGeneratorContext
    {
    }
}