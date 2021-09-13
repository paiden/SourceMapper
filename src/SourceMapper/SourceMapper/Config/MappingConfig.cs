using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace SourceMapper.Config
{
    public sealed class MappingConfig
    {
        private readonly HashSet<IPropertySymbol> ignoredProps = new(SymbolEqualityComparer.Default);

        public bool IsIgnored(IPropertySymbol prop)
            => this.ignoredProps.Contains(prop);

        public void AddIgnoredProperty(IPropertySymbol propSymbol)
            => this.ignoredProps.Add(propSymbol);
    }
}
