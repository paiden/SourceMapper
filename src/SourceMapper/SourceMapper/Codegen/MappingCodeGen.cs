using System.Linq;
using System.Text;
using CodegenCS;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceMapper.Config;
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
            if (sourceMappingInfo.BestConstructor == null && config.Activator == null)
            {
                w.WriteLine($"#error No valid constructor for type '{sourceMappingInfo.TypeName}' found");
            }
            else
            {
                w.WriteLine(GenerateMakeInstance(parseResult, config, target, sourceMappingInfo));
                w.WriteLine();

                var written = WritePropertyAssignments(w, parseResult, sourceMappingInfo, config);
                if (written > 0)
                {
                    w.WriteLine();
                }

                if (config.PostProcess != null)
                {
                    WritePostProcessing(w, config.PostProcess);
                    w.WriteLine();
                }

                w.WriteLine($"return obj;");
            }
        }

        public static string GenerateMakeInstance(
            ParseResult parseResult,
            MappingConfig config,
            ParserTypeInfo target,
            ParserMappingTypeInfo source)
        {
            if (config.Activator is ArgumentSyntax activatorArg)
            {
                var sb = new StringBuilder();
                if (activatorArg.ChildNodes().FirstOrDefault() is LambdaExpressionSyntax lambda)
                {
                    sb.AppendLine($"Func<{source.TypeName}, {target.TypeName}> instanceCreator = {lambda};");
                    sb.Append("var obj = instanceCreator(source);");
                }
                else
                {
                    sb.Append($"var obj = {activatorArg}(source);");
                }

                return sb.ToString();
            }
            else
            {
                return $"var obj = new {target.TypeName}({GenerateConstructorArgs(parseResult, source)});";
            }
        }

        private static int WritePropertyAssignments(
            CodegenTextWriter w,
            ParseResult parseResult,
            ParserMappingTypeInfo cloneable,
            MappingConfig config)
        {
            int written = 0;
            foreach (var p in cloneable.AssignmentProps)
            {
                if (config.IsIgnored(p))
                {
                    continue;
                }

                w.EnsureEmptyLine();
                w.Write($"obj.{p.Name} = source.{p.Name}{CloneCloneableProps(parseResult, p)};");
                written++;
            }

            w.EnsureEmptyLine();
            return written;
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
            if (postProcessingArg.ChildNodes().FirstOrDefault() is LambdaExpressionSyntax lambda)
            {
                w.WriteLine($"var postProcess = {lambda};");
                w.WriteLine("postProcess(ref obj, source);");
            }
            else
            {
                w.WriteLine($"{postProcessingArg}(ref obj, source);");
            }
        }

        private static string CloneCloneableProps(ParseResult parseResult, IPropertySymbol prop)
            => parseResult.Cloneables.ContainsKey(prop.Type) ? ".Clone()" : string.Empty;
    }
}
