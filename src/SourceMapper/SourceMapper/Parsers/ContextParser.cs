using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceMapper.Parsers
{
    internal static class ContextParser
    {


        public static void Parse(
            ParseResult parseResult,
            ParseContext parseContext,
            ClassDeclarationSyntax sourceMapperContext)
        {
            var members = sourceMapperContext.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var m in members)
            {
                ParseMember(parseResult, parseContext, m);
            }
        }

        private static void ParseMember(ParseResult result, ParseContext parseContext, MethodDeclarationSyntax mds)
        {
            var symbol = parseContext.SemanticModel.GetDeclaredSymbol(mds) as IMethodSymbol;
            if (symbol.Name == SourceMapperContext.ConfigureName)
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
            var cloneables = new List<TypeInfo>();

            var makeInvocations = syntax.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                .Where(mas => mas.Name.Identifier.ValueText == nameof(MappingConfig.Make));

            foreach (var make in makeInvocations)
            {
                MakeParser.Parse(result, parseContext, make);
            }
        }
    }
}
