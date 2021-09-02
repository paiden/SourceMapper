using System;
using System.Text;
using CodegenCS;
using Microsoft.CodeAnalysis;
using SourceMapper.Config;
using SourceMapper.Extensions;
using SourceMapper.Parsers;

namespace SourceMapper.Generators
{
    internal static class CloneableCodeGen
    {
        public static void Generate(CodegenTextWriter writer, ParseResult result, TypeInfo cloneableType)
        {
            var cloneable = result.ParseInfos[cloneableType];
            var cloneableConfig = result.Cloneables[cloneableType];
            WriteCode(writer, cloneable, cloneableConfig);
        }

        private static void Foo(Func<object>? c)
        {
        }

        private static void WriteCode(CodegenTextWriter w, ParserTypeInfo cloneable, CloneableConfig config)
        {
            var typeName = cloneable.TypeName;
            w.WithCBlock($"public static {typeName} Clone(this {typeName} source)", w => CloneBody(w, cloneable, config));

            static void CloneBody(CodegenTextWriter w, ParserTypeInfo cloneable, CloneableConfig config)
            {
                var cloneableInfo = cloneable.GetCloneableInfo();
                if (cloneableInfo.BestConstructor == null)
                {
                    w.WriteLine($"#error No valid constructor for type '{cloneable.TypeName}' found");
                }
                else
                {
                    w.WriteCBlock(
                        GenerateMakeInstance(cloneableInfo),
                        w => WritePropertyAssignments(w, cloneableInfo, config),
                        ";");
                    w.WriteLine();
                    w.WriteLine($"return clone;");
                }
            }

            static string GenerateMakeInstance(CloneableParserTypeInfo cloneable)
            {
                return $"var clone = new {cloneable.TypeName}({GenerateConstructorArgs(cloneable)})";
            }

            static string GenerateConstructorArgs(CloneableParserTypeInfo cloneable)
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
                        .Append(',')
                        .Append(' ');
                }

                sb.Remove(sb.Length - 2, 2);

                return sb.ToString();
            }

            static void WritePropertyAssignments(
                CodegenTextWriter w,
                CloneableParserTypeInfo cloneable,
                CloneableConfig config)
            {

                foreach (var p in cloneable.AssignmentProps)
                {
                    if (config.IsIgnored(p))
                    {
                        continue;
                    }

                    w.EnsureEmptyLine();
                    w.Write($"{p.Name} = source.{p.Name},");
                }
            }
        }
    }
}
