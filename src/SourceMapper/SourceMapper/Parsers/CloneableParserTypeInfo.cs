using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceMapper.Parsers
{
    public class CloneableParserTypeInfo
    {
        private readonly ParserTypeInfo generic;

        public string TypeName => this.generic.TypeName;

        public IMethodSymbol? BestConstructor { get; }

        public IReadOnlyList<IPropertySymbol> ConstructionProps { get; private set; }

        public IReadOnlyList<IPropertySymbol> AssignmentProps { get; private set; }

        private CloneableParserTypeInfo(
            ParserTypeInfo generic,
            IMethodSymbol? bestConstructor,
            IReadOnlyList<IPropertySymbol> constructionProps,
            IReadOnlyList<IPropertySymbol> assignmentProps)
        {
            this.generic = generic;
            this.BestConstructor = bestConstructor;
            this.ConstructionProps = constructionProps;
            this.AssignmentProps = assignmentProps;
        }

        public static CloneableParserTypeInfo Create(ParserTypeInfo info)
        {
            var constructionProps = new List<IPropertySymbol>(16);
            var best = FindBestCloneConstructor(info.Constructors, info.Properties, ref constructionProps);
            var assignmentProps = info.Properties.Except(constructionProps).ToList();
            return new CloneableParserTypeInfo(info, best, constructionProps, assignmentProps);
        }

        private static IMethodSymbol? FindBestCloneConstructor(
            IReadOnlyList<IMethodSymbol> constructors,
            IReadOnlyList<IPropertySymbol> props,
            ref List<IPropertySymbol> constructionProps)
        {
            foreach (var constr in constructors)
            {
                constructionProps.Clear();

                foreach (var param in constr.Parameters)
                {
                    var found = FindParamCompatibleProperty(param, props);
                    if (found == null)
                    {
                        break;
                    }

                    constructionProps.Add(found);
                }

                if (constructionProps.Count == constr.Parameters.Length)
                {
                    return constr;
                }
            }

            constructionProps.Clear();
            return null;

            static IPropertySymbol? FindParamCompatibleProperty(IParameterSymbol param, IReadOnlyList<IPropertySymbol> props)
                => props.FirstOrDefault(pi => pi.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));
        }


    }
}
