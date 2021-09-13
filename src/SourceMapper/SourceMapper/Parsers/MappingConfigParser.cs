using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceMapper.Config;
using SourceMapper.Utils;

namespace SourceMapper.Parsers
{
    internal static class MappingConfigParser
    {
        public static MappingConfig Parse(
            ParseResult parseResult,
            ParseContext parseContext,
            ITypeSymbol sourceType,
            (InvocationExpressionSyntax syntax, IMethodSymbol symbol) mappingCall)
        {
            var mapConfig = new MappingConfig();
            var (syntax, symbol) = mappingCall;
            var ignoreCalls = ParseUtils.FindCallsOfMethodWithName(
                parseContext, syntax, nameof(IMapConfig<object, object, object>.Ignore));

            foreach (var ign in ignoreCalls)
            {
                ParseIgnoreCall(parseResult, mapConfig, sourceType, ign);
            }

            return mapConfig;
        }

        private static void ParseIgnoreCall(
            ParseResult parseResult,
            MappingConfig clonableConfig,
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
