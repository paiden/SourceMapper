using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceMapper.Utils;

namespace SourceMapper.Parsers
{
    internal static class ContextParser
    {
        public static void Parse(
            ParseResult parseResult,
            ParseContext parseContext,
            ClassDeclarationSyntax sourceMapperContext)
        {
            var members = sourceMapperContext.DescendantNodes()
                .OfType<MethodDeclarationSyntax>();

            foreach (var m in members)
            {
                ParseMember(parseResult, parseContext, m);
            }
        }

        private static void ParseMember(ParseResult result, ParseContext parseContext, MethodDeclarationSyntax mds)
        {
            var symbol = parseContext.SemanticModel.GetDeclaredSymbol(mds) as IMethodSymbol;
            if (symbol != null && symbol.Name == SourceMapperContext.ConfigureName)
            {
                ParseConfigure(result, parseContext, mds, symbol); ;
            }
        }

        private static void ParseConfigure(
            ParseResult result,
            ParseContext parseContext,
            MethodDeclarationSyntax syntax,
            IMethodSymbol symbol)
        {
            DbgUtils.LaunchDebugger();
            var cloneables = new List<TypeInfo>();

            var makeCalls = ParseUtils.FindCallsOfMethodWithName(parseContext, syntax, nameof(ContextConfig.Make));

            foreach (var make in makeCalls)
            {
                MakeParser.Parse(result, parseContext, make);
            }
        }

        private static IReadOnlyList<(InvocationExpressionSyntax syntax, IMethodSymbol symbol)>
            GetAllMakeCalls(ParseContext parseContext, MethodDeclarationSyntax configureDeclaration)
        {
            var dn = configureDeclaration.DescendantNodes().ToList();
            // Oh man getting all fluent calls of a methis is really not that easy... try to use the symbosl to find them
            var calls = configureDeclaration.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Select(n => (node: n, symbol: TryGetMethodSymbolInfo(parseContext, n)))
                .Where(tpl => tpl.symbol != null && tpl.symbol.Name.Equals(nameof(ContextConfig.Make)))
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
