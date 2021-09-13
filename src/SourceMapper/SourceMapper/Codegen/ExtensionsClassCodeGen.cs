using CodegenCS;
using Microsoft.CodeAnalysis;
using SourceMapper.Generators;
using SourceMapper.Parsers;

namespace SourceMapper.Codegen
{
    internal static class ExtensionsClassCodeGen
    {
        public static void GenerateExtensionsClass(
           CodegenTextWriter writer,
           ParseResult parseResult,
           ITypeSymbol makeType)
        {
            writer.WithCBlock($"namespace {makeType.ContainingNamespace.ToDisplayString()}", ExtensionClassDeclaration);
            writer.WriteLine();

            void ExtensionClassDeclaration(CodegenTextWriter w)
            {
                w.WithCBlock(
                    beforeBlock: $"public static class {makeType.Name}SourceMapperExtensions",
                    innerBlockAction: ExtensionClassBody);
            }

            void ExtensionClassBody(CodegenTextWriter writer)
            {
                if (parseResult.Cloneables.TryGetValue(makeType, out var config))
                {
                    var cloneableTypeInfo = parseResult.ParseInfos[makeType];
                    CloneableCodeGen.WriteCloneMethodFor(writer, parseResult, cloneableTypeInfo, config);
                }

                if (parseResult.Mapables.TryGetValue(makeType, out var mappings))
                {
                    var sourceTypeInfo = parseResult.ParseInfos[makeType];

                    foreach (var mapping in mappings)
                    {
                        var targetTypeInfo = parseResult.ParseInfos[mapping.Key];
                        MapToCodeGen.WriteMakeToMethod(writer, parseResult, sourceTypeInfo, targetTypeInfo, mapping.Value);
                    }
                }
            }
        }
    }
}
