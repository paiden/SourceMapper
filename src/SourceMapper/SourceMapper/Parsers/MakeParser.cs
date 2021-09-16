using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceMapper.Utils;

namespace SourceMapper.Parsers
{
    internal static class MakeParser
    {
        public static void Parse(ParseResult result, ParseContext parseContext, CallInfo makeCall)
        {
            ITypeSymbol makeType = makeCall.MethodSymbol.TypeArguments[0]!;

            var cloneableCalls = ParseUtils.FindCallsOfMethodInConfigLambda(parseContext, makeCall, nameof(MakeConfig<object>.Cloneable));
            foreach (var clone in cloneableCalls)
            {
                var config = MappingConfigParser.Parse(result, parseContext, makeType, clone);
                result.AddCloneable(makeType, config);
            }

            var mapToCalls = ParseUtils.FindCallsOfMethodInConfigLambda(parseContext, makeCall, nameof(MakeConfig<object>.MapTo));
            foreach (var map in mapToCalls)
            {
                var targetType = map.MethodSymbol.TypeArguments[0]!;
                var config = MappingConfigParser.Parse(result, parseContext, makeType, map);
                result.AddMappable(makeType, targetType, config);
            }
        }
    }
}
