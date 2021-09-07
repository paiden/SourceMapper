using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceMapper.Utils;

namespace SourceMapper.Parsers
{
    internal static class MakeParser
    {
        public static void Parse(ParseResult result, ParseContext parseContext, (InvocationExpressionSyntax syntax, IMethodSymbol symbol) makeCall)
        {
            var (syntax, symbol) = makeCall;
            ITypeSymbol makeType = symbol.TypeArguments[0]!;


            var cloneableCalls = ParseUtils.FindCallsOfMethodWithName(parseContext, syntax, nameof(MakeConfig<object>.Cloneable));
            foreach (var clone in cloneableCalls)
            {
                CloneableConfigParser.Parse(result, parseContext, makeType, clone);
            }

        }
    }
}
