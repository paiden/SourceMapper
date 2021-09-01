using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceMapper.Parsers
{
    internal static class ContextParser
    {


        public static ContextParseResult Parse(Compilation compilation, ClassDeclarationSyntax cds)
        {
            if (false)
            {
                Debugger.Launch();
            }

            var model = compilation.GetSemanticModel(cds.SyntaxTree);
            var members = cds.DescendantNodes().OfType<MethodDeclarationSyntax>();

            var result = new ContextParseResult();

            foreach (var m in members)
            {
                ParseMember(result, model, m);
            }

            return result;
        }

        private static void ParseMember(ContextParseResult result, SemanticModel model, MethodDeclarationSyntax mds)
        {
            var symbol = model.GetDeclaredSymbol(mds) as IMethodSymbol;
            if (symbol.Name == SourceMapperContext.ConfigureName)
            {
                ParseConfigure(result, model, mds, symbol); ;
            }
        }

        private static void ParseConfigure(
            ContextParseResult result,
            SemanticModel model,
            MethodDeclarationSyntax syntax,
            IMethodSymbol symbol)
        {
            var cloneables = new List<TypeInfo>();

            var memberAccess = syntax.DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            foreach (var ma in memberAccess)
            {
                if (ma.Name.Identifier.ValueText == nameof(MapBuilder.MakeCloneable))
                {
                    var gen = ma.Name as GenericNameSyntax;
                    var cloneableTypeSyntax = gen.TypeArgumentList.Arguments[0];
                    var cloneableType = model.GetTypeInfo(cloneableTypeSyntax);

                    if (!result.ParseInfos.ContainsKey(cloneableType))
                    {
                        var typeParseInfo = TypeInfoParser.Parse(cloneableType);
                        result.AddTypeInfo(cloneableType, typeParseInfo);
                    }

                    result.AddCloneable(cloneableType);
                }
            }
        }
    }
}
