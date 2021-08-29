using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceMapper
{
    public class FindMapablesSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> Candidates { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax cds && DerivesFromSourceMapperContext(cds))
            {
                this.Candidates.Add(cds);
            }
        }

        private static bool DerivesFromSourceMapperContext(ClassDeclarationSyntax cds)
            => (cds.BaseList?.ToString() ?? string.Empty).Contains("SourceMapperContext");
    }
}
