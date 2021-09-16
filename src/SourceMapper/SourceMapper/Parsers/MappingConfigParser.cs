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
            CallInfo mappingCall)
        {
            var mapConfig = new MappingConfig();
            var ignoreCalls = ParseUtils.FindCallsOfMethodInConfigLambda(
                parseContext, mappingCall, nameof(IMapConfig<object, object, object>.Ignore), optional: true);

            foreach (var ign in ignoreCalls)
            {
                ParseIgnoreCall(parseResult, mapConfig, sourceType, ign);
            }

            var postProcessCalls = ParseUtils.FindCallsOfMethodInConfigLambda(
                parseContext, mappingCall, nameof(IMapConfig<object, object, object>.PostProcess), optional: true);

            foreach (var pp in postProcessCalls)
            {
                ParsePostProcessCall(parseResult, mapConfig, sourceType, pp);
            }

            var activatorCalls = ParseUtils.FindCallsOfMethodInConfigLambda(
                parseContext, mappingCall, nameof(IMapConfig<object, object, object>.Activator), optional: true);

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
            CallInfo ignoreCall)
        {
            var ignoreIdentifier = ignoreCall.Invocation.DescendantNodes()
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
            CallInfo postProcessCall)
        {
            var arg = postProcessCall.ArgumentList;

            if (arg == null || arg.Arguments.Count <= 0)
            {
                parseResult.Report(ParseResult.Diag.SM9999GenericError, postProcessCall.Invocation.GetLocation(), "Could not find post process func.");
                return;
            }

            mapableConfig.PostProcess = arg.Arguments[0];
        }

        private static void ParseActivatorCall(
            ParseResult parseResult,
            MappingConfig mapableConfig,
            ITypeSymbol mapableType,
            CallInfo activatorCall)
        {
            var arg = activatorCall.ArgumentList;

            if (arg == null || arg.Arguments.Count <= 0)
            {
                parseResult.Report(ParseResult.Diag.SM9999GenericError, activatorCall.Invocation.GetLocation(), "Could not find activator func.");
                return;
            }

            mapableConfig.Activator = arg.Arguments[0];
        }
    }
}
