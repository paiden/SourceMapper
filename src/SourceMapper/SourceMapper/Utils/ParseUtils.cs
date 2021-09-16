using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceMapper.Parsers;

namespace SourceMapper.Utils
{
    internal struct CallInfo
    {
        public InvocationExpressionSyntax Invocation;
        public IMethodSymbol MethodSymbol;
        public ArgumentListSyntax ArgumentList;
    }

    internal static class ParseUtils
    {
        private static List<CallInfo> EmptyCallsList = new ();

        internal static IReadOnlyList<CallInfo> FindCallsOfMethodWithName(
            ParseContext context, SyntaxNode node, string methodName)
        {
            // Oh man getting all fluent calls of a method is really not that easy... try to use the symbol to find them
            // The invocation expression often span multiple calls when using fluent syntax...
            var callInfos = new List<CallInfo>();
            var identifierNodes = node.DescendantNodes()
                .OfType<SimpleNameSyntax>()
                .Where(sns => sns.Identifier.ValueText == methodName);

            foreach(var ins in identifierNodes)
            {
                var ci = CreateCallInfo(context, ins);
                if(ci.HasValue)
                {
                    callInfos.Add(ci.Value);
                }
            }

            return callInfos;
        }

        internal static IReadOnlyList<CallInfo> FindCallsOfMethodInConfigLambda(
            ParseContext context,
            CallInfo call,
            string name,
            bool optional = false)
        {
            if(optional && call.ArgumentList.Arguments.Count <= 0)
            {
                return EmptyCallsList;
            }

            var configLambda = call.ArgumentList.Arguments[0];
            return FindCallsOfMethodWithName(context, configLambda, name);
        }

        private static CallInfo? CreateCallInfo(ParseContext context, SimpleNameSyntax nameSyntax)
        {
            var invocation = (InvocationExpressionSyntax)nameSyntax.Parent!.Parent!;
            var method = TryGetMethodSymbolInfo<IMethodSymbol>(context, nameSyntax);
            var argList = invocation.ArgumentList;

            if(method == null)
            {
                return null;
            }

            return new CallInfo()
            {
                Invocation = invocation,
                MethodSymbol = method,
                ArgumentList = argList,
            };
        }

        static T? TryGetMethodSymbolInfo<T>(ParseContext context, SyntaxNode n) where T : class, ISymbol
        {
            try
            {
                return context.SemanticModel.GetSymbolInfo(n).Symbol as T;
            }
            catch
            {
                return default;
            }
        }
    }
}
