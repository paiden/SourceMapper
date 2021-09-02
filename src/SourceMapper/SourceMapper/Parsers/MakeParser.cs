using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceMapper.Parsers
{
    internal static class MakeParser
    {
        public static void Parse(ParseResult result, ParseContext parseContext, MemberAccessExpressionSyntax make)
        {
            var gen = make.Name as GenericNameSyntax;
            if (gen == null)
            {
                result.Report(ParseResult.Diag.MakeGenericTypeNotResolvedSM0001, make.GetLocation());
                return;
            }

            var makeTypeStyntax = gen.TypeArgumentList.Arguments[0];
            var makeType = parseContext.SemanticModel.GetTypeInfo(makeTypeStyntax);

            var invocation = make.Parent as InvocationExpressionSyntax;
            if (invocation == null)
            {
                result.Report(ParseResult.Diag.MakeConfigurationNotFoundSM0003, make.GetLocation());
                return;
            }

            var configureExpression = invocation.DescendantNodes().OfType<LambdaExpressionSyntax>().FirstOrDefault();
            if (configureExpression == null)
            {
                result.Report(ParseResult.Diag.MakeConfigurationNotFoundSM0003, make.GetLocation());
                return;
            }

            var cloneExpressions = configureExpression.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                .Where(mas => mas.Name.Identifier.ValueText.Equals(nameof(MakeConfig<object>.Cloneable)));

            foreach (var clone in cloneExpressions)
            {
                CloneableConfigParser.Parse(result, parseContext, makeType, clone);
            }

        }
    }
}
