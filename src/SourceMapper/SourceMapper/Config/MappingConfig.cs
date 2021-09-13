using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceMapper.Config
{
    public sealed class MappingConfig
    {
        private readonly HashSet<IPropertySymbol> ignoredProps = new(SymbolEqualityComparer.Default);

        public MappingConfig()
        {

        }

        public ArgumentSyntax? PostProcess { get; set; }

        public ArgumentSyntax? Activator { get; set; }

        public bool IsIgnored(IPropertySymbol prop)
            => this.ignoredProps.Contains(prop);

        public void AddIgnoredProperty(IPropertySymbol propSymbol)
            => this.ignoredProps.Add(propSymbol);
    }
}
