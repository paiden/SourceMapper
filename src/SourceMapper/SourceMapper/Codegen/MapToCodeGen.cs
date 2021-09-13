using CodegenCS;
using SourceMapper.Codegen;
using SourceMapper.Config;
using SourceMapper.Parsers;

namespace SourceMapper.Generators
{
    internal static class MapToCodeGen
    {
        public static void WriteMakeToMethod(
            CodegenTextWriter w,
            ParseResult parseResult,
            ParserTypeInfo sourceType,
            ParserTypeInfo targetType,
            MappingConfig config)
        {
            var srcType = sourceType.TypeName;
            var tgtType = targetType.TypeName;
            w.WithCBlock($"public static {tgtType} MapTo<T>(this {srcType} source) where T : {tgtType}", w => MapToBody(w, parseResult, sourceType, targetType, config));
        }

        private static void MapToBody(
            CodegenTextWriter writer,
            ParseResult result,
            ParserTypeInfo sourceType,
            ParserTypeInfo targetType,
            MappingConfig config)
        {
            MappingCodeGen.WriteMappingBody(writer, result, sourceType, targetType, config);
        }
    }
}
