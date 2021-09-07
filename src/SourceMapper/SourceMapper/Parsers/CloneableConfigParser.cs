using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceMapper.Config;
using SourceMapper.Utils;

namespace SourceMapper.Parsers
{
    internal static class CloneableConfigParser
    {
        public static void Parse(
            ParseResult parseResult,
            ParseContext parseContext,
            ITypeSymbol cloneType,
            (InvocationExpressionSyntax syntax, IMethodSymbol symbol) cloneableCall)
        {
            var cloneableConfig = new CloneableConfig();
            var (syntax, symbol) = cloneableCall;
            var ignoreCalls = ParseUtils.FindCallsOfMethodWithName(parseContext, syntax, nameof(CloneConfig<object>.Ignore));

            foreach (var ign in ignoreCalls)
            {
                ParseIgnoreCall(parseResult, cloneableConfig, cloneType, ign);
            }

            parseResult.AddCloneable(cloneType, cloneableConfig);
        }

        private static void ParseIgnoreCall(
            ParseResult parseResult,
            CloneableConfig clonableConfig,
            ITypeSymbol cloneableType,
            (InvocationExpressionSyntax syntax, IMethodSymbol symbol) ignoreCall)
        {

            var ignoreIdentifier = ignoreCall.syntax.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Last(); // The last identifier is the prop name.... maybe add nicer parse logic in future but for now this works.

            var ignoreProp = cloneableType.GetMembers(ignoreIdentifier.Identifier.ValueText)
                .OfType<IPropertySymbol>()
                .SingleOrDefault();

            if (ignoreProp == null)
            {
                parseResult.Report(ParseResult.Diag.SM9999GenericError, null, $"This should never happen: member '{ignoreIdentifier.Identifier.ValueText}' not found.");
            }
            else
            {
                clonableConfig.AddIgnoredProperty(ignoreProp);
            }
        }
    }
}
