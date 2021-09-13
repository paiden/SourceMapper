using System.Linq;
using System.Text;
using CodegenCS;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceMapper.Config;
using SourceMapper.Extensions;
using SourceMapper.Parsers;

namespace SourceMapper.Codegen
{
    internal static class MappingCodeGen
    {
        public static void WriteMappingBody(
            CodegenTextWriter w,
            ParseResult parseResult,
            ParserTypeInfo source,
            ParserTypeInfo target,
            MappingConfig config)
        {
            var sourceMappingInfo = source.GetCloneableInfo();
            if (sourceMappingInfo.BestConstructor == null)
            {
                w.WriteLine($"#error No valid constructor for type '{sourceMappingInfo.TypeName}' found");
            }
            else
            {
                w.WriteCBlock(
                    GenerateMakeInstance(parseResult, sourceMappingInfo),
                    w => WritePropertyAssignments(w, parseResult, sourceMappingInfo, config),
                    ";");
                w.WriteLine();

                if (config.PostProcess != null)
                {
                    WritePostProcessing(w, config.PostProcess);
                }

                w.WriteLine($"return obj;");
            }
        }

        public static string GenerateMakeInstance(ParseResult parseResult, ParserMappingTypeInfo cloneable)
        {
            return $"var obj = new {cloneable.TypeName}({GenerateConstructorArgs(parseResult, cloneable)})";
        }

        private static void WritePropertyAssignments(
            CodegenTextWriter w,
            ParseResult parseResult,
            ParserMappingTypeInfo cloneable,
            MappingConfig config)
        {

            foreach (var p in cloneable.AssignmentProps)
            {
                if (config.IsIgnored(p))
                {
                    continue;
                }

                w.EnsureEmptyLine();
                w.Write($"{p.Name} = source.{p.Name}{CloneCloneableProps(parseResult, p)},");
            }
        }

        private static string GenerateConstructorArgs(ParseResult parseResult, ParserMappingTypeInfo cloneable)
        {
            if (cloneable.ConstructionProps.Count <= 0)
            {
                return string.Empty;
            }

            var props = cloneable.ConstructionProps;
            var sb = new StringBuilder(256);
            foreach (var prop in props)
            {
                sb.Append("source.")
                    .Append(prop.Name)
                    .Append(CloneCloneableProps(parseResult, prop))
                    .Append(',')
                    .Append(' ');
            }

            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        private static void WritePostProcessing(
            CodegenTextWriter w,
            ArgumentSyntax postProcessingArg)
        {
            if (postProcessingArg.ChildNodes().FirstOrDefault() is ParenthesizedLambdaExpressionSyntax lambda)
            {
                w.WriteLine($"var postProcess = {lambda};");
                w.WriteLine("postProcess(ref obj, source);");
            }
            else
            {
                w.WriteLine($"{postProcessingArg}(ref obj, source);");
            }

            w.WriteLine();
        }

        private static string CloneCloneableProps(ParseResult parseResult, IPropertySymbol prop)
            => parseResult.Cloneables.ContainsKey(prop.Type) ? ".Clone()" : string.Empty;
    }
}
