using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceMapper.Config;

namespace SourceMapper.Parsers
{
    internal static class CloneableConfigParser
    {
        public static void Parse(
            ParseResult parseResult,
            ParseContext parseContext,
            TypeInfo cloneType,
            MemberAccessExpressionSyntax cloneableCall)
        {
            var cloneableConfig = new CloneableConfig();

            var cloneableConfigLambda = GetConfigLambda(cloneableCall);
            if (cloneableConfigLambda == null)
            {
                parseResult.Report(ParseResult.Diag.SM9999GenericError, cloneableCall.GetLocation());
                return;
            }

            ParseCloneableConfig(parseResult, cloneableConfig, parseContext, cloneType, cloneableConfigLambda);

            parseResult.AddCloneable(cloneType, cloneableConfig);
        }

        private static void ParseCloneableConfig(
            ParseResult parseResult,
            CloneableConfig clonableConfig,
            ParseContext parseContext,
            TypeInfo cloneType,
            SimpleLambdaExpressionSyntax cloneableConfigLambda)
        {
            var ignoreCalls = cloneableConfigLambda.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                .Where(mas => mas.Name.Identifier.ValueText.Equals(nameof(CloneConfig<object>.Ignore)));

            foreach (var call in ignoreCalls)
            {
                ParseIgnoreCall(parseResult, clonableConfig, cloneType, call);
            }
        }

        private static void ParseIgnoreCall(ParseResult parseResult, CloneableConfig clonableConfig, TypeInfo cloneableType, MemberAccessExpressionSyntax ignoreCall)
        {
            // Resolving symbols for lambdas does not work well... get the proeprty by name from the type
            var ignoreConfigLambda = GetConfigLambda(ignoreCall);
            if (ignoreConfigLambda == null)
            {
                parseResult.Report(ParseResult.Diag.SM9999GenericError, ignoreCall.GetLocation(), "ign_nolocation");
                return;
            }

            var identifiers = ignoreConfigLambda.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Skip(1); // Skip the lambda paramter identifer

            foreach (var id in identifiers)
            {
                var ignoreProp = cloneableType.Type!.GetMembers(id.Identifier.ValueText)
                    .OfType<IPropertySymbol>()
                    .SingleOrDefault();

                if (ignoreProp == null)
                {
                    parseResult.Report(ParseResult.Diag.SM9999GenericError, null, "This should never happen");
                }
                else
                {
                    clonableConfig.AddIgnoredProperty(ignoreProp);
                }
            }
        }

        private static SimpleLambdaExpressionSyntax? GetConfigLambda(MemberAccessExpressionSyntax call)
        {
            if (call.Parent is not InvocationExpressionSyntax invocation)
            {
                return null;
            }

            if (invocation.ArgumentList.Arguments.Count <= 0)
            {
                return null;
            }

            return invocation.ArgumentList.DescendantNodes()
                .OfType<SimpleLambdaExpressionSyntax>()
                .FirstOrDefault();
        }
    }
}
