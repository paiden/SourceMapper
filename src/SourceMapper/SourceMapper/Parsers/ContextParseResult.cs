using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace SourceMapper.Parsers
{
    internal class ContextParseResult
    {
        private readonly HashSet<TypeInfo> cloneables = new();
        private readonly Dictionary<TypeInfo, ParserTypeInfo> parseInfos = new();

        public static ContextParseResult Empty = new();

        public Dictionary<TypeInfo, ParserTypeInfo> ParseInfos => parseInfos;

        public IReadOnlyCollection<TypeInfo> Cloneables => this.cloneables;

        public IEnumerable<TypeInfo> ConfiguredTypes => this.ParseInfos.Keys;

        public ContextParseResult()
        {

        }

        public void AddTypeInfo(TypeInfo type, ParserTypeInfo info)
        {
            this.ParseInfos.Add(type, info);
        }

        public void AddCloneable(TypeInfo type)
        {
            if (!this.ParseInfos.ContainsKey(type))
            {
                throw new InvalidOperationException("Boom");
            }

            this.cloneables.Add(type);
        }
    }
}
