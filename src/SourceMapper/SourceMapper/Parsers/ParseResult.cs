using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using SourceMapper.Config;

namespace SourceMapper.Parsers
{
    internal class ParseResult
    {
        public static class Diag
        {
            const string Category = "SourceMapper";

            public static readonly DiagnosticDescriptor MakeGenericTypeNotResolvedSM0001 = new(
                id: "SM0001",
                category: Category,
                title: "Failed to resolve generic type argument for 'Make<>'",
                messageFormat: "Failed to resolve generic type argument for 'Make<>'",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

            public static readonly DiagnosticDescriptor FailedToCreateTypeInfoSM0002 = new(
                id: "SM0002",
                category: Category,
                title: "Failed to resolve generic type argument for 'Make<>'",
                messageFormat: "Failed to resolve generic type argument for '{0}'",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);

            public static readonly DiagnosticDescriptor MakeConfigurationNotFoundSM0003 = new(
                id: "SM0003",
                category: Category,
                title: "Failed to parse Make<> configration",
                messageFormat: "Failed to parse Make<> configuration.",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);


            public static readonly DiagnosticDescriptor SM9999GenericError = new(
                id: "SM9999",
                category: Category,
                title: "Unhandled source mapper error",
                messageFormat: "Unhandled source mapper eror: {0}",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);
        }

        private readonly GeneratorExecutionContext executionContext;

        internal void EnsureHasTypeInfo(ITypeSymbol makeType)
        {
            if (!this.ParseInfos.ContainsKey(makeType))
            {
                var typeParseInfo = TypeInfoParser.Parse(makeType);
                this.parseInfos.Add(makeType, typeParseInfo);
            }
        }

        private readonly Dictionary<ITypeSymbol, ParserTypeInfo> parseInfos = new(SymbolEqualityComparer.Default);

        private readonly Dictionary<ITypeSymbol, MappingConfig> cloneables = new Dictionary<ITypeSymbol, MappingConfig>(SymbolEqualityComparer.Default);
        private readonly Dictionary<ITypeSymbol, Dictionary<ITypeSymbol, Config.MappingConfig>> mapables = new Dictionary<ITypeSymbol, Dictionary<ITypeSymbol, Config.MappingConfig>>(SymbolEqualityComparer.Default);

        public Dictionary<ITypeSymbol, ParserTypeInfo> ParseInfos => parseInfos;

        public IReadOnlyDictionary<ITypeSymbol, MappingConfig> Cloneables => this.cloneables;
        public IReadOnlyDictionary<ITypeSymbol, Dictionary<ITypeSymbol, MappingConfig>> Mapables => this.mapables;

        public IEnumerable<ITypeSymbol> ConfiguredTypes => this.ParseInfos.Keys;

        public ParseResult(GeneratorExecutionContext generatorContext)
        {
            this.executionContext = generatorContext;
        }

        public void AddCloneable(ITypeSymbol type, MappingConfig config)
        {
            if (!this.ParseInfos.ContainsKey(type))
            {
                this.EnsureHasTypeInfo(type);
            }

            this.cloneables.Add(type, config);
        }

        public void AddMappable(ITypeSymbol source, ITypeSymbol target, MappingConfig config)
        {
            if (!this.ParseInfos.ContainsKey(source))
            {
                this.EnsureHasTypeInfo(source);
            }

            if (!this.ParseInfos.ContainsKey(target))
            {
                this.EnsureHasTypeInfo(target);
            }

            if (!this.mapables.TryGetValue(source, out var targetTypes))
            {
                targetTypes = new Dictionary<ITypeSymbol, MappingConfig>(SymbolEqualityComparer.Default);
                this.mapables.Add(source, targetTypes);
            }

            targetTypes.Add(target, config);
        }

        public void Report(DiagnosticDescriptor descriptor, Location? location, params object[] msgArgs)
        {
            this.executionContext.ReportDiagnostic(Diagnostic.Create(descriptor, location, msgArgs));
        }
    }
}
