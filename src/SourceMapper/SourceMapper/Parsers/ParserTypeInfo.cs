using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceMapper.Parsers
{
    public class ParserTypeInfo
    {
        private readonly Lazy<CloneableParserTypeInfo> lazyCloneableInfo;

        public string TypeName { get; }

        public IReadOnlyList<IMethodSymbol> Constructors { get; }

        public IReadOnlyList<IPropertySymbol> Properties { get; }

        public CloneableParserTypeInfo GetCloneableInfo() => this.lazyCloneableInfo.Value;

        public ParserTypeInfo(string typeName, IEnumerable<IPropertySymbol> properties, IEnumerable<IMethodSymbol> constructors)
        {
            this.TypeName = typeName;
            this.Constructors = constructors.OrderByDescending(c => c.Parameters.Length).ToList();
            this.Properties = properties.ToList();
            this.lazyCloneableInfo = new(this.InitCloneableInfo);
        }

        private CloneableParserTypeInfo InitCloneableInfo()
            => CloneableParserTypeInfo.Create(this);
    }
}
