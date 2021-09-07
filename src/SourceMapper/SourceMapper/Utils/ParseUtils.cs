using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceMapper.Parsers;

namespace SourceMapper.Utils
{
    internal static class ParseUtils
    {
        internal static IReadOnlyList<(InvocationExpressionSyntax styntax, IMethodSymbol symbol)> FindCallsOfMethodWithName(
            ParseContext context, SyntaxNode node, string methodName)
        {
            // Oh man getting all fluent calls of a methis is really not that easy... try to use the symbosl to find them
            var calls = node.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Select(n => (node: n, symbol: TryGetMethodSymbolInfo(context, n)))
                .Where(tpl => tpl.symbol != null && tpl.symbol.Name.Equals(methodName))
                .ToList();

            static IMethodSymbol? TryGetMethodSymbolInfo(ParseContext context, SyntaxNode n)
            {
                try
                {
                    return context.SemanticModel.GetSymbolInfo(n).Symbol as IMethodSymbol;
                }
                catch
                {
                    return null;
                }
            }

            return calls;
        }
    }
}
