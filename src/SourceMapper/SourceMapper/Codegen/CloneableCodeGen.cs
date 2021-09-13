using CodegenCS;
using SourceMapper.Codegen;
using SourceMapper.Config;
using SourceMapper.Parsers;

namespace SourceMapper.Generators
{
    internal static class CloneableCodeGen
    {
        public static void WriteCloneMethodFor(
            CodegenTextWriter writer,
            ParseResult parseResult,
            ParserTypeInfo cloneable,
            MappingConfig config)
        {
            var typeName = cloneable.TypeName;
            writer.WithCBlock(
                beforeBlock: $"public static {typeName} Clone(this {typeName} source)",
                innerBlockAction: w => MappingCodeGen.WriteMappingBody(w, parseResult, cloneable, cloneable, config));
        }
    }
}
