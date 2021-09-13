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

            var postProcessCalls = ParseUtils.FindCallsOfMethodWithName(
                parseContext, syntax, nameof(IMapConfig<object, object, object>.PostProcess));

            foreach (var pp in postProcessCalls)
            {
                ParsePostProcessCall(parseResult, mapConfig, sourceType, pp);
            }

            var activatorCalls = ParseUtils.FindCallsOfMethodWithName(
                parseContext, syntax, nameof(IMapConfig<object, object, object>.Activator));

            foreach (var ac in activatorCalls)
            {
                ParseActivatorCall(parseResult, mapConfig, sourceType, ac);
            }

            return mapConfig;
        }

        private static void ParseIgnoreCall(
            ParseResult parseResult,
            MappingConfig mapableConfig,
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
                mapableConfig.AddIgnoredProperty(ignoreProp);
            }
        }

        private static void ParsePostProcessCall(
            ParseResult parseResult,
            MappingConfig mapableConfig,
            ITypeSymbol mapableType,
            (InvocationExpressionSyntax syntax, IMethodSymbol symbol) postProcessCall)
        {
            var (syntax, symbol) = postProcessCall;
            var arg = syntax.DescendantNodes().OfType<ArgumentListSyntax>().FirstOrDefault();

            if (arg == null || arg.Arguments.Count <= 0)
            {
                parseResult.Report(ParseResult.Diag.SM9999GenericError, syntax.GetLocation(), "Could not find post process func.");
                return;
            }

            mapableConfig.PostProcess = arg.Arguments[0];
        }

        private static void ParseActivatorCall(
            ParseResult parseResult,
            MappingConfig mapableConfig,
            ITypeSymbol mapableType,
            (InvocationExpressionSyntax syntax, IMethodSymbol symbol) activatorCall)
        {
            var (syntax, symbol) = activatorCall;
            var arg = syntax.DescendantNodes().OfType<ArgumentListSyntax>().FirstOrDefault();

            if (arg == null || arg.Arguments.Count <= 0)
            {
                parseResult.Report(ParseResult.Diag.SM9999GenericError, syntax.GetLocation(), "Could not find activator func.");
                return;
            }

            mapableConfig.Activator = arg.Arguments[0];
        }
    }
}
